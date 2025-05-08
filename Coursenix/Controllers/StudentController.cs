using Coursenix.Models;
using Microsoft.AspNetCore.Mvc;

namespace Coursenix.Controllers
{
    public class StudentController : Controller
    {
        Context context = new Context();
        public IActionResult SignUp()
        {
            return View("SignUp");
        }
        public IActionResult SaveNewStudent(Student student)
        {
            if (ModelState.IsValid) {
                context.Students.Add(student);
                context.SaveChanges();
                return RedirectToAction("Home","Home");
            }
            else
                return View("SignUp");
        }
        public IActionResult Sign_In()
        {
            return View("Sign_In");
        }
    }
}