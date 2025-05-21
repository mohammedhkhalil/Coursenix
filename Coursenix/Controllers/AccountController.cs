using Coursenix.Models;
using Coursenix.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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

        // injection
        public AccountController
        (
            UserManager<AppUser> _UserManager,
            SignInManager<AppUser> _signInManager,
            Context _context
        )
        {
            userManager = _UserManager;
            signInManager = _signInManager;
            context = _context;
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
        [ValidateAntiForgeryToken] // VIP: to protect against CSRF attacks --> to not take data from outside domain
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

                // Create account
                AppUser userModel = new AppUser
                {
                    UserName = newUserVM.FullName,
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
                            Grade = newUserVM.Grade.Value,
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
                    }
                    await context.SaveChangesAsync();


                    // Redirect based on role
                    if (await userManager.IsInRoleAsync(userModel, "Student"))
                        return RedirectToAction("Index", "Courses");

                    if (await userManager.IsInRoleAsync(userModel, "Teacher"))
                        return RedirectToAction("Create", "Courses");

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


            var errors = ModelState.Values.SelectMany(v => v.Errors);
            foreach (var error in errors)
            {
                Console.WriteLine(error.ErrorMessage);
            }
            return View(newUserVM.RoleType == "Student" ? "StudentRegister" : "TeacherRegister", newUserVM);
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

            // 4) Build cookie settings — lifetime depends on “Remember Me”.
            var authProps = new AuthenticationProperties
            {
                IsPersistent = userVM.RememberMe,                    // keep the cookie after closing the browser?
                ExpiresUtc = userVM.RememberMe
                               ? DateTimeOffset.UtcNow.AddDays(8)        // 8-day cookie
                               : DateTimeOffset.UtcNow.AddMinutes(30)     // 30-minute cookie
            };

            //await userManager.AddToRoleAsync(userModel, userVM.RoleType);  // add role 

            // 5) Sign the user in (creates the authentication cookie).
            await signInManager.SignInAsync(userModel, authProps);

            // 6) Redirect according to the user’s role.
            if (await userManager.IsInRoleAsync(userModel, "Student"))
                return RedirectToAction("Index", "Courses");

            if (await userManager.IsInRoleAsync(userModel, "Teacher"))
                return RedirectToAction("Create", "Courses");

            // 7) If the user has no recognized role, sign out and show error.
            await signInManager.SignOutAsync();
            ModelState.AddModelError(string.Empty, "Unknown role.");
            return View("Login");
        }


        /************************* Log Out *****************************/
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home"); // Redirect to the home page or login page
        }

    }
}
