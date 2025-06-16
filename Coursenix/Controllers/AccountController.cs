using Coursenix.Models;
using Coursenix.Repository;
using Coursenix.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NuGet.DependencyResolver;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Coursenix.Controllers
{
    //public enum UserType
    //{
    //    Student, 
    //    Teacher,
    //    Admin
    //}
    public class AccountController : Controller
    {
        private readonly Context context;
        private readonly UserManager<AppUser> userManager;
        private readonly SignInManager<AppUser> signInManager;
        private readonly EmailService emailService;


        // injection
        public AccountController
        (
            UserManager<AppUser> _UserManager,
            SignInManager<AppUser> _signInManager,
            Context _context,
            EmailService _emailService
        )
        {
            userManager = _UserManager;
            signInManager = _signInManager;
            context = _context;
            emailService = _emailService;
        }

        /************************* Register *****************************/
        [HttpGet]
        public IActionResult StudentRegister() // it does not take an object only for open view
        {
            return View("StudentRegister");
        }
        [HttpGet]
        public IActionResult TeacherRegister() // it does not take an object only for open view
        {
            return View("TeacherRegister");
        }

        [HttpPost]
        [ValidateAntiForgeryToken] 
        public async Task<IActionResult> Register(RegisterUserViewModel newUserVM) // Make this async
        {
            if (ModelState.IsValid)
            {
                // Check if email already exists
                var existingUser = await userManager.FindByEmailAsync(newUserVM.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Email", "This email is already registered.");
                    return View(newUserVM.RoleType == "Student" ? "StudentRegister" : "TeacherRegister", newUserVM);
                }

                var code = new Random().Next(100000, 999999).ToString();

                var ResetCode = new ResetCode
                {
                    Email = newUserVM.Email,
                    Code = code,
                    Expiry = DateTime.UtcNow.AddMinutes(10),
                    Purpose = "Register" 
                };

                context.ResetCodes.Add(ResetCode);
                await context.SaveChangesAsync();

                await emailService.SendEmailAsync(newUserVM.Email, "Email Confirmation", $"Your code is: {code}");
                // save mail in session to pass it for "CheckYourEmail"
                HttpContext.Session.SetString("VerifyEmail", newUserVM.Email);
                // save new user to complete register 
                HttpContext.Session.SetString("RegisterUserData", JsonConvert.SerializeObject(newUserVM));
                return RedirectToAction("CheckYourEmail" ,new { Purpose = "Register" });
            }
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            foreach (var error in errors)
            {
                Console.WriteLine(error.ErrorMessage);
            }
            return View(newUserVM.RoleType == "Student" ? "StudentRegister" : "TeacherRegister", newUserVM);
        }

        public async Task<IActionResult> CompleteRegistration()
        {
            var temp = HttpContext.Session.GetString("RegisterUserData");
            if (string.IsNullOrEmpty(temp)) 
            {
                return RedirectToAction("Index", "Home");
            }
            var newUserVM = JsonConvert.DeserializeObject<RegisterUserViewModel>(temp); 
            // Create account
            AppUser userModel = new AppUser
            {
                UserName = newUserVM.Email,
                Name = newUserVM.FullName,
                Email = newUserVM.Email,
                PasswordHash = newUserVM.Password, // Password is handled by IdentityUser
                RoleType = newUserVM.RoleType
            };

            // var result = await userManager.CreateAsync(userModel);  not hash password
            var result = await userManager.CreateAsync(userModel, newUserVM.Password); // hash password

            if (result.Succeeded)
            {
                // Assign role to user.
                await userManager.AddToRoleAsync(userModel, newUserVM.RoleType);

                //add cliams 
                await userManager.AddClaimsAsync(userModel, new List<Claim>
                    {
                        new Claim("FullName", userModel.Name),
                        new Claim("Email", userModel.Email),
                    });

                // create a cookie for the user
                await signInManager.SignInAsync(userModel, isPersistent: false);

                // Add user to the appropriate table based on role
                if (newUserVM.RoleType == "Student")
                {
                    var student = new Student
                    {
                        AppUserId = userModel.Id,
                        Name = newUserVM.FullName,
                        Email = newUserVM.Email,
                        PhoneNumber = newUserVM.PhoneNumber,
                        ParentPhoneNumber = newUserVM.ParentNumber
                    };
                    context.Students.Add(student);
                }
                else if (newUserVM.RoleType == "Teacher")
                {
                    var teacher = new Teacher
                    {
                        AppUserId = userModel.Id,
                        Name = newUserVM.FullName,
                        Email = newUserVM.Email,
                        PhoneNumber = newUserVM.PhoneNumber,
                        Biography = newUserVM.Biography,
                    };
                    context.Teachers.Add(teacher);
                    await userManager.AddClaimsAsync(userModel, new List<Claim>
                    {
                        new Claim("FullName", userModel.Name),
                        new Claim("Email", userModel.Email),
                        new Claim("TeachID", teacher.Id.ToString())
                    });
                }
                await context.SaveChangesAsync();

                HttpContext.Session.Remove("VerifyEmail");
                HttpContext.Session.Remove("RegisterUserData");


                // Redirect based on role
                if (await userManager.IsInRoleAsync(userModel, "Student"))
                    return RedirectToAction("Index", "Home");

                if (await userManager.IsInRoleAsync(userModel, "Teacher"))
                    return RedirectToAction("Index", "Home");

                return RedirectToAction("Index", "Home");
            }
            else
            {
                // Add errors to ModelState
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(newUserVM.RoleType == "Student" ? "StudentRegister" : "TeacherRegister", newUserVM);
            }
        }

        /************************* Log IN *****************************/
        [HttpGet]
        public IActionResult Login() // open view
        {
            return View("Login", new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel userVM)
        {
            // 1) Basic model validation 
            if (!ModelState.IsValid)
                return View(userVM);

            // 2) Find the user by e-mail address; return error if not found.
            var userModel = await userManager.FindByEmailAsync(userVM.Email);
            if (userModel is null)
            {
                ModelState.AddModelError(string.Empty, "User does not exist.");
                return View(userVM);
            }

            // 3) Verify the password in a secure way (hash + salt handled by Identity).
            var passResult = await signInManager.CheckPasswordSignInAsync(
                userModel,                               // user object
                userVM.Password,                 // plain-text password from the form
                lockoutOnFailure: false);             // disable automatic lockout for this demo

            if (!passResult.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Invalid password.");
                return View(userVM);
            }

            // 4) check Role 
            var userRoles = await userManager.GetRolesAsync(userModel);
            if (!userRoles.Contains(userVM.Role))
            {
                ModelState.AddModelError(string.Empty, $"You are not registered as a {userVM.Role}.");
                return View(userVM);
            }

            // 5) Build cookie settings — lifetime depends on “Remember Me”.
            var authProps = new AuthenticationProperties
            {
                IsPersistent = userVM.RememberMe,                    // keep the cookie after closing the browser?
                ExpiresUtc = userVM.RememberMe
                               ? DateTimeOffset.UtcNow.AddDays(8)        // 8-day cookie
                               : DateTimeOffset.UtcNow.AddMinutes(30)     // 30-minute cookie
            };

          
            //-----------------///// if blocked /////--------------
            //if (userModel.LockoutEnd <= DateTime.Now)
            //{
            //    return RedirectToAction("Logout");
            //}

            //await userManager.AddToRoleAsync(userModel, userVM.RoleType);  // add role 
            // 6) Sign the user in (creates the authentication cookie & cliams).
            await signInManager.SignInWithClaimsAsync(
                 userModel,
                 authProps,
                 new List<Claim>
                 {
                    new Claim("FullName", userModel.Name),
                    new Claim("Email", userModel.Email)
                 }
            );


            // 7) Redirect according to the user’s role.
            if (await userManager.IsInRoleAsync(userModel, "Student"))
                return RedirectToAction("Index", "Course");

            if (await userManager.IsInRoleAsync(userModel, "Teacher"))
                return RedirectToAction("Index", "Teacher");

            if (await userManager.IsInRoleAsync(userModel, "Admin"))
                return RedirectToAction("Index", "Admin");

            // 8) If the user has no recognized role, sign out and show error.
            await signInManager.SignOutAsync();
            ModelState.AddModelError(string.Empty, "Unknown role.");
            return View("Login");
        }


        /************************* Log Out *****************************/
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }


        /************************* Forgot Pass *****************************/

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return View();
            }

            var code = new Random().Next(100000, 999999).ToString();

            var ResetCode = new ResetCode
            {
                Email = model.Email,
                Code = code,
                Expiry = DateTime.UtcNow.AddMinutes(10) ,
                Purpose = "ResetPassword"
            };

            context.ResetCodes.Add(ResetCode);
            await context.SaveChangesAsync();

            await emailService.SendEmailAsync(model.Email, "Password Reset Code", $"Your code is: {code}");
            // save mail in session to pass it for "CheckYourEmail"
            HttpContext.Session.SetString("VerifyEmail", model.Email);

            return RedirectToAction("CheckYourEmail", new { Purpose = "ResetPassword" });
        }

        [HttpPost]
        public async Task<IActionResult> ResendCode(string Purpose)
        {
            /*
             catch curr mail -> delet resetcode -> make new resetcode -> sent it -> verify (check you mail)
             */
            var email = HttpContext.Session.GetString("VerifyEmail");

            if (string.IsNullOrEmpty(email))
            {
                if (Purpose == "ResetPassword")
                    return RedirectToAction("ForgotPassword");
                else
                    return RedirectToAction("Index", "Home");
            }

            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                if (Purpose == "ResetPassword")
                    return RedirectToAction("ForgotPassword");
                else
                    return RedirectToAction("Index", "Home");
            }

            var oldCodes = context.ResetCodes.Where(c => c.Email == email && c.Purpose == Purpose);
            context.ResetCodes.RemoveRange(oldCodes);

            var code = new Random().Next(100000, 999999).ToString();

            var ResetCode = new ResetCode
            {
                Email = email,
                Code = code,
                Expiry = DateTime.UtcNow.AddMinutes(10),
                Purpose = Purpose
            };

            context.ResetCodes.Add(ResetCode);
            await context.SaveChangesAsync();

            await emailService.SendEmailAsync(email, "Password Reset Code", $"Your new code is: {code}");

            TempData["Message"] = "A new code has been sent to your email.";

            return RedirectToAction("CheckYourEmail", new { Purpose });
        }

        [HttpGet]
        public IActionResult CheckYourEmail(string Purpose)
        {
            VerifyCodeViewModel model = new VerifyCodeViewModel() { Purpose = Purpose } ;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckYourEmail(VerifyCodeViewModel model)
        {
            var email = HttpContext.Session.GetString("VerifyEmail");
            var Purpose = model.Purpose;
            if (string.IsNullOrEmpty(email))
            {
                if (Purpose == "ResetPassword")
                    return RedirectToAction("ForgotPassword");
                else
                    return RedirectToAction("Index","Home"); 
            }

            var enteredCode = model.Digit1 + model.Digit2 + model.Digit3 + model.Digit4 + model.Digit5 + model.Digit6;

            var codeEntry = await context.ResetCodes
                .FirstOrDefaultAsync(c => c.Email == email && c.Code == enteredCode && c.Purpose == Purpose);

            if (codeEntry == null || codeEntry.Expiry < DateTime.UtcNow)
            {
                ModelState.AddModelError("", "Invalid or expired code");
                return View(model);
            }

            context.ResetCodes.Remove(codeEntry);
            await context.SaveChangesAsync();

            
            if (Purpose == "ResetPassword")
            {
                return RedirectToAction("CreateNewPassword");
            }
            else if (Purpose == "Register")
            {
                return RedirectToAction("CompleteRegistration"); 
            }

            return View(model); 
        }


        [HttpGet]
        public IActionResult CreateNewPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateNewPassword(CreateNewPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            // get mail in the curr session
            var email = HttpContext.Session.GetString("VerifyEmail");
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("ForgotPassword");
            }

            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return RedirectToAction("ForgotPassword");
            }

            var token = await userManager.GeneratePasswordResetTokenAsync(user);

            var result = await userManager.ResetPasswordAsync(user, token, model.Password);

            if (result.Succeeded)
            {
                var codesToRemove = context.ResetCodes.Where(c => c.Email == email);
                context.ResetCodes.RemoveRange(codesToRemove);
                await context.SaveChangesAsync();

                // remove mail form session
                HttpContext.Session.Remove("VerifyEmail");

                return RedirectToAction("PasswordResetSuccessful");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(model);
        }

        [HttpGet]
        public IActionResult PasswordResetSuccessful()
        {
            return View();
        }

    }
}