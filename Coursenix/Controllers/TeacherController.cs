using System.Security.Cryptography;
using Coursenix.Models;
using Coursenix.Models.ViewModels;
using Coursenix.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Coursenix.Controllers
{
    [Authorize]
    public class TeacherController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly Context _context;
        public TeacherController(UserManager<AppUser> userManager, Context context)
        {
            _userManager = userManager;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Settings()
        {
            var user = await _userManager.GetUserAsync(User);
            var teacher = _context.Teachers.FirstOrDefault(t => t.AppUserId == user.Id);

            if (teacher == null) return NotFound();

            var model = new TeacherSettingsViewModel
            {
                Id = teacher.Id,
                fullName = teacher.Name,
                email = teacher.Email,
                phone = teacher.PhoneNumber,
                bio = teacher.Biography,
            };

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Settings(TeacherSettingsViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            var teacher = _context.Teachers.FirstOrDefault(t => t.AppUserId == user.Id);

            if (teacher == null) return NotFound();

            // Only update if value is not null or empty
            if (!string.IsNullOrWhiteSpace(model.fullName)) teacher.Name = model.fullName;
            if (!string.IsNullOrWhiteSpace(model.phone)) teacher.PhoneNumber = model.phone;
            if (!string.IsNullOrWhiteSpace(model.bio)) teacher.Biography = model.bio;

            //if (model.ProfilePicture != null)
            //{
            //    //Guid.NewGuid().ToString() -> randomly create name for picture 
            //    //Path.GetExtension --> get Extension for the photo user upload
            //    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.ProfilePicture.FileName);
            //    // save the new photo in root
            //    var filePath = Path.Combine("wwwroot/uploads", fileName);
            //    using (var stream = new FileStream(filePath, FileMode.Create))
            //    {
            //        await model.ProfilePicture.CopyToAsync(stream);
            //    }
            //    teacher.ProfilePicture = "/uploads/" + fileName;
            //}

            if (!string.IsNullOrEmpty(model.CurrentPassword) && !string.IsNullOrEmpty(model.password))
            {
                var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.password);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                        ModelState.AddModelError("", error.Description);

                    return View(model);
                }
            }

            _context.SaveChanges();
            ViewBag.Message = "Changes saved successfully!";
            return View(model);
        }

        // ---------- DashBoard ---------
        public async Task<IActionResult> Index() {
            var user = await _userManager.GetUserAsync(User);
            var teacher = await _context.Teachers
                 .Include(t => t.Courses)
                 .ThenInclude(g => g.GradeLevels)
                 .ThenInclude(gl => gl.Groups)
                 .FirstOrDefaultAsync(t => t.AppUserId == user.Id);

            var VM = new TeacherDashboardVM
            {
                TeacherName = teacher.Name,
                PP = string.IsNullOrEmpty(teacher.ProfilePicture) ? "/assets/OIP1.svg" : $"/uploads/thumbnails/{teacher.ProfilePicture}",
                Biography = teacher.Biography,
                TotalCourses = teacher.Courses.Count,
                TotalStudents = teacher.Courses
                .SelectMany(c => c.GradeLevels)
                .SelectMany(gl => gl.Groups)
                 .Sum(g => g.EnrolledStudentsCount),
                TotalGroups = teacher.Courses
                .SelectMany(c => c.GradeLevels)
                .SelectMany(gl => gl.Groups)
                .Select(g => g.Id)
                .Distinct()
                .Count(),
                Courses = teacher.Courses.Select(course => new CourseInfo
                {
                    Id = course.Id,
                    Name = course.Name,
                    Description = course.Description,
                    ThumbnailFileName = string.IsNullOrEmpty(course.ThumbnailFileName)
        ? "/assets/course7.svg"
        : $"/uploads/thumbnails/{course.ThumbnailFileName}",
                    GradeRange = (course.GradeLevels != null && course.GradeLevels.Any())
        ? string.Join(", ", course.GradeLevels
            .Select(gl => gl.NumberOfGrade)
            .Distinct()
            .OrderBy(n => n)
            .Select(n => $"{n}"))
        : string.Empty
                }).ToList()
            };
       
            return View(VM); 
        }
    }
}
