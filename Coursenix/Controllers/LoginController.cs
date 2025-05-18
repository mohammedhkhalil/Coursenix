using Coursenix.Models;
using Coursenix.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace Coursenix.Controllers
{
    public class LoginController : Controller
    {
        private readonly Context context = new Context();
        [HttpGet]
        public IActionResult Login()
        {
            return View("Login");
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("Home", "Home");

            if (model.Role == "Student")
            {
                var student = context.Students
                    .FirstOrDefault(s => s.Email == model.Email && s.Password == model.Password);
                if (student != null)
                    return RedirectToAction("Index", "Courses");
            }
            else if (model.Role == "Teacher")
            {
                var teacher = context.Students
                    .FirstOrDefault(s => s.Email == model.Email && s.Password == model.Password);
                if (teacher != null)
                    return RedirectToAction("Create", "Courses");
            }

            //ModelState.AddModelError("", "Invalid login attempt.");
            return RedirectToAction("Home", "Home");
        }
    }
}
