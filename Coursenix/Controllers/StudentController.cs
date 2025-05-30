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
                fullName = student.Name,
                email = student.Email,
                phone = student.PhoneNumber,
                parentPhone = student.ParentPhoneNumber,
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

            if (!string.IsNullOrWhiteSpace(model.phone))
                student.PhoneNumber = model.phone;

            if (!string.IsNullOrWhiteSpace(model.parentPhone))
                student.ParentPhoneNumber = model.parentPhone;

            if (model.gradeLevel.HasValue)
                student.Grade = model.gradeLevel.Value ;

            await _userManager.UpdateAsync(student.AppUser);
            _context.Update(student);
            await _context.SaveChangesAsync();

            // Password change 
            if (!string.IsNullOrEmpty(model.CurrPassword) && !string.IsNullOrEmpty(model.password))
            {
                var result = await _userManager.ChangePasswordAsync(student.AppUser, model.CurrPassword, model.password);
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

        public async Task<IActionResult> DeleteStudents(int groupId, [FromBody] List<int> studentIds)
        {
            
            var group = await _context.Groups.FindAsync(groupId);
            if (group == null)
            {
                return NotFound($"Group with id {groupId} not found.");
            }

            foreach (var Id in studentIds)
            {
                var booking = await _context.Bookings
                .FirstOrDefaultAsync(b => b.GroupId == groupId && b.StudentId == Id);
                if (booking == null)
                {
                    return NotFound($"Booking for student {Id} in group {groupId} not found.");
                }
                _context.Bookings.Remove(booking);

                // Update enrolled students count
                group.EnrolledStudentsCount = Math.Max(0, group.EnrolledStudentsCount - 1);

                await _context.SaveChangesAsync();
            }

            return Ok();
        }
        
        [HttpGet]
        // course id 
        public async Task<IActionResult> Enroll(int id)
        {
            var course = await _context.Courses
                .Include(c => c.Teacher)
                .Include(c => c.GradeLevels)
                    .ThenInclude(g => g.Groups)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (course == null)
                return NotFound();

            var vm = new EnrollViewModel
            {
                CourseId = course.Id,
                CourseName = course.Name,
                Description = course.Description,
                Location = course.Location,
                TeacherName = course.Teacher.Name,
                TeacherEmail = course.Teacher.Email,
                GradeLevels = course.GradeLevels.Select(g => new GradeLevelViewModel
                {
                    GradeLevelId = g.Id,
                    NumberOfGrade = g.NumberOfGrade,
                    Price = g.Price,
                    Groups = g.Groups.Select(grp => new GroupsGradeLevel
                    {
                        GroupId = grp.Id,
                        Name = grp.Name,
                        SelectedDays = grp.SelectedDays,
                        StartTime = grp.StartTime,
                        EndTime = grp.EndTime,
                        TotalSeats = grp.TotalSeats,
                        EnrolledStudentsCount = grp.EnrolledStudentsCount
                    }).ToList()
                }).ToList()
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> EnrollPost(int groupId)
        {
            var userId = _userManager.GetUserId(User);

            var student = await _context.Students
                .Include(s => s.AppUser)
                .FirstOrDefaultAsync(s => s.AppUserId == userId);

            if (student == null) return NotFound();
            var group = await _context.Groups
               .Include(g => g.Bookings)
               .FirstOrDefaultAsync(g => g.Id == groupId);

            if (group == null)
                return NotFound("Group not found.");

            if (group.Bookings.Any(b => b.StudentId == student.Id))
                return BadRequest("You are already enrolled in this group.");

            var booking = new Booking
            {
                StudentId = student.Id,
                GroupId = groupId,
                BookingDate = DateTime.UtcNow
            };

            group.Bookings.Add(booking);

            group.EnrolledStudentsCount++;

            await _context.SaveChangesAsync();

            return Redirect("Home");
        }
        public IActionResult Dashboard()
        {
            // Get the logged-in user's ID from Identity
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

    
            //public async Task<IActionResult> Dashboard()
            //{
            //    var appUser = await _userManager.GetUserAsync(User);
            //    if (appUser is null)
            //        return Challenge();   // force login
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