using Coursenix.Models;
using Coursenix.Models.ViewModels;
using Coursenix.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Coursenix.Controllers
{
    public class StudentController : Controller
    {
        //private readonly Context _context = new Context()

        // all view of student 
        private readonly Context _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        public StudentController(UserManager<AppUser> userManager, Context context, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _context = context;
            _signInManager = signInManager;
        }

        [HttpGet]
        public async Task<IActionResult> Settings()
        {
            var userId = _userManager.GetUserId(User);

            var student = await _context.Students
                .Include(s => s.AppUser)
                .FirstOrDefaultAsync(s => s.AppUserId == userId);

            if (student == null) return NotFound();

            var vm = new StudentSettingsVM
            {
                Name = student.Name,
                PhoneNumber = student.PhoneNumber,
                ParentPhoneNumber = student.ParentPhoneNumber,
                gradeLevel = student.Grade
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Settings(StudentSettingsVM model)
        {
            var userId = _userManager.GetUserId(User);

            var student = await _context.Students
                .Include(s => s.AppUser)
                .FirstOrDefaultAsync(s => s.AppUserId == userId);

            if (student == null) return NotFound();

            if (!string.IsNullOrWhiteSpace(model.Name))
                student.Name = model.Name;

            if (!string.IsNullOrWhiteSpace(model.PhoneNumber))
                student.PhoneNumber = model.PhoneNumber;

            if (!string.IsNullOrWhiteSpace(model.ParentPhoneNumber))
                student.ParentPhoneNumber = model.ParentPhoneNumber;

            if (model.gradeLevel.HasValue)
                student.Grade = model.gradeLevel.Value ;

            await _userManager.UpdateAsync(student.AppUser);
            _context.Update(student);
            await _context.SaveChangesAsync();

            // Password change 
            if (!string.IsNullOrEmpty(model.CurrPassword) && !string.IsNullOrEmpty(model.NewPassword))
            {
                var result = await _userManager.ChangePasswordAsync(student.AppUser, model.CurrPassword, model.NewPassword);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);
                    return View(model);
                }

                // sgin in 
                await _signInManager.RefreshSignInAsync(student.AppUser);
            }

            return RedirectToAction("Index", "Course");
        }

        public async void DeleteStudent(int StudentId)
        {
            var student = await _context.Students.FindAsync(StudentId);

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();

        }

        //public async Task<IActionResult> Dashboard()
        //{
        //    var appUser = await _userManager.GetUserAsync(User);
        //    if (appUser is null)
        //        return Challenge();   // force login

        //    var student = await _context.Students
        //        .FirstOrDefaultAsync(s => s.AppUserId == appUser.Id);

        //    if (student is null)
        //        return NotFound("Student profile not found.");

        //    // All bookings for this student (=> groups)
        //    var bookings = await _context.Bookings
        //        .Where(b => b.StudentId == student.Id)
        //        .Include(b => b.Group)
        //            .ThenInclude(g => g.Subject)
        //                .ThenInclude(su => su.Teacher)
        //        .Include(b => b.Group.GroupDays)
        //        .ToListAsync();

        //    // Build the view-model list
        //    var viewModel = new List<StudentGroupViewModel>();

        //    foreach (var booking in bookings)
        //    {
        //        var group = booking.Group;

        //        // a) All sessions in this group
        //        var sessionIds = await _context.Sessions
        //            .Where(se => se.GroupId == group.Id)
        //            .Select(se => se.Id)
        //            .ToListAsync();

        //        var totalSessions = sessionIds.Count;

        //        // b) How many the student attended
        //        var sessionsAttended = await _context.Attendances
        //            .CountAsync(a =>
        //                a.StudentId == student.Id &&
        //                a.IsPresent &&
        //                sessionIds.Contains(a.SessionId));

        //        // c) One dashboard card
        //        viewModel.Add(new StudentGroupViewModel
        //        {
        //            StudentId = student.Id,
        //            StudentName = student.Name,

        //            GroupId = group.Id,
        //            GroupName = group.Name,
        //            SubjectName = group.Subject.SubjectName,
        //            TeacherName = group.Subject.Teacher.Name,

        //            Days = group.GroupDays
        //                                .Select(d => d.Day.ToString())
        //                                .ToList(),
        //            StartTime = group.StartTime,
        //            EndTime = group.EndTime,
        //            Location = group.Location,

        //            TotalSessions = totalSessions,
        //            SessionsAttended = sessionsAttended
        //        });
        //    }

        //    return View("Dashboard", viewModel);
        //}

    }


}