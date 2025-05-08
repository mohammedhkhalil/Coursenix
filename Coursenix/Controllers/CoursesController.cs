// Controllers/CoursesController.cs
using Microsoft.AspNetCore.Mvc;
using Coursenix.Models; 
using Coursenix.Models.ViewModels; 
using Microsoft.EntityFrameworkCore;
using System.Linq; // نحتاجها لاستخدام ToList(), Where(), Select(), Distinct(), OrderBy()
using System.Threading.Tasks; 
using System.IO; // للتعامل مع الملفات
using Microsoft.AspNetCore.Hosting; // لجلب مسار مجلد wwwroot


namespace Coursenix.Controllers
{
    // تم التعليق على سمة Authorize مؤقتاً لتجاوز خطأ المصادقة بدون تسجيل دخول
    // [Authorize(Roles = "Teacher")]
    public class CoursesController : Controller
    {
        private readonly CoursenixContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment; // نحتاجها لرفع الصور
        // تم التعليق على UserManager مؤقتاً
        // private readonly UserManager<Teacher> _userManager;


        // Constructor - يتم حقن الخدمات هنا
        public CoursesController(CoursenixContext context, IWebHostEnvironment webHostEnvironment /*, UserManager<Teacher> userManager */)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            // _userManager = userManager; // إذا كنت تستخدم Identity
        }

        // إجراء لعرض قائمة الدورات مع دعم الفلترة والبحث
        // GET: /Courses/Index
        public async Task<IActionResult> Index(string selectedSubject, int? selectedGrade, string searchQuery)
        {
            // ... كود إجراء Index الذي قمنا بشرحه سابقاً لجلب وعرض الدورات مع الفلاتر ...
            // جلب خيارات الفلترة المتاحة
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

            // بناء استعلام جلب الدورات مع تطبيق الفلاتر والبحث
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
                subjectsQuery = subjectsQuery.Where(s => s.Teacher != null && s.Teacher.FirstName.Contains(searchQuery));
            }

            var subjects = await subjectsQuery.OrderBy(s => s.SubjectName).ToListAsync();

            // إنشاء ViewModel وتعبئته
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

        // إجراء لعرض نموذج إنشاء دورة جديدة
        // GET: /Courses/Create
        public IActionResult Create()
        {
            // ببساطة نعرض النموذج
            return View();
        }

        // إجراء لمعالجة إرسال نموذج إنشاء دورة جديدة
        // POST: /Courses/Create
        [HttpPost]
        [ValidateAntiForgeryToken] // لحماية النموذج من هجمات CSRF
        // تم التعليق على سمة Authorize مؤقتاً
        // [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Create(CreateCourseViewModel model)
        {
            if (ModelState.IsValid)
            {
                // --- بداية: معالجة رفع الصورة المصغرة (Thumbnail) ---
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

                // --- جلب Teacher ID ---
                // !!! هذا القسم تم تعديله مؤقتاً لتجاوز خطأ المصادقة !!!
                // عدنا لاستخدام قيمة ثابتة لـ TeacherId.
                // يجب استبدال هذا المنطق بمنطق جلب ID المدرس الذي سجل الدخول فعلياً عند إعداد المصادقة.
                int currentTeacherId = 12; // قيمة ثابتة مؤقتاً - تأكد أن المدرس رقم 1 موجود في قاعدة البيانات للاختبار
                // !!! نهاية الحل المؤقت لـ Teacher ID !!!

                // تم حذف أو تعليق الكود الذي يتحقق من المصادقة ويستدعي Challenge() مؤقتاً هنا


                // --- إنشاء كائن Subject ---
                var subject = new Subject
                {
                    SubjectName = model.CourseTitle,
                    Description = model.CourseDescription,
                    Price = model.CoursePrice,
                    GradeLevel = gradeLevel,
                    Location = model.Location,
                    TeacherId = currentTeacherId, // تعيين الـ ID (المؤقت حالياً) للمدرس
                    ThumbnailFileName = thumbnailFileName, // تعيين اسم ملف الصورة المصغرة
                };

                // --- إضافة وحفظ في قاعدة البيانات ---
                _context.Subjects.Add(subject);
                await _context.SaveChangesAsync(); // هذا السطر يحفظ البيانات في قاعدة البيانات

                // --- إعادة التوجيه بعد النجاح ---
                // إعادة التوجيه إلى صفحة عرض الدورات بعد إنشاء دورة جديدة
                return RedirectToAction("Index", "Courses");
            }

            // إذا كان ModelState غير صالح، أعد عرض النموذج مع الأخطاء
            return View(model);
        }

        // مستقبلاً: إجراء Details, Edit, Delete, إلخ.
    }
}