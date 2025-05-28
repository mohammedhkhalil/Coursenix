using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Coursenix.Models;
using Coursenix.ViewModels;
using Coursenix.Repository;
using System.Security.Claims;

namespace Coursenix.Controllers
{
    [Authorize(Roles = "Teacher")] // Only teachers can create courses
    public class CourseController : Controller
    {
        private readonly Context _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CourseController(Context context, UserManager<AppUser> userManager, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

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
                        StartTime = groupModel.StartTime ?? TimeSpan.FromHours(9), // Default start time
                        EndTime = groupModel.EndTime ?? TimeSpan.FromHours(10), // Default end time
                        TotalSeats = model.StudentsPerGroup,
                        EnrolledStudentsCount = 0,
                        Location = model.Location
                    };

                    _context.Groups.Add(group);
                }
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Course created successfully!";
            return RedirectToAction("Index", "Course");
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
    }
}