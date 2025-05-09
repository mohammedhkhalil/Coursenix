// Controllers/CoursesController.cs
using Microsoft.AspNetCore.Mvc;
using Coursenix.Models; 
using Coursenix.Models.ViewModels; 
using Microsoft.EntityFrameworkCore;
using System.Linq; 
using System.Threading.Tasks; 
using System.IO; 
using Microsoft.AspNetCore.Hosting; 


namespace Coursenix.Controllers
{
    // تم التعليق على سمة Authorize مؤقتاً لتجاوز خطأ المصادقة بدون تسجيل دخول
    // [Authorize(Roles = "Teacher")]
    public class CoursesController : Controller
    {
        private readonly Context _context;
        private readonly IWebHostEnvironment _webHostEnvironment; 
        // private readonly UserManager<Teacher> _userManager;


        public CoursesController(Context context, IWebHostEnvironment webHostEnvironment /*, UserManager<Teacher> userManager */)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            // _userManager = userManager; 
        }

        // GET: /Courses/Index
        public async Task<IActionResult> Index(string selectedSubject, int? selectedGrade, string searchQuery)
        {
            var allSubjectNames = await _context.Subjects
                                        .Select(s => s.SubjectName)
                                        .Distinct()
                                        .OrderBy(name => name)
                                        .ToListAsync();

            var allGradeLevels = await _context.Subjects
                                      .Select(s => s.GradeLevel)
                                      .Distinct()
                                      .OrderBy(grade => grade)
                                      .ToListAsync();

            var subjectsQuery = _context.Subjects
                                .Include(s => s.Teacher)
                                .AsQueryable();

            if (!string.IsNullOrEmpty(selectedSubject) && selectedSubject != "All subjects")
            {
                subjectsQuery = subjectsQuery.Where(s => s.SubjectName == selectedSubject);
            }

            if (selectedGrade.HasValue && selectedGrade.Value > 0)
            {
                subjectsQuery = subjectsQuery.Where(s => s.GradeLevel == selectedGrade.Value);
            }

            if (!string.IsNullOrEmpty(searchQuery))
            {
                subjectsQuery = subjectsQuery.Where(s => s.Teacher != null && s.Teacher.Name.Contains(searchQuery));
            }

            var subjects = await subjectsQuery.OrderBy(s => s.SubjectName).ToListAsync();

            var viewModel = new CourseListViewModel
            {
                Courses = subjects,
                AvailableSubjects = allSubjectNames,
                AvailableGrades = allGradeLevels,
                SelectedSubject = selectedSubject,
                SelectedGrade = selectedGrade,
                SearchQuery = searchQuery
            };

            return View(viewModel);
        }

        // GET: /Courses/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Courses/Create
        [HttpPost]
        [ValidateAntiForgeryToken] 
        // [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Create(CreateCourseViewModel model)
        {
            if (ModelState.IsValid)
            {
                string thumbnailFileName = null;
                if (model.ThumbnailFile != null)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "thumbnails");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    thumbnailFileName = Guid.NewGuid().ToString() + "_" + model.ThumbnailFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, thumbnailFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.ThumbnailFile.CopyToAsync(fileStream);
                    }
                }


                int gradeLevel;
                if (!int.TryParse(model.GradeLevels, out gradeLevel))
                {
                    ModelState.AddModelError("GradeLevels", "Please enter a valid number for Grade Levels.");
                    return View(model);
                }

                int currentTeacherId = 12; 
                var subject = new Subject
                {
                    SubjectName = model.CourseTitle,
                    Description = model.CourseDescription,
                    GradeLevel = gradeLevel,
                    Location = model.Location,
                    TeacherId = currentTeacherId, 
                    ThumbnailFileName = thumbnailFileName, 
                };

                _context.Subjects.Add(subject);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Courses");
            }

            return View(model);
        }

        public async Task<IActionResult> Details(int? id) 
        {
            if (id == null)
            {
                return NotFound();
            }

            var subject = await _context.Subjects
                .Include(s => s.Teacher) 
                .Include(s => s.Groups)  
                .FirstOrDefaultAsync(m => m.SubjectId == id);

            if (subject == null)
            {
                return NotFound();
            }

            var viewModel = new CourseDetailsViewModel
            {
                Subject = subject,
                Groups = subject.Groups?.ToList() 
                                      ?? new List<Group>() 
            };

            // viewModel.Groups = viewModel.Groups.OrderBy(g => g.DayOfWeek).ThenBy(g => g.StartTime).ToList();


            return View("Details", viewModel);
        }

    }
}