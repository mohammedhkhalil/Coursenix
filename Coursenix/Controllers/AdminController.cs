using Coursenix.Models;
using Coursenix.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Coursenix.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly Context _context;
        public AdminController(UserManager<AppUser> userManager, Context context)
        {
            _userManager = userManager;
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            int Students = _context.Students.Count();
            int Teachers = _context.Teachers.Count();
            ViewData["Students"] = Students;
            ViewData["Teachers"] = Teachers;
            ViewData["Users"] = Students + Teachers;
            return View();
        }

        public async Task<IActionResult> Teachers()
        {
            var teachers = await _context.Teachers
                .Select(t => new AdminManagementVM
                {
                    UserName = t.Name,
                    PP = string.IsNullOrEmpty(t.ProfilePicture) ? "/assets/OIP1.svg" : $"/uploads/thumbnails/{t.ProfilePicture}",
                    UserEmail = t.Email,
                    Status = t.AppUser.LockoutEnd < DateTime.Now || t.AppUser.LockoutEnd == null
                    ? true
                    : false,
                    AppUserId = t.AppUserId
                })
                .ToListAsync();

            return View(teachers);
        }
        public async Task<IActionResult> Students()
        {
            var Students = await _context.Students
                .Select(t => new AdminManagementVM
                {
                    UserName = t.Name,
                    //PP = string.IsNullOrEmpty(t.ProfilePicture) ? "/assets/OIP1.svg" : $"/uploads/thumbnails/{t.ProfilePicture}",
                    UserEmail = t.Email,
                    Status = t.AppUser.LockoutEnd < DateTime.Now || t.AppUser.LockoutEnd==null
                    ? true 
                    : false,
                    AppUserId = t.AppUserId
                })
                .ToListAsync();

            return View(Students);
        }

        [HttpPost]
        public async Task<IActionResult> Block(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            user.LockoutEnd = DateTime.Now.AddYears(100);
           
            await _userManager.UpdateAsync(user);
            if (user.RoleType == "Teacher")
            {
                var Teacher = await _context.Teachers.FirstOrDefaultAsync(i => i.Id.ToString() == userId);
                if (Teacher != null && Teacher.Courses != null)
                {
                    Teacher.Courses.Clear();
                }
                return RedirectToAction("Teachers");

            }
            else if (user.RoleType == "Student")
            {
                var Student = await _context.Students.FirstOrDefaultAsync(i => i.Id.ToString() == userId);
                if (Student != null && Student.Bookings != null)
                    Student.Bookings.Clear();
                if (Student != null && Student.Attendances != null)
                    Student.Attendances.Clear();
                return RedirectToAction("Students");
            }
            return View("Index");
        }
        [HttpPost]
        public async Task<IActionResult> UnBlock(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            user.LockoutEnd = DateTime.Now;

            await _userManager.UpdateAsync(user);
            if (user.RoleType == "Teacher")
            {
                return RedirectToAction("Teachers");
            }
            else if (user.RoleType == "Student")
            {
                return RedirectToAction("Students");
            }
            return View("Index");
        }
    }
}
