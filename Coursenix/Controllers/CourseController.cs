using System.Drawing.Printing;
using System.Security.Claims;
using Coursenix.Models;
using Coursenix.Models.ViewModels;
using Coursenix.Repository;
using Coursenix.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nest;

namespace Coursenix.Controllers
{
    public class CourseController : Controller
    {
        private readonly Coursenix.Models.Context _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public const int PageSize = 9;

        public CourseController(Coursenix.Models.Context context, UserManager<AppUser> userManager, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }


        [Authorize]
        public async Task<IActionResult> Index(string selectedSubject = "", int selectedGrade = 0, string searchQuery = "", int page = 1)
        {
            // Get all courses with related data
            var coursesQuery = _context.Courses
                .Include(c => c.Teacher)
                .Include(c => c.GradeLevels)
                .AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(selectedSubject) && selectedSubject != "All subjects")
            {
                coursesQuery = coursesQuery.Where(c => c.Name.Contains(selectedSubject));
            }

            if (selectedGrade > 0)
            {
                coursesQuery = coursesQuery.Where(c => c.GradeLevels.Any(gl => gl.NumberOfGrade == selectedGrade));
            }

            if (!string.IsNullOrEmpty(searchQuery))
            {
                coursesQuery = coursesQuery.Where(c => c.Teacher.Name.Contains(searchQuery));
            }

            // Get total count for pagination
            var totalCourses = await coursesQuery.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalCourses / PageSize);

            // Apply pagination
            var courses = await coursesQuery
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            // Get filter options
            var availableSubjects = await _context.Courses
                .Select(c => c.Name)
                .Distinct()
                .OrderBy(s => s)
                .ToListAsync();

            var availableGrades = await _context.GradeLevels
                .Select(gl => gl.NumberOfGrade)
                .Distinct()
                .OrderBy(g => g)
                .ToListAsync();

            var viewModel = new CourseListViewModel
            {
                Courses = courses,
                AvailableSubjects = availableSubjects,
                AvailableGrades = availableGrades,
                SelectedSubject = selectedSubject,
                SelectedGrade = selectedGrade,
                SearchQuery = searchQuery,
                CurrentPage = page,
                TotalPages = totalPages,
                TotalCourses = totalCourses
            };

