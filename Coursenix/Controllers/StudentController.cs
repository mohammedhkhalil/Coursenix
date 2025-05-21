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
        
        // all view of student 

    }
}