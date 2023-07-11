using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PracticalTwenty.Db.Interfaces;
using PracticalTwenty.Models.Models;

namespace PracticalTwenty.Controllers
{
    public class UserController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UserController> _logger;


        public UserController(IUnitOfWork unitOfWork, ILogger<UserController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("User Data!");
            return _unitOfWork.GetRepository<User>().GetAll() != null ?
                        View(_unitOfWork.GetRepository<User>().GetAll().ToList()) :
                        Problem("Entity set 'DatabaseContext.Users'  is null.");
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _unitOfWork.GetRepository<User>().GetAll() == null)
            {
                return NotFound();
            }

            var user = _unitOfWork.GetRepository<User>().GetById(id ?? 0);
            if (user == null)
            {
                return NotFound();
            }

            _logger.LogInformation("Student data information requested!");

            return View(user);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Email,Password")] User user)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.GetRepository<User>().Insert(user);
                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation("User created!");

                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _unitOfWork.GetRepository<User>().GetAll() == null)
            {
                return NotFound();
            }

            var user = _unitOfWork.GetRepository<User>().GetById(id ?? 0);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Email,Password")] User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _unitOfWork.GetRepository<User>().Update(user);
                    _logger.LogInformation("User data edited!");
                    await _unitOfWork.SaveChangesAsync();

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
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
            return View(user);
        }
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _unitOfWork.GetRepository<User>().GetAll() == null)
            {
                return NotFound();
            }

            var user = _unitOfWork.GetRepository<User>().GetById(id ?? 0);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_unitOfWork.GetRepository<User>().GetAll() == null)
            {
                return Problem("Entity set 'DatabaseContext.Users'  is null.");
            }
            var user = _unitOfWork.GetRepository<User>().GetById(id);
            if (user != null)
            {
                _unitOfWork.GetRepository<User>().Delete(user);

            }

            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("User deleted!");

            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return (_unitOfWork.GetRepository<User>().GetAll()?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
