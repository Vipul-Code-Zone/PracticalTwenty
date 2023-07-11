using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PracticalTwenty.Db.Interfaces;
using PracticalTwenty.Models.Models;

namespace PracticalTwenty.Controllers
{
    public class StudentController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<StudentController> _logger;

        public StudentController(IUnitOfWork unitOfWork, ILogger<StudentController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("List of Students!");
            return _unitOfWork.GetRepository<Student>().GetAll() != null ?
                        View(_unitOfWork.GetRepository<Student>().GetAll().ToList()) :
                        Problem("Entity set 'UserDbContext.Students'  is null.");
        }
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _unitOfWork.GetRepository<Student>().GetAll() == null)
            {
                _logger.LogError("Student Data not found");
                return NotFound();
            }

            var student = _unitOfWork.GetRepository<Student>().GetById(id ?? 0);
            if (student == null)
            {
                _logger.LogError("Student Data not found");
                return NotFound();
            }
            _logger.LogInformation($"Student Detail Show {student.Id}");
            return View(student);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Email")] Student student)
        {
            if (ModelState.IsValid)
            {
                _logger.LogInformation("Student Added!");
                _unitOfWork.GetRepository<Student>().Insert(student);
                await _unitOfWork.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            _logger.LogWarning("Studnet Data is not valid!");
            return View(student);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _unitOfWork.GetRepository<Student>().GetAll() == null)
            {
                _logger.LogCritical("Requested Data is not in database!");
                return NotFound();
            }

            var student = _unitOfWork.GetRepository<Student>().GetById(id ?? 0);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Email")] Student student)
        {
            if (id != student.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _logger.LogInformation($"{student.Name}'s data has been Updated!");
                    _unitOfWork.GetRepository<Student>().Update(student);
                    await _unitOfWork.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(student.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(student);
        }
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _unitOfWork.GetRepository<Student>().GetAll() == null)
            {
                return NotFound();
            }

            var student = _unitOfWork.GetRepository<Student>().GetById(id ?? 0);

            if (student == null)
            {
                _logger.LogCritical("Requested Data is not in database!");

                return NotFound();
            }

            return View(student);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_unitOfWork.GetRepository<Student>().GetAll() == null)
            {
                return Problem("Entity set 'DatabaseContext.Students'  is null.");
            }
            var student = _unitOfWork.GetRepository<Student>().GetById(id);
            if (student != null)
            {
                _unitOfWork.GetRepository<Student>().Delete(student);
                _logger.LogCritical("Requested Data is not in database!");

            }

            await _unitOfWork.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StudentExists(int id)
        {
            return (_unitOfWork.GetRepository<Student>().GetAll()?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
