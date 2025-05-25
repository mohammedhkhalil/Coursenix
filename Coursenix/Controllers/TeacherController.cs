using System.Security.Cryptography;
using Coursenix.Models;
using Coursenix.Models.ViewModels;
using Coursenix.ViewModels;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Coursenix.Controllers
{
    
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
                Name = teacher.Name,
                Email = teacher.Email,
                PhoneNumber = teacher.PhoneNumber,
                Biography = teacher.Biography,
                ExistingProfilePicture = teacher.ProfilePicture
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
            if (!string.IsNullOrWhiteSpace(model.Name)) teacher.Name = model.Name;
            if (!string.IsNullOrWhiteSpace(model.Email)) teacher.Email = model.Email;
            if (!string.IsNullOrWhiteSpace(model.PhoneNumber)) teacher.PhoneNumber = model.PhoneNumber;
            if (!string.IsNullOrWhiteSpace(model.Biography)) teacher.Biography = model.Biography;

            if (model.ProfilePicture != null)
            {
                //Guid.NewGuid().ToString() -> randomly create name for picture 
                //Path.GetExtension --> get Extension for the photo user upload
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.ProfilePicture.FileName);
                // save the new photo in root
                var filePath = Path.Combine("wwwroot/uploads", fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ProfilePicture.CopyToAsync(stream);
                }
                teacher.ProfilePicture = "/uploads/" + fileName;
            }

            if (!string.IsNullOrEmpty(model.NewPassword))
            {
                var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
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

    }
}