            return View(viewModel);
        }

        [Authorize(Roles = "Teacher")] // Only teachers can create courses
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            // Ensure user is a teacher
            var user = await _userManager.GetUserAsync(User);
            if (user == null || !await _userManager.IsInRoleAsync(user, "Teacher"))
            {
                return RedirectToAction("Login", "Account");
            }

            var model = new CreateCourseVM();
            return View("Create", model);
        }

        [Authorize(Roles = "Teacher")] // Only teachers can create courses
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCourseVM model)
        {
            // Validate model state
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Get current teacher
            var user = await _userManager.GetUserAsync(User);
            var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.AppUserId == user.Id);

            if (teacher == null)
            {
                ModelState.AddModelError("", "Teacher profile not found.");
                return View(model);
            }

            // Validate that at least one grade is selected
            if (model.GradeGroups == null || !model.GradeGroups.Any())
            {
                ModelState.AddModelError("", "Please select at least one grade and add groups.");
                return View(model);
            }

            // Handle thumbnail upload
            string thumbnailFileName = null;
            if (model.ThumbnailFile != null && model.ThumbnailFile.Length > 0)
            {
                thumbnailFileName = await SaveThumbnailAsync(model.ThumbnailFile);
                if (thumbnailFileName == null)
                {
                    ModelState.AddModelError("", "Failed to upload thumbnail. Please try again.");
                    return View(model);
                }
            }

            // Create the course
            var course = new Course
            {
                Name = model.CourseTitle,
                Description = model.CourseDescription,
                Location = model.Location,
                StudentsPerGroup = model.StudentsPerGroup,
                ThumbnailFileName = thumbnailFileName,
                TeacherId = teacher.Id
            };

            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            // Process each grade and its groups
            foreach (var gradeGroupsKvp in model.GradeGroups)
            {
                int gradeNumber = gradeGroupsKvp.Key;
                var groups = gradeGroupsKvp.Value;

                if (groups == null || !groups.Any()) continue;

                // Get the price from the first group (assuming same price for all groups in a grade)
                decimal gradePrice = groups.FirstOrDefault()?.Price ?? 0;

                // Create GradeLevel
                var gradeLevel = new GradeLevel
                {
                    CourseId = course.Id,
                    NumberOfGrade = gradeNumber,
                    Price = gradePrice
                };

                _context.GradeLevels.Add(gradeLevel);
                await _context.SaveChangesAsync();

                // Create groups for this grade
                foreach (var groupModel in groups)
                {
                    if (string.IsNullOrEmpty(groupModel.GroupName) ||
                        groupModel.Days == null || !groupModel.Days.Any())
                    {
                        continue; // Skip incomplete groups
                    }

                    var group = new Group
                    {
                        GradeLevelId = gradeLevel.Id,
                        Name = groupModel.GroupName,
                        SelectedDays = groupModel.Days.ToList(),
                        StartTime = groupModel.StartTime,
                        EndTime = groupModel.EndTime,
                        TotalSeats = model.StudentsPerGroup,
                        EnrolledStudentsCount = 0,
                        Location = model.Location
                    };

                    _context.Groups.Add(group);
                }
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Course created successfully!";
            return RedirectToAction("Index", "Teacher");
        }

        // Helper method to save thumbnail
        private async Task<string> SaveThumbnailAsync(IFormFile thumbnailFile)
        {
            if (thumbnailFile == null || thumbnailFile.Length == 0)
                return null;

            // Validate file type
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var fileExtension = Path.GetExtension(thumbnailFile.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(fileExtension))
            {
                return null; // Return null instead of throwing exception
            }

            // Generate unique filename
            var fileName = Guid.NewGuid().ToString() + fileExtension;
            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "thumbnails");

            // Create directory if it doesn't exist
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var filePath = Path.Combine(uploadsFolder, fileName);

            // Save file
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await thumbnailFile.CopyToAsync(fileStream);
            }

            return fileName;
        }

        // Helper method to delete thumbnail
        private void DeleteThumbnail(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return;

            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "thumbnails", fileName);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }

        // GET: Course/ViewCourse/5
        [HttpGet]
        public async Task<IActionResult> ViewCourse(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses
                .Include(c => c.Teacher)
                .Include(c => c.GradeLevels)
                    .ThenInclude(gl => gl.Groups)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (course == null)
            {
                return NotFound();
            }

            return View("ViewCourse", course);
        }

        public IActionResult GetGradeByCourse(int CourseId)
        {
            var grades = _context.GradeLevels
                .Where(g => g.CourseId == CourseId)
                .Select(g => new
                {
                    g.Id,
                    number = g.NumberOfGrade,
                }).ToList();
            return Json(grades);
        }

        // GET: Course/Edit/5
        [HttpGet]
        public async Task<IActionResult> EditCourse(int id)
        {
            var course = await _context.Courses
                .Include(c => c.Teacher)
                .Include(c => c.GradeLevels)
                    .ThenInclude(gl => gl.Groups) // Include Groups to calculate TotalGroups and check for grade level deletion constraint
                        .ThenInclude(g => g.Bookings) // Include Bookings to calculate TotalEnrollments
                .FirstOrDefaultAsync(c => c.Id == id);

            if (course == null)
            {
                return NotFound();
            }

            var viewModel = new EditCourseVM
            {
                Id = course.Id,
                Name = course.Name,
                Description = course.Description,
                Location = course.Location, // Corrected from course.L
                StudentsPerGroup = course.StudentsPerGroup,
                CurrentThumbnailUrl = course.ThumbnailFileName,
                TeacherId = course.TeacherId,
                TeacherName = course.Teacher?.Name,
                TotalGroups = course.GradeLevels?.SelectMany(gl => gl.Groups).Count() ?? 0,
                TotalEnrollments = course.GradeLevels?.SelectMany(gl => gl.Groups).SelectMany(g => g.Bookings).Count() ?? 0,
                GradeLevels = course.GradeLevels?.Select(gl => new GradeLevelVM
                {
                    Id = gl.Id,
                    NumberOfGrade = gl.NumberOfGrade,
                    Price = gl.Price
                }).ToList() ?? new List<GradeLevelVM>()
            };

            // Ensure there's at least one empty grade level for new entry if no grade levels exist
            if (!viewModel.GradeLevels.Any())
            {
                viewModel.GradeLevels.Add(new GradeLevelVM());
            }

            return View(viewModel);
        }

        // POST: Course/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCourse(EditCourseVM viewModel)
        {
            // Fetch the course with its grade levels and groups for update and validation checks
            var courseToUpdate = await _context.Courses
                .Include(c => c.GradeLevels)
                    .ThenInclude(gl => gl.Groups) // Important for checking if a grade level can be deleted
                .FirstOrDefaultAsync(c => c.Id == viewModel.Id);

            if (courseToUpdate == null)
            {
                return NotFound();
            }

            // Manually update scalar properties
            courseToUpdate.Name = viewModel.Name;
            courseToUpdate.Description = viewModel.Description;
            courseToUpdate.Location = viewModel.Location;
            courseToUpdate.StudentsPerGroup = viewModel.StudentsPerGroup;

            // Handle thumbnail upload/removal
            if (viewModel.RemoveThumbnail)
            {
                if (!string.IsNullOrEmpty(courseToUpdate.ThumbnailFileName))
                {
                    var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, courseToUpdate.ThumbnailFileName.TrimStart('/'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }
                courseToUpdate.ThumbnailFileName = null;
            }
            else if (viewModel.ThumbnailFile != null)
            {
                if (!string.IsNullOrEmpty(courseToUpdate.ThumbnailFileName))
                {
                    var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, courseToUpdate.ThumbnailFileName.TrimStart('/'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }
                var uniqueFileName = Guid.NewGuid().ToString() + "_" + viewModel.ThumbnailFile.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await viewModel.ThumbnailFile.CopyToAsync(fileStream);
                }
                courseToUpdate.ThumbnailFileName = "/uploads/" + uniqueFileName;
            }

            // --- Handle GradeLevels ---
            var gradeLevelsInDb = courseToUpdate.GradeLevels.ToList();
            var gradeLevelsFromForm = viewModel.GradeLevels.Where(gl => !gl.IsDeleted).ToList(); // Only consider non-deleted ones from the form

            // Identify and update existing grade levels, and add new ones
            foreach (var gradeLevelVM in gradeLevelsFromForm)
            {
                if (gradeLevelVM.Id.HasValue && gradeLevelVM.Id.Value != 0) // Existing grade level
                {
                    var existingGl = gradeLevelsInDb.FirstOrDefault(gl => gl.Id == gradeLevelVM.Id.Value);
                    if (existingGl != null)
                    {
                        existingGl.NumberOfGrade = gradeLevelVM.NumberOfGrade;
                        existingGl.Price = gradeLevelVM.Price;
                        _context.Entry(existingGl).State = EntityState.Modified; // Explicitly mark as modified
                    }
                }
                else // New grade level
                {
                    courseToUpdate.GradeLevels.Add(new GradeLevel
                    {
                        NumberOfGrade = gradeLevelVM.NumberOfGrade,
                        Price = gradeLevelVM.Price,
                        CourseId = courseToUpdate.Id
                    });
                }
            }

            // Identify and remove deleted grade levels
            foreach (var dbGradeLevel in gradeLevelsInDb)
            {
                // If a grade level from the database is not found in the form's submitted list
                if (!gradeLevelsFromForm.Any(glvm => glvm.Id == dbGradeLevel.Id))
                {
                    // Check if it has associated groups before deleting
                    if (dbGradeLevel.Groups != null && dbGradeLevel.Groups.Any())
                    {
                        ModelState.AddModelError("", $"Cannot delete Grade Level {dbGradeLevel.NumberOfGrade} as it has associated groups. Please delete its groups first.");
                        // Re-add this grade level to the view model so it is displayed again with the error
                        viewModel.GradeLevels.Add(new GradeLevelVM
                        {
                            Id = dbGradeLevel.Id,
                            NumberOfGrade = dbGradeLevel.NumberOfGrade,
                            Price = dbGradeLevel.Price,
                            IsDeleted = false // Ensure it's not marked as deleted in the VM for display
                        });
                    }
                    else
                    {
                        _context.GradeLevels.Remove(dbGradeLevel);
                    }
                }
            }

            if (!ModelState.IsValid)
            {
                // Re-populate display-only properties if validation fails
                viewModel.TeacherName = courseToUpdate.Teacher?.Name;
                viewModel.TotalGroups = courseToUpdate.GradeLevels?.SelectMany(gl => gl.Groups).Count() ?? 0;
                viewModel.TotalEnrollments = courseToUpdate.GradeLevels?.SelectMany(gl => gl.Groups).SelectMany(g => g.Bookings).Count() ?? 0;

                // Ensure there's at least one empty grade level if all were removed or none existed
                if (!viewModel.GradeLevels.Any(gl => !gl.IsDeleted))
                {
                    viewModel.GradeLevels.Add(new GradeLevelVM());
                }

                return View(viewModel);
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Course updated successfully!";
            return RedirectToAction("ViewCourse", new { id = courseToUpdate.Id });
        }

        private bool CourseExists(int id)
        {
            return _context.Courses.Any(e => e.Id == id);
        }

        // GET: Group/Edit/5
        [HttpGet]
        public async Task<IActionResult> EditGroup(int id)
        {
            var group = await _context.Groups
                .Include(g => g.GradeLevel)
                    .ThenInclude(gl => gl.Course)
                        .ThenInclude(c => c.Teacher)
                .Include(g => g.Bookings)
                    .ThenInclude(b => b.Student)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (group == null)
            {
                return NotFound();
            }
            // --- Calculate the sequential group number within its grade ---
            // 1. Get all groups for the current grade level, ordered by Id (or Name, StartTime, etc.)
            var groupsInGrade = await _context.Groups
                                              .Where(g => g.GradeLevelId == group.GradeLevelId)
                                              .OrderBy(g => g.Id) // Order by Id to get a consistent sequence
                                              .ToListAsync();

            // 2. Find the 0-based index of the current group in that ordered list
            int groupIndex = groupsInGrade.FindIndex(g => g.Id == group.Id);

            // 3. Convert to 1-based number (add 1)
            int groupNumber = groupIndex + 1;
            // ---------------------------------------------------------------

            var viewModel = new EditGroupVM
            {
                Id = group.Id,
                GradeLevelId = group.GradeLevelId,
                Name = group.Name,
                SelectedDays = group.SelectedDays ?? new List<string>(),
                StartTime = group.StartTime,
                EndTime = group.EndTime,
                TotalSeats = group.TotalSeats,
                Location = group.Location,
                CourseName = group.GradeLevel.Course.Name,
                CourseId = group.GradeLevel.CourseId,
                GradeNumber = group.GradeLevel.NumberOfGrade,
                EnrolledStudentsCount = group.EnrolledStudentsCount,
                EnrolledStudents = group.Bookings
                    .Where(b => b.Student != null)
                    .Select(b => new EnrolledStudent
                    {
                        Id = b.Student.Id,
                        Name = b.Student.Name,
                        Email = b.Student.Email,
                        EnrollmentDate = b.BookingDate
                    }).ToList(),
                GroupNumberInGrade = groupNumber // Assign the calculated sequential number

            };

            return View(viewModel);
        }

        // POST: Group/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditGroup(EditGroupVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Additional validation
            if (model.SelectedDays == null || !model.SelectedDays.Any())
            {
                ModelState.AddModelError("SelectedDays", "Please select at least one day");
                return View(model);
            }

            if (model.EndTime <= model.StartTime)
            {
                ModelState.AddModelError("EndTime", "End time must be after start time");
                return View(model);
            }

            var group = await _context.Groups.FindAsync(model.Id);
            if (group == null)
            {
                return NotFound();
            }

            // Check if reducing total seats below current enrollments
            if (model.TotalSeats < group.EnrolledStudentsCount)
            {
                ModelState.AddModelError("TotalSeats",
                    $"Cannot reduce total seats below current enrollments ({group.EnrolledStudentsCount})");
                return View(model);
            }
            // Re-calculate GroupNumberInGrade for re-display if validation fails
            var groupsInGrade = await _context.Groups
                                              .Where(g => g.GradeLevelId == model.GradeLevelId)
                                              .OrderBy(g => g.Id) // Must use the same order as GET
                                              .ToListAsync();
            int groupIndex = groupsInGrade.FindIndex(g => g.Id == model.Id);
            model.GroupNumberInGrade = groupIndex + 1;
            // --- End re-calculation for re-display ---

            // Update group properties
            group.Name = model.Name;
            group.SelectedDays = model.SelectedDays;
            group.StartTime = model.StartTime;
            group.EndTime = model.EndTime;
            group.TotalSeats = model.TotalSeats;
            group.Location = model.Location;

            try
            {
                _context.Update(group);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Group updated successfully!";
                return RedirectToAction("ViewCourse", "Course", new { id = model.CourseId });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GroupExists(group.Id))
                {
                    return NotFound();
                }
                throw;
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while updating the group. Please try again.");
                return View(model);
            }
        }

        // POST: Group/RemoveStudent
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveStudent(int groupId, int studentId)
        {
            var booking = await _context.Bookings
                .FirstOrDefaultAsync(b => b.GroupId == groupId && b.StudentId == studentId);

            if (booking == null)
            {
                TempData["ErrorMessage"] = "Student enrollment not found.";
                return RedirectToAction("EditGroup", new { id = groupId });
            }

            var group = await _context.Groups.FindAsync(groupId);
            if (group == null)
            {
                return NotFound();
            }

            try
            {
                _context.Bookings.Remove(booking);

                // Update enrolled students count
                group.EnrolledStudentsCount = Math.Max(0, group.EnrolledStudentsCount - 1);

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Student removed from group successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while removing the student. Please try again.";
            }

            return RedirectToAction("EditGroup", new { id = groupId });
        }

        private bool GroupExists(int id)
        {
            return _context.Groups.Any(e => e.Id == id);
        }


        [HttpPost]
        [ValidateAntiForgeryToken] // Highly recommended for POST requests
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null)
            {
                TempData["ErrorMessage"] = "Course not found.";
                return RedirectToAction("Index", "Teacher"); // Redirect to a suitable list page
            }

            try
            {
                _context.Courses.Remove(course);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Course deleted successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while deleting the course. Please try again.";
                // Log the exception (ex) here for debugging
            }

            return RedirectToAction("Index", "Teacher"); // Redirect after deletion
        }


        // --- GET: Course/CreateGroup
        public async Task<IActionResult> CreateGroup(int courseId, int gradeId)
        {
            var gradeLevel = await _context.GradeLevels
                                         .Include(gl => gl.Course)
                                         .FirstOrDefaultAsync(gl => gl.Id == gradeId);

            if (gradeLevel == null || gradeLevel.CourseId != courseId)
            {
                return NotFound(); // Or redirect to an error page
            }

            ViewBag.CourseId = courseId;
            ViewBag.GradeId = gradeId;
            ViewBag.CourseName = gradeLevel.Course.Name;
            ViewBag.GradeNumber = gradeLevel.NumberOfGrade; // Assuming NumberOfGrade exists on GradeLevel
            ViewBag.MaxStudentsPerGroup = 500; // Example: Set a default or fetch from configuration

            var model = new CreateGroupVM
            {
                CourseId = courseId,
                GradeId = gradeId,
                SelectedDays = new List<string>() // Ensure it's not null on initial load
            };

            return View(model);
        }

        // --- POST: Course/CreateGroup
        [HttpPost]
        [ValidateAntiForgeryToken] // Important for security
        public async Task<IActionResult> CreateGroup(CreateGroupVM model)
        {
            // Re-populate ViewBag properties in case ModelState is invalid and we return the view
            // This ensures context info like CourseName, GradeNumber is displayed again.
            var gradeLevel = await _context.GradeLevels
                                         .Include(gl => gl.Course)
                                         .FirstOrDefaultAsync(gl => gl.Id == model.GradeId);

            if (gradeLevel == null || gradeLevel.CourseId != model.CourseId)
            {
                return NotFound();
            }

            ViewBag.CourseId = model.CourseId;
            ViewBag.GradeId = model.GradeId;
            ViewBag.CourseName = gradeLevel.Course.Name;
            ViewBag.GradeNumber = gradeLevel.NumberOfGrade;
            ViewBag.MaxStudentsPerGroup = 500; // Keep consistent with GET action

            // Server-side validation based on your ViewModel and Model
            if (model.SelectedDays == null || !model.SelectedDays.Any())
            {
                ModelState.AddModelError("SelectedDays", "Please select at least one day for the group schedule.");
            }

            if (model.EndTime <= model.StartTime)
            {
                ModelState.AddModelError("EndTime", "End time must be after start time.");
            }

            if (!ModelState.IsValid)
            {
                // If validation fails, return the view with the model to show errors
                return View(model);
            }

            // Create a new Group entity from the ViewModel
            var group = new Group
            {
                GradeLevelId = model.GradeId,
                Name = model.GroupName,
                SelectedDays = model.SelectedDays,
                StartTime = model.StartTime,
                EndTime = model.EndTime,
                TotalSeats = model.TotalSeats,
                Location = model.Description, // Assuming Description in VM maps to Location in Group model
                EnrolledStudentsCount = 0 // New groups start with 0 enrolled students
            };

            try
            {
                _context.Groups.Add(group);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Group created successfully!";
                // Redirect to the Course details page, or wherever appropriate
                return RedirectToAction("ViewCourse", "Course", new { id = model.CourseId });
            }
            catch (Exception ex)
            {
                // Log the exception (ex)
                ModelState.AddModelError("", "An error occurred while creating the group. Please try again.");
                return View(model);
            }
        }


        // POST: Course/DeleteGroup
        [HttpPost]
        [ValidateAntiForgeryToken] // Crucial for security with POST requests
        public async Task<IActionResult> DeleteGroup(int id, int courseId) // 'id' is GroupId, 'courseId' is for redirection
        {
            var group = await _context.Groups.FindAsync(id);

            if (group == null)
            {
                TempData["ErrorMessage"] = "Group not found.";
                // Redirect back to the course page even if the group wasn't found
                return RedirectToAction("ViewCourse", "Course", new { id = courseId });
            }

            try
            {
                _context.Groups.Remove(group);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Group '{group.Name ?? "Unnamed Group"}' has been successfully deleted.";
                return RedirectToAction("ViewCourse", "Course", new { id = courseId });
            }
            catch (Exception ex)
            {
                // Log the exception (ex) here for debugging purposes
                TempData["ErrorMessage"] = $"An error occurred while deleting group '{group.Name ?? "Unnamed Group"}'. It might have associated records. Please ensure no students are enrolled before deleting.";
                // Redirect back to the same edit page or the course page to show the error
                return RedirectToAction("EditGroup", "Course", new { id = id }); // Stay on current page to show error
            }
        }

    }
}
