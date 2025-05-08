using System.Security.Cryptography;
using Coursenix.Models;
using Coursenix.Models.ViewModels;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Coursenix.Controllers
{
    public class TeacherController : Controller
    {
        private readonly CoursenixContext _context;

        public TeacherController(CoursenixContext context)
        {
            _context = context;
        }
        public IActionResult SignUp()
        {
            return View(new TeacherSignUpViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUp(TeacherSignUpViewModel model)
        {
            if (ModelState.IsValid)
            {
                var existingTeacher = await _context.Teachers
                    .FirstOrDefaultAsync(t => t.Email == model.Email);

                if (existingTeacher != null)
                {
                    ModelState.AddModelError("Email", "Email address is already registered.");
                    return View(model); 
                }


                // 2. **التعامل اليدوي مع كلمة المرور: التشفير (Hashing)**
                // هذا أمر بالغ الأهمية للأمان. نقوم بتوليد Salt وتشفير كلمة المرور باستخدام PBKDF2.

                // Generate a 128-bit (16 byte) salt
                byte[] salt = new byte[128 / 8];
                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(salt);
                }

                // Hash the password using PBKDF2
                // We use SHA256 as the pseudorandom function, 10000 iterations, 256-bit (32 byte) hash size
                byte[] hashedPasswordBytes = KeyDerivation.Pbkdf2(
                    password: model.Password,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 10000, // يفضل استخدام عدد تكرارات عالي (10000 فما فوق)
                    numBytesRequested: 256 / 8); // حجم الـ Hash المطلوب (32 بايت = 256 بت)

                string hashedPassword = Convert.ToBase64String(hashedPasswordBytes);
                string saltString = Convert.ToBase64String(salt);


                var teacher = new Teacher // تأكد أن Teacher.cs يحتوي الآن على Biography و PasswordSalt
                {
                    Email = model.Email,
                    Password = hashedPassword,
                    FirstName = model.FullName, 
                    LastName = model.FullName,
                    PhoneNumber = model.PhoneNumber, // رقم الهاتف

                };

                _context.Teachers.Add(teacher);
                await _context.SaveChangesAsync(); // حفظ كيان Teacher في قاعدة البيانات

                return RedirectToAction("SignUpConfirmation", "Teacher"); 
            }

            return View(model);
        }

        

        public IActionResult Sign_In()
        {
            return View("Sign_In");
        }
    }
}
