using System.Security.Cryptography;
using Coursenix.Models;
using Coursenix.Models.ViewModels;
using Coursenix.ViewModels;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Coursenix.Controllers
{
    public class TeacherController : Controller
    {
        private readonly Context _context;

        public TeacherController(Context context)
        {
            _context = context;
        }

    }
}
