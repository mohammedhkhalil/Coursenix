using System.Security.Claims;
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
        public IActionResult Dashboard()
        {
            // Get the logged-in user's ID from Identity
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            // Find the student record
            var student = _context.Students
                .FirstOrDefault(s => s.AppUserId == userId);
            if (student == null)
            {
                return NotFound("Student profile not found.");
            }

            // Fetch bookings with related data
            var bookings = _context.Bookings
                .Include(b => b.Group)
                    .ThenInclude(g => g.GradeLevel)
                        .ThenInclude(gl => gl.Course)
                            .ThenInclude(c => c.Teacher)
                .Where(b => b.StudentId == student.Id)
                .ToList();

            // Prepare view model
            var viewModel = new StudentDashboardViewModel
            {
                StudentName = student.Name,
                Courses = new List<StudentDashboardViewModel.CourseInfo>()
            };

            foreach (var booking in bookings)
            {
                var group = booking.Group;
                var course = group.GradeLevel.Course;

                // Fetch sessions for this group
                var sessions = _context.Sessions
                    .Where(s => s.GroupId == group.Id)
                    .ToList();

                // Fetch attendances for this student, only for sessions in this group
                var sessionIds = sessions.Select(s => s.Id).ToList();
                var attendances = _context.Attendances
                    .Where(a => a.StudentId == student.Id && sessionIds.Contains(a.SessionId))
                    .ToList();

                var totalSessions = sessions.Count;
                var absences = attendances.Count(a => !a.IsPresent);
                var absenceRatio = totalSessions > 0 ? (double)absences / totalSessions * 100 : 0;

                // Determine absence class
                string absenceClass = absenceRatio switch
                {
                    <= 10 => "low",
                    <= 20 => "medium",
                    _ => "high"
                };

                // Add course info to view model
                viewModel.Courses.Add(new StudentDashboardViewModel.CourseInfo
                {
                    CourseName = course.Name,
                    TeacherName = course.Teacher.Name,
                    GroupName = group.Name ?? $"Group {group.Id}",
                    Location = group.Location ?? course.Location ?? "Not specified",
                    Days = string.Join(" & ", group.SelectedDays),
                    Time = $"{group.StartTime:hh\\:mm tt} - {group.EndTime:hh\\:mm tt}",
                    AbsenceRatio = Math.Round(absenceRatio, 1),
                    AbsenceClass = absenceClass
                });
            }

            return View(viewModel);
        }

    }


}