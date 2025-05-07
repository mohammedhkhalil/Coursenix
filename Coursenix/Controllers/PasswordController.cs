using Microsoft.AspNetCore.Mvc;

namespace Coursenix.Controllers
{
    public class PasswordController : Controller
    {
        public IActionResult CreatePass()
        {
            return View();
        }

        public IActionResult ForgetPass()
        {
            return View();
        }
    }
}
