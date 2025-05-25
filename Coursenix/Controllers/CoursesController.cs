using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Coursenix.Models;
using Microsoft.EntityFrameworkCore;
using Coursenix.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Globalization;
using Coursenix.ViewModels;

namespace Coursenix.Controllers
{
    public class CoursesController : Controller
    {
        private readonly Context _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CoursesController(Context context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: /Courses/Index
        public async Task<IActionResult> Index(string selectedSubject, int? selectedGrade, string searchQuery)
        {
            var allSubjectNames = await _context.Subjects
                                        .Select(s => s.SubjectName)
                                        .Distinct()
                                        .OrderBy(name => name)
                                        .ToListAsync();

            var allGradeLevels = await _context.Subjects
                                      .Select(s => s.GradeLevel)
                                      .Distinct()
                                      .OrderBy(grade => grade)
                                      .ToListAsync();

            var subjectsQuery = _context.Subjects
                                .Include(s => s.Teacher)
                                .AsQueryable();

            if (!string.IsNullOrEmpty(selectedSubject) && selectedSubject != "All subjects")
            {
                subjectsQuery = subjectsQuery.Where(s => s.SubjectName == selectedSubject);
            }

            if (selectedGrade.HasValue && selectedGrade.Value > 0)
            {
                subjectsQuery = subjectsQuery.Where(s => s.GradeLevel == selectedGrade.Value);
            }

            if (!string.IsNullOrEmpty(searchQuery))
            {
                subjectsQuery = subjectsQuery.Where(s => s.Teacher != null && s.Teacher.Name.Contains(searchQuery));
            }

            var subjects = await subjectsQuery.OrderBy(s => s.SubjectName).ToListAsync();

            var viewModel = new CourseListViewModel
            {
                Courses = subjects,
                AvailableSubjects = allSubjectNames,
                AvailableGrades = allGradeLevels,
                SelectedSubject = selectedSubject,
                SelectedGrade = selectedGrade,
                SearchQuery = searchQuery
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subject = await _context.Subjects
                .Include(s => s.Teacher)
                .Include(s => s.Groups)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (subject == null)
            {
                return NotFound();
            }

            var viewModel = new CourseDetailsViewModel
            {
                Subject = subject,
                Groups = subject.Groups?.ToList() ?? new List<Group>(),
                SelectedGroupId = 0,
                RequestSuccessful = false,
                RequestMessage = null
            };

            if (TempData.ContainsKey("StatusMessage"))
            {
                viewModel.RequestMessage = TempData["StatusMessage"] as string;
                viewModel.RequestSuccessful = TempData["IsSuccess"] as bool? ?? false;
            }

            return View("Details", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> Enroll(int subjectId, int selectedGroupId)
        {
            if (subjectId <= 0 || selectedGroupId <= 0)
            {
                TempData["StatusMessage"] = "Invalid course or group selected.";
                TempData["IsSuccess"] = false;
                return RedirectToAction("Details", new { id = subjectId });
            }

            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdString))
            {
                TempData["StatusMessage"] = "You must be logged in to enroll.";
                TempData["IsSuccess"] = false;
                return RedirectToAction("Sign_In", "Account");
            }

            if (!int.TryParse(userIdString, out int studentId))
            {
                TempData["StatusMessage"] = "Error identifying your student profile.";
                TempData["IsSuccess"] = false;
                return RedirectToAction("Details", new { id = subjectId });
            }

            var group = await _context.Groups
                                  .Include(g => g.Subject)
                                  .FirstOrDefaultAsync(g => g.Id == selectedGroupId);

            var student = await _context.Students.FirstOrDefaultAsync(s => s.Id == studentId);

            if (group == null || student == null || group.SubjectId != subjectId)
            {
                TempData["StatusMessage"] = "Invalid group or student data.";
                TempData["IsSuccess"] = false;
                return RedirectToAction("Details", new { id = subjectId });
            }

            if (group.EnrolledStudentsCount >= group.TotalSeats)
            {
                TempData["StatusMessage"] = "Selected group is already full.";
                TempData["IsSuccess"] = false;
                return RedirectToAction("Details", new { id = subjectId });
            }

            try
            {
                group.EnrolledStudentsCount++;
                await _context.SaveChangesAsync();

                TempData["StatusMessage"] = "Enrollment request submitted successfully!";
                TempData["IsSuccess"] = true;
                return RedirectToAction("EnrollmentSuccess", "Courses");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred during enrollment.");
                TempData["StatusMessage"] = "An error occurred during enrollment. Please try again.";
                TempData["IsSuccess"] = false;
                return RedirectToAction("Details", new { id = subjectId });
            }
        }

        public IActionResult EnrollmentSuccess()
        {
            return View();
        }

        [HttpGet]
        // GET: /Courses/Create
        public IActionResult Create()
        {
            return View(new CreateCourseViewModel()); // Pass a new ViewModel to the view
        }

        // POST: /Courses/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        // [Authorize(Roles = "Teacher")] // Keep commented for testing without login
        public async Task<IActionResult> Create(CreateCourseViewModel model)
        {
            // Bypass teacher email check for testing (as discussed previously)
            // var teacherEmail = User.FindFirstValue(ClaimTypes.Email);
            // if (string.IsNullOrEmpty(teacherEmail))
            // {
            //     model.StatusMessage = "You must be logged in as a teacher to create a course.";
            //     model.IsSuccess = false;
            //     return View(model);
            // }

            // Bypass fetching teacher by email for testing (as discussed previously)
            // var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.Email == teacherEmail);
            // if (teacher == null)
            // {
            //     model.StatusMessage = "Teacher profile not found. Cannot create course.";
            //     model.IsSuccess = false;
            //     return View(model);
            // }

            int defaultTeacherId = 1;
            if (ModelState.IsValid)
            {
                string thumbnailFileName = null;
                if (model.ThumbnailFile != null && model.ThumbnailFile.Length > 0)
                {
                    if (ModelState.ContainsKey("ThumbnailFile") && ModelState["ThumbnailFile"]?.Errors.Count > 0)
                    {
                        model.StatusMessage = model.StatusMessage ?? "Please fix the errors related to the thumbnail file in the form."; // More specific message
                        model.IsSuccess = false;
                        return View(model);
                    }

                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "thumbnails");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    thumbnailFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(model.ThumbnailFile.FileName);
                    string filePath = Path.Combine(uploadsFolder, thumbnailFileName);

                    try
                    {
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await model.ThumbnailFile.CopyToAsync(fileStream);
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log the exception details to the console for debugging
                        Console.Error.WriteLine($"File Upload Error: {ex.Message}");
                        if (ex.InnerException != null)
                        {
                            Console.Error.WriteLine($"Inner File Upload Error: {ex.InnerException.Message}");
                        }
                        ModelState.AddModelError("ThumbnailFile", "Error uploading thumbnail image. Please try again.");
                        model.StatusMessage = "Error uploading thumbnail image.";
                        model.IsSuccess = false;
                        return View(model);
                    }
                }


                var subject = new Subject
                {
                    SubjectName = model.CourseTitle,
                    Price = model.CoursePrice,
                    Description = model.CourseDescription,
                    GradeLevel = model.CourseGradeLevel,
                    Location = model.Location,
                    StudentsPerGroup = model.StudentsPerGroup,
                    TeacherId = defaultTeacherId, // Use the hardcoded default ID
                    ThumbnailFileName = thumbnailFileName

                };

                try
                {
                    _context.Subjects.Add(subject);
                    await _context.SaveChangesAsync();
                    // Redirect to the AddGroups action on success, passing the new SubjectId
                    return RedirectToAction("AddGroups", new { subjectId = subject.Id });
                }
                catch (Exception ex) // Catching the database save exception
                {
                    // *** EDITED CATCH BLOCK: Include exception details in the StatusMessage ***
                    // Log the exception details to the console for debugging
                    Console.Error.WriteLine($"Database Save Error: {ex.Message}");
                    if (ex.InnerException != null)
                    {
                        Console.Error.WriteLine($"Inner Database Save Error: {ex.InnerException.Message}");
                    }

                    ModelState.AddModelError("", "An error occurred while saving basic course details.");

                    // Include the specific exception message(s) in the StatusMessage shown to the user
                    model.StatusMessage = $"An error occurred while saving basic course details: {ex.Message}";
                    if (ex.InnerException != null)
                    {
                        model.StatusMessage += $" Details: {ex.InnerException.Message}";
                    }

                    model.IsSuccess = false;

                    // Clean up uploaded file if database save fails
                    if (!string.IsNullOrEmpty(thumbnailFileName))
                    {
                        string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "thumbnails", thumbnailFileName);
                        if (System.IO.File.Exists(filePath))
                        {
                            try { System.IO.File.Delete(filePath); }
                            catch (Exception cleanupEx) { /* Log cleanup error if necessary */ }
                        }
                    }

                    return View(model);
                }
            }

            // If ModelState was not valid initially
            model.StatusMessage = model.StatusMessage ?? "Please fix the errors in the form."; // Ensure a message is set
            model.IsSuccess = false;
            return View(model);
        }

