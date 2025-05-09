using Coursenix.Models;
using Microsoft.AspNetCore.Mvc;

namespace Coursenix.Controllers
{
    public class StudentController : Controller
    {
        private readonly Context _context;

        public StudentController(Context context)
        {
            _context = context; 
        }
        public IActionResult SignUp()
        {
            return View("SignUp");
        }
        public IActionResult SaveNewStudent(Student student)
        {
            if (ModelState.IsValid) {
                _context.Students.Add(student);
                _context.SaveChanges();
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