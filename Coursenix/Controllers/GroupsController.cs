using Coursenix.Models;
using Coursenix.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using Coursenix.Models.ViewModels;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Coursenix.Enums;

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
                    // for each el
                    var student = b.Student;
                    var attendedCount = group.Sessions // like we iterate over all session list and count how many session that sutdent attentd
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
                GradeLevel = group.GradeLevel.NumberOfGrade,
                GroupName = group.Name,
                Days = group.SelectedDays,
                StartTime = group.StartTime,
                EndTime = group.EndTime,
                Students = studentAttendances
            };

            return View(viewModel);
        }

        //SessionAttendance
        // GET: Attendance/TakeAttendance?sessionId=5
        [HttpGet]
        public IActionResult TakeAttendance(int sessionId, string? searchTerm)
        {
            var session = _context.Sessions
                .Include(s => s.Group)
                .FirstOrDefault(s => s.Id == sessionId);

            if (session == null)
            {
                return NotFound();
            }

            // Get all students booked in this group
            var studentBookings = _context.Bookings
                .Where(b => b.GroupId == session.GroupId)
                .Include(b => b.Student)
                .ToList();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                studentBookings = studentBookings
                    .Where(b => b.Student.Name.Contains(searchTerm))
                    .ToList();
            }

            var viewModel = new TakeAttendanceViewModel
            {
                SessionId = session.Id,
                SessionDateTime = session.SessionDateTime,
                GStartTime = session.Group.StartTime,
                GEndTime = session.Group.EndTime,
                Days = session.Group.SelectedDays,
                GroupId = session.GroupId,
                GroupName = session.Group.Name ?? "Group",
                Students = studentBookings.Select(b => new StudentAttendanceVM
                {
                    StudentId = b.Student.Id,
                    StudentFullName = b.Student.Name,
                    IsPresent = false
                }).ToList()
            };

            return View(viewModel);
        }

        // POST: Attendance/TakeAttendance
        [HttpPost]
        public IActionResult TakeAttendance(TakeAttendanceViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Get existing attendance records for the session
            var existingAttendance = _context.Attendances
                .Where(a => a.SessionId == model.SessionId)
                .Select(a => a.StudentId)
                .ToHashSet();

            foreach (var student in model.Students)
            {
                if (existingAttendance.Contains(student.StudentId))
                    continue; // Skip if already recorded

                var attendance = new Attendance
                {
                    SessionId = model.SessionId,
                    StudentId = student.StudentId,
                    IsPresent = student.IsPresent
                };

                _context.Attendances.Add(attendance);
            }

            _context.SaveChanges();

            TempData["Success"] = "Attendance saved successfully!";
            return RedirectToAction("TakeAttendance");
        }
    }
}