using Microsoft.AspNetCore.Mvc;

namespace Coursenix.Controllers
{
    public class TeacherController : Controller
    {
        public IActionResult SignUp()
        {
            return View("SignUp");
        }

        public IActionResult Sign_In()
        {
            return View("Sign_In");
        }
    }
}
