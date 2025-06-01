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

            var totalCourses = await coursesQuery.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalCourses / PageSize);

            var courses = await coursesQuery
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

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

        [Authorize(Roles = "Teacher")] 
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || !await _userManager.IsInRoleAsync(user, "Teacher"))
            {
                return RedirectToAction("Login", "Account");
            }

            var model = new CreateCourseVM();
            return View("Create", model);
        }

        [Authorize(Roles = "Teacher")] 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCourseVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.AppUserId == user.Id);

            if (teacher == null)
            {
                ModelState.AddModelError("", "Teacher profile not found.");
                return View(model);
            }

            if (model.GradeGroups == null || !model.GradeGroups.Any())
            {
                ModelState.AddModelError("", "Please select at least one grade and add groups.");
                return View(model);
            }

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

            foreach (var gradeGroupsKvp in model.GradeGroups)
            {
                int gradeNumber = gradeGroupsKvp.Key;
                var groups = gradeGroupsKvp.Value;

                if (groups == null || !groups.Any()) continue;

                decimal gradePrice = groups.FirstOrDefault()?.Price ?? 0;

                var gradeLevel = new GradeLevel
                {
                    CourseId = course.Id,
                    NumberOfGrade = gradeNumber,
                    Price = gradePrice
                };

                _context.GradeLevels.Add(gradeLevel);
                await _context.SaveChangesAsync();

                foreach (var groupModel in groups)
                {
                    if (string.IsNullOrEmpty(groupModel.GroupName) ||
                        groupModel.Days == null || !groupModel.Days.Any())
                    {
                        continue; 
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

        private async Task<string> SaveThumbnailAsync(IFormFile thumbnailFile)
        {
            if (thumbnailFile == null || thumbnailFile.Length == 0)
                return null;

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var fileExtension = Path.GetExtension(thumbnailFile.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(fileExtension))
            {
                return null; 
            }

            var fileName = Guid.NewGuid().ToString() + fileExtension;
            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "thumbnails");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await thumbnailFile.CopyToAsync(fileStream);
            }

            return fileName;
        }

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
                    .ThenInclude(gl => gl.Groups) 
                        .ThenInclude(g => g.Bookings) 
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
                Location = course.Location, 
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
            var courseToUpdate = await _context.Courses
                .Include(c => c.GradeLevels)
                    .ThenInclude(gl => gl.Groups) 
                .FirstOrDefaultAsync(c => c.Id == viewModel.Id);

            if (courseToUpdate == null)
            {
                return NotFound();
            }

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

            var gradeLevelsInDb = courseToUpdate.GradeLevels.ToList();
            var gradeLevelsFromForm = viewModel.GradeLevels.Where(gl => !gl.IsDeleted).ToList();

            foreach (var gradeLevelVM in gradeLevelsFromForm)
            {
                if (gradeLevelVM.Id.HasValue && gradeLevelVM.Id.Value != 0) 
                {
                    var existingGl = gradeLevelsInDb.FirstOrDefault(gl => gl.Id == gradeLevelVM.Id.Value);
                    if (existingGl != null)
                    {
                        existingGl.NumberOfGrade = gradeLevelVM.NumberOfGrade;
                        existingGl.Price = gradeLevelVM.Price;
                        _context.Entry(existingGl).State = EntityState.Modified; 
                    }
                }
                else 
                {
                    courseToUpdate.GradeLevels.Add(new GradeLevel
                    {
                        NumberOfGrade = gradeLevelVM.NumberOfGrade,
                        Price = gradeLevelVM.Price,
                        CourseId = courseToUpdate.Id
                    });
                }
            }

            foreach (var dbGradeLevel in gradeLevelsInDb)
            {
                if (!gradeLevelsFromForm.Any(glvm => glvm.Id == dbGradeLevel.Id))
                {
                    if (dbGradeLevel.Groups != null && dbGradeLevel.Groups.Any())
                    {
                        ModelState.AddModelError("", $"Cannot delete Grade Level {dbGradeLevel.NumberOfGrade} as it has associated groups. Please delete its groups first.");
                        viewModel.GradeLevels.Add(new GradeLevelVM
                        {
                            Id = dbGradeLevel.Id,
                            NumberOfGrade = dbGradeLevel.NumberOfGrade,
                            Price = dbGradeLevel.Price,
                            IsDeleted = false 
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
                viewModel.TeacherName = courseToUpdate.Teacher?.Name;
                viewModel.TotalGroups = courseToUpdate.GradeLevels?.SelectMany(gl => gl.Groups).Count() ?? 0;
                viewModel.TotalEnrollments = courseToUpdate.GradeLevels?.SelectMany(gl => gl.Groups).SelectMany(g => g.Bookings).Count() ?? 0;

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
            var groupsInGrade = await _context.Groups
                                              .Where(g => g.GradeLevelId == group.GradeLevelId)
                                              .OrderBy(g => g.Id) 
                                              .ToListAsync();

            int groupIndex = groupsInGrade.FindIndex(g => g.Id == group.Id);

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
                GroupNumberInGrade = groupNumber 

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

            if (model.TotalSeats < group.EnrolledStudentsCount)
            {
                ModelState.AddModelError("TotalSeats",
                    $"Cannot reduce total seats below current enrollments ({group.EnrolledStudentsCount})");
                return View(model);
            }
            var groupsInGrade = await _context.Groups
                                              .Where(g => g.GradeLevelId == model.GradeLevelId)
                                              .OrderBy(g => g.Id) 
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
        [ValidateAntiForgeryToken] 
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null)
            {
                TempData["ErrorMessage"] = "Course not found.";
                return RedirectToAction("Index", "Teacher"); 
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
            }

            return RedirectToAction("Index", "Teacher"); 
        }


        // --- GET: Course/CreateGroup
        public async Task<IActionResult> CreateGroup(int courseId, int gradeId)
        {
            var gradeLevel = await _context.GradeLevels
                                         .Include(gl => gl.Course)
                                         .FirstOrDefaultAsync(gl => gl.Id == gradeId);

            if (gradeLevel == null || gradeLevel.CourseId != courseId)
            {
                return NotFound(); 
            }

            ViewBag.CourseId = courseId;
            ViewBag.GradeId = gradeId;
            ViewBag.CourseName = gradeLevel.Course.Name;
            ViewBag.GradeNumber = gradeLevel.NumberOfGrade; 
            ViewBag.MaxStudentsPerGroup = 500; 

            var model = new CreateGroupVM
            {
                CourseId = courseId,
                GradeId = gradeId,
                SelectedDays = new List<string>() 
            };

            return View(model);
        }

        // --- POST: Course/CreateGroup
        [HttpPost]
        [ValidateAntiForgeryToken] 
        public async Task<IActionResult> CreateGroup(CreateGroupVM model)
        {
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
                return View(model);
            }

            var group = new Group
            {
                GradeLevelId = model.GradeId,
                Name = model.GroupName,
                SelectedDays = model.SelectedDays,
                StartTime = model.StartTime,
                EndTime = model.EndTime,
                TotalSeats = model.TotalSeats,
                Location = model.Description, 
                EnrolledStudentsCount = 0 
            };

            try
            {
                _context.Groups.Add(group);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Group created successfully!";
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
        [ValidateAntiForgeryToken] 
        public async Task<IActionResult> DeleteGroup(int id, int courseId) 
        {
            var group = await _context.Groups.FindAsync(id);

            if (group == null)
            {
                TempData["ErrorMessage"] = "Group not found.";
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
                TempData["ErrorMessage"] = $"An error occurred while deleting group '{group.Name ?? "Unnamed Group"}'. It might have associated records. Please ensure no students are enrolled before deleting.";
                return RedirectToAction("EditGroup", "Course", new { id = id }); // Stay on current page to show error
            }
        }

    }
}
