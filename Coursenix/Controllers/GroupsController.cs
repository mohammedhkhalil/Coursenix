using Coursenix.Models;
using Coursenix.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using Coursenix.Models.ViewModels;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Coursenix.Controllers
{
    public class GroupsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        private readonly Context _context;

        public GroupsController(Context context)
        {
            _context = context;
        }
        public async Task<IActionResult> studentAttendance(int groupId)
        {
            var group = await _context.Groups
                .Include(g => g.Bookings)
                    .ThenInclude(b => b.Student)
                .Include(g => g.Sessions)
                    .ThenInclude(s => s.Attendances)
                .FirstOrDefaultAsync(g => g.Id == groupId);

            if (group == null)
                return NotFound();

            var totalSessions = group.Sessions.Count;
            // list
            var studentAttendances = group.Bookings
                .Select(b =>
                {
                    var student = b.Student;
                    var attendedCount = group.Sessions
                        .Count(s => s.Attendances.Any(a => a.Student.Id == student.Id));

                    var absencePercentage = totalSessions == 0 ? 0 : (1 - (attendedCount / (double)totalSessions)) * 100;

                    return new GroupAttendanceViewModel.StudentAttendance
                    {
                        Id = student.Id,
                        Name = student.Name,
                        PhoneNumber = student.PhoneNumber,
                        ParentPhoneNumber = student.ParentPhoneNumber,
                        AbsencePercentage = Math.Round(absencePercentage, 1)
                    };
                }).ToList();

            var viewModel = new GroupAttendanceViewModel
            {
                GradeLevel = group.Subject.GradeLevel ,
                GroupName = group.Name,
                //Days = group.Days,
                StartTime = group.StartTime,
                EndTime = group.EndTime,
                Students = studentAttendances
            };

            return View(viewModel);
        }
    }
}
