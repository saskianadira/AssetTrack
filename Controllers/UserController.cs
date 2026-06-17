using Microsoft.AspNetCore.Mvc;
using AssetTrack.Data;
using AssetTrack.Models;
using Microsoft.EntityFrameworkCore;

namespace AssetTrack.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        // INDEX
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
            {
                return RedirectToAction("Login", "Auth");
            }

            var users = _context.Users.ToList();

            return View(users);
        }

        //CREATE
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(User user)
        {
            // cek panjang username
            if (string.IsNullOrEmpty(user.Username) || user.Username.Length < 5)
            {
                ModelState.AddModelError("Username", "Username minimal diisi dengan 5 karakter");
            }

            // cek apakah username sudah ada
            var existingUser =_context.Users
                .FirstOrDefault(u => u.Username == user.Username);

            if (existingUser != null)
            {
                ModelState.AddModelError("Username", "Username sudah digunakan");
            }

            if (ModelState.IsValid)
            {
                return View(user);
            }

            _context.Users.Add(user);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // EDIT
        public IActionResult Edit(int id)
        {
            var user = _context.Users.Find(id);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }
        [HttpPost]
        public IActionResult Edit(User user)
        {
            if (ModelState.IsValid)
            {
                _context.Users.Update(user);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(user);
        }

        // DELETE
        public IActionResult Delete(int id)
        {
            var user = _context.Users.Find(id);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var user = _context.Users.Find(id);

            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }
    }
}
