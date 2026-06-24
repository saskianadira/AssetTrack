using Microsoft.AspNetCore.Mvc;
using AssetTrack.Data;
using AssetTrack.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

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

            if (!ModelState.IsValid)
            {
                return View(user);
            }

            var hasher = new PasswordHasher<User>();
            user.Password = hasher.HashPassword(user, user.Password);

            _context.Users.Add(user);
            _context.SaveChanges();

            TempData["Success"] = "Data user berhasil ditambahkan";

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
            ModelState.Remove("Password");

            var existingUser = _context.Users.Find(user.Id);

            if (existingUser == null)
            {
                return NotFound();
            }

            // cek username selain dirinya sendiri
            var usernameExist = _context.Users
                .FirstOrDefault(u => u.Username == user.Username && u.Id != user.Id);

            if (usernameExist != null)
            {
                ModelState.AddModelError("Username", "Username sudah digunakan");
                return View(user);
            }

            existingUser.Nama = user.Nama;
            existingUser.Username = user.Username;

            // Role tetap
            existingUser.Role = existingUser.Role;

            // Jika password diisi
            if (!string.IsNullOrWhiteSpace(user.Password))
            {
                var hasher = new PasswordHasher<User>();

                existingUser.Password =
                    hasher.HashPassword(existingUser, user.Password);
            }

            _context.SaveChanges();

            TempData["Success"] = "Data user berhasil diperbarui";

            return RedirectToAction("Index");
        }

        // DELETE
        public IActionResult Delete(int id)
        {
            var user = _context.Users.Find(id);

            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            _context.SaveChanges();

            TempData["Success"] = "Data user berhasil dihapus";

            return RedirectToAction("Index");
        }
    }
}