        [HttpGet]
        //[Authorize(Roles = "Teacher")]
        public async Task<IActionResult> AddGroups(int? subjectId)
        {
            int testSubjectId = 1;
            int idToUse = subjectId ?? testSubjectId;


            if (idToUse <= 0) 
            {
                return RedirectToAction("Create");
            }

            var subject = await _context.Subjects
                                        .Include(s => s.Groups)
                                        .FirstOrDefaultAsync(s => s.Id == idToUse); // Use idToUse

            if (subject == null)
            {
                TempData["StatusMessage"] = "Course not found. Cannot add groups.";
                TempData["IsSuccess"] = false;
                return RedirectToAction("Create");
            }

            var viewModel = new AddGroupsViewModel 
            {
                SubjectId = subject.Id, 
                Subject = subject,             
                Groups = subject.Groups?       
                                .Select(g => new GroupViewModel 
                                {
                                    Grade = g.Grade,          
                                    GroupName = g.Name,   
                                    Days = g.DayOfWeek,       
                                    StartTime = g.StartTime.ToString("HH:mm", CultureInfo.InvariantCulture), 
                                    EndTime = g.EndTime.ToString("HH:mm", CultureInfo.InvariantCulture)   
                                })
                                .ToList() // Converting to List<GroupViewModel>
                                ?? new List<GroupViewModel>() // Providing default empty list if needed
            };

            if (TempData.ContainsKey("StatusMessage")) 
            {
                viewModel.StatusMessage = TempData["StatusMessage"] as string; 
                viewModel.IsSuccess = TempData["IsSuccess"] as bool? ?? false; 
            }

            return View(viewModel); 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "Teacher")] 
        public async Task<IActionResult> AddGroups(AddGroupsViewModel model) 
        {
            var subject = await _context.Subjects.FirstOrDefaultAsync(s => s.Id == model.SubjectId); 

            if (subject == null)
            {
                TempData["StatusMessage"] = "Course not found. Cannot save groups.";
                TempData["IsSuccess"] = false;
                return RedirectToAction("Create");
            }

            if (ModelState.ContainsKey("StatusMessage"))
            {
                ModelState.Remove("StatusMessage");
            }
            if (ModelState.ContainsKey("Subject")) 
            {
                ModelState.Remove("Subject");
            }
            if (ModelState.ContainsKey("IsSuccess")) 
            {
                ModelState.Remove("IsSuccess");
            }


            bool allGroupsValid = ModelState.IsValid; 

            if (model.Groups == null || !model.Groups.Any())
            {
              
                if (allGroupsValid) 
                {
                    ModelState.AddModelError("Groups", "At least one group must be added.");
                    allGroupsValid = false;
                }
                else
                {
                    // If other errors exist, the form won't submit anyway, so we can rely on those.
                    // We still need to know that the group list is empty for later processing if needed,
                    // but the main validation summary will show other errors.
                }
            }


            if (model.Groups != null && model.Groups.Any()) // Re-check after the "At least one group" check
            {
                for (int i = 0; i < model.Groups.Count; i++) // Iterating over model.Groups
                {
                    var groupModel = model.Groups[i]; 

                    DateTime startTime = default;
                    DateTime endTime = default;

                    bool startTimeParsed = false;
                    bool endTimeParsed = false;

                    if (!string.IsNullOrEmpty(groupModel.StartTime))
                    {
                        if (DateTime.TryParseExact(groupModel.StartTime, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out startTime))
                        {
                            startTimeParsed = true;
                        }
                        else if (DateTime.TryParseExact(groupModel.StartTime, "h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out startTime)) // Try AM/PM format
                        {
                            startTimeParsed = true;
                        }
                    }

                    if (!string.IsNullOrEmpty(groupModel.EndTime))
                    {
                        if (DateTime.TryParseExact(groupModel.EndTime, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out endTime))
                        {
                            endTimeParsed = true;
                        }
                        else if (DateTime.TryParseExact(groupModel.EndTime, "h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out endTime)) // Try AM/PM format
                        {
                            endTimeParsed = true;
                        }
                    }

                    if (!string.IsNullOrEmpty(groupModel.StartTime) && !string.IsNullOrEmpty(groupModel.EndTime))
                    {
                        if (!startTimeParsed || !endTimeParsed)
                        {
                            if (!startTimeParsed) ModelState.AddModelError($"Groups[{i}].StartTime", "Invalid time format. Use HH:mm (24-hour) or h:mm AM/PM."); // More specific message
                            if (!endTimeParsed) ModelState.AddModelError($"Groups[{i}].EndTime", "Invalid time format. Use HH:mm (24-hour) or h:mm AM/PM."); // More specific message
                            allGroupsValid = false;
                        }
                        else 
                        {
                            if (endTime <= startTime)
                            {
                                ModelState.AddModelError($"Groups[{i}].EndTime", "End time must be after start time for this group.");
                                allGroupsValid = false;
                            }
                        }
                    }
                    else if (!string.IsNullOrEmpty(groupModel.StartTime) || !string.IsNullOrEmpty(groupModel.EndTime))
                    {
                        if (string.IsNullOrEmpty(groupModel.StartTime)) ModelState.AddModelError($"Groups[{i}].StartTime", "Start time is required if end time is provided.");
                        if (string.IsNullOrEmpty(groupModel.EndTime)) ModelState.AddModelError($"Groups[{i}].EndTime", "End time is required if start time is provided.");
                        allGroupsValid = false;
                    }


                    if (groupModel.Grade <= 0 || groupModel.Grade > 12) // Added > 12 check for completeness
                    {
                        ModelState.AddModelError($"Groups[{i}].Grade", "Grade is required and must be between 1 and 12 for this group.");
                        allGroupsValid = false;
                    }

                    if (string.IsNullOrEmpty(groupModel.Days))
                    {
                        ModelState.AddModelError($"Groups[{i}].Days", "Days are required for this group.");
                        allGroupsValid = false;
                    }

                    if (string.IsNullOrEmpty(groupModel.GroupName))
                    { // Accessing groupModel.GroupName
                        ModelState.AddModelError($"Groups[{i}].GroupName", "Group name is required.");
                        allGroupsValid = false;
                    }
                }
            }


            if (ModelState.IsValid && allGroupsValid) 
            {
                try
                {
                    var groupsToSave = new List<Group>();
                    foreach (var groupModel in model.Groups) 
                    {
                        DateTime startTime = DateTime.ParseExact(groupModel.StartTime, groupModel.StartTime.Contains("AM", StringComparison.OrdinalIgnoreCase) || groupModel.StartTime.Contains("PM", StringComparison.OrdinalIgnoreCase) ? "h:mm tt" : "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None); // Parse based on detected format
                        DateTime endTime = DateTime.ParseExact(groupModel.EndTime, groupModel.EndTime.Contains("AM", StringComparison.OrdinalIgnoreCase) || groupModel.EndTime.Contains("PM", StringComparison.OrdinalIgnoreCase) ? "h:mm tt" : "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None);     // Parse based on detected format

                        var group = new Group 
                        {
                            SubjectId = subject.Id, 
                            Grade = groupModel.Grade, 
                            Name = groupModel.GroupName, 
                            DayOfWeek = groupModel.Days, 
                            StartTime = startTime, 
                            EndTime = endTime,     
                            TotalSeats = subject.StudentsPerGroup, 
                            EnrolledStudentsCount = 0,
                            Location = subject.Location ,
                            //Teacherid = Teacher.TeacherId
                        };
                        groupsToSave.Add(group);
                    }

                    _context.Groups.AddRange(groupsToSave);
                    await _context.SaveChangesAsync();

                    TempData["StatusMessage"] = "Groups added successfully and course is complete!"; 
                    TempData["IsSuccess"] = true;
                    return RedirectToAction("Details", new { id = subject.Id }); 
                }
                catch (Exception ex)
                {
                    model.Subject = await _context.Subjects.FirstOrDefaultAsync(s => s.Id == model.SubjectId);

                    ModelState.AddModelError("", "An error occurred while saving the groups.");
                    model.StatusMessage = "An error occurred while saving the groups."; 
                    model.IsSuccess = false;

                    return View(model); 
                }
            }
            else
            {
                model.Subject = await _context.Subjects.FirstOrDefaultAsync(s => s.Id == model.SubjectId);

                model.StatusMessage = model.StatusMessage ?? "Please fix the errors in the form."; 
                model.IsSuccess = false; 
                return View(model); 
            }
        }
    }
}
