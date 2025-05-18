using Coursenix.Models;
using Coursenix.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Coursenix.Controllers
{
    public class StudentController : Controller
    {
        //private readonly Context _context = new Context();
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

        public IActionResult Settings()
        {
            int? studentId = HttpContext.Session.GetInt32("StudentId");

            if (studentId == null)
                return RedirectToAction("Sign_In");

            var student = _context.Students.FirstOrDefault(s => s.StudentId == studentId);

            if (student == null)
                return NotFound();

            var viewModel = new StudentSettingsViewModel
            {
                Name = student.Name,
                Email = student.Email,
                PhoneNumber = student.PhoneNumber,
                ParentNumber = student.ParentNumber,
                Grade = student.Grade
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Settings(StudentSettingsViewModel model)
        {
            // get cur session for student
            int? studentId = HttpContext.Session.GetInt32("StudentId");

            if (studentId == null)
                return RedirectToAction("Sign_In");

            if (!ModelState.IsValid)
                return View(model);

            var student = _context.Students.FirstOrDefault(s => s.StudentId == studentId);

            if (student == null)
                return NotFound();

            student.Name = model.Name;
            student.Email = model.Email;
            student.PhoneNumber = model.PhoneNumber;
            student.ParentNumber = model.ParentNumber;
            student.Grade = model.Grade;

            if (!string.IsNullOrEmpty(model.NewPassword))
            {
                if (string.IsNullOrEmpty(model.CurrentPassword))
                {
                    ModelState.AddModelError("CurrentPassword", "Please enter current password to change your password.");
                    return View(model);
                }

                var hasher = new PasswordHasher<Student>();
                var result = hasher.VerifyHashedPassword(student, student.Password, model.CurrentPassword);

                if (result == PasswordVerificationResult.Success) // two password match --> in DB vs curpass
                {
                    student.Password = hasher.HashPassword(student, model.NewPassword);
                }
                else
                {
                    ModelState.AddModelError("CurrentPassword", "Current password is incorrect");
                    return View(model);
                }
            }

            _context.SaveChanges();
            ViewBag.Message = "Changes saved successfully";
            return View(model);
        }

    }
}