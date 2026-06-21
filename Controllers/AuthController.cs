using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using AssetTrack.Data;
using AssetTrack.Models;
using System.Linq;
using System.Text.RegularExpressions;

namespace AssetTrack.Controllers
{
    public class AuthController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly PasswordHasher<User> _hasher = new PasswordHasher<User>();
        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Tampilkan halaman login
        public IActionResult Login()
        {
            return View();
        }

        // Proses login
        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password))
            {
                TempData["Error"] = "Username dan password wajib diisi";
                return RedirectToAction("Login");
            }

            var user = _context.Users
                .AsEnumerable()
                .FirstOrDefault(u => u.Username == username);

            if (user != null)
            {
                var result = _hasher.VerifyHashedPassword(user, user.Password, password);

                if (result == PasswordVerificationResult.Success)
                {
                    // simpan session
                    HttpContext.Session.SetString("UserUsername", user.Username);
                    HttpContext.Session.SetString("UserRole", user.Role);

                    // redirect berdasarkan role
                    if (user.Role == "Admin")
                        return RedirectToAction("Index", "Dashboard");

                    return RedirectToAction("Index", "Peminjaman");
                }

            }

            TempData["Error"] = "Username atau password salah";
            return RedirectToAction("Login");
        }

        // GET REGISTER
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST REGISTER
        [HttpPost]
        public IActionResult Register(Register model)
        {
            if (string.IsNullOrWhiteSpace(model.Nama))
            {
                TempData["Error"] = "Nama wajib diisi";
                return View(model);
            }

            if (string.IsNullOrWhiteSpace(model.Username))
            {
                TempData["Error"] = "Username wajib diisi";
                return View(model);
            }

            if(string.IsNullOrWhiteSpace(model.Password))
            {
                TempData["Error"] = "Password wajib diisi";
                return View(model);
            }

            if (string.IsNullOrWhiteSpace(model.ConfirmPassword))
            {
                TempData["Error"] = "Konfirmasi password wajib diisi";
                return View(model);
            }

            // Username unik
            var existingUser = _context.Users
                .FirstOrDefault(u => u.Username == model.Username);

            if (existingUser != null)
            {
                TempData["Error"] = "Username sudah digunakan.";
                return View(model);
            }

            // password
            if (model.Password.Length < 8)
            {
                TempData["Error"] = "Password minimal memiliki 8 karakter.";
                return View(model);
            }

            // wajib ada huruf besar
            if (!Regex.IsMatch(model.Password, @"[A-Z]"))
            {
                TempData["Error"] = "Password harus mengandung huruf besar.";
                return View(model);
            }

            // harus ada angka
            if (!Regex.IsMatch(model.Password, @"\d"))
            {
                TempData["Error"] = "Password harus mengandung angka.";
                return View(model);
            }

            // pasword sama dengan konfirmasi
            if (model.Password != model.ConfirmPassword)
            {
                TempData["Error"] = "Konfirmasi password tidak sesuai";
                return View(model);
            }

            User user = new User
            {
                Nama = model.Nama,
                Username = model.Username,
                Role = "User"
            };

            user.Password = _hasher.HashPassword(user, model.Password);

            _context.Users.Add(user);
            _context.SaveChanges();

            TempData["Success"] = "Registrasi berhasil. Silakan login.";

            return RedirectToAction("Register");
        }

        // GET FORGOT PASSWORD
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        // POST SEARCH AKUN
        [HttpPost]
        public IActionResult ForgotPassword(ForgotPassword model)
        {
            if (string.IsNullOrWhiteSpace(model.Username))
            {
                TempData["Error"] = "Username wajib diisi.";
                return View(model);
            }

            var user = _context.Users
                .FirstOrDefault(u => u.Username == model.Username);

            if (user == null)
            {
                TempData["Error"] = "Username tidak ditemukan.";
                return View(model);
            }

            ViewBag.UserFound = true;

            return View(model);
        }

        // RESET PASSWORD
        [HttpPost]
        public IActionResult ResetPassword(ForgotPassword model)
        {
            var user = _context.Users
                .FirstOrDefault(u => u.Username == model.Username);

            if (user == null)
            {
                TempData["Error"] = "Username tidak ditemukan.";
                return View("ForgotPassword", model);
            }

            if (string.IsNullOrWhiteSpace(model.NewPassword))
            {
                TempData["Error"] = "Password baru wajib diisi.";
                ViewBag.UserFound = true;
                return View("ForgotPassword", model);
            }

            if (model.NewPassword.Length < 8)
            {
                TempData["Error"] = "Password minimal 8 karakter.";
                ViewBag.UserFound = true;
                return View("ForgotPassword", model);
            }

            if (!Regex.IsMatch(model.NewPassword, @"[A-Z]"))
            {
                TempData["Error"] = "Password harus mengandung huruf besar.";
                ViewBag.UserFound = true;
                return View("ForgotPassword", model);
            }

            if (!Regex.IsMatch(model.NewPassword, @"\d"))
            {
                TempData["Error"] = "Password harus mengandung angka.";
                ViewBag.UserFound = true;
                return View("ForgotPassword", model);
            }

            if (model.NewPassword != model.ConfirmPassword)
            {
                TempData["Error"] = "Konfirmasi password tidak sesuai.";
                ViewBag.UserFound = true;
                return View("ForgotPassword", model);
            }

            user.Password = _hasher.HashPassword(user, model.NewPassword);
            _context.SaveChanges();
            TempData["Success"] = "Password berhasil diubah. Silahkan login menggunakan password baru.";
            return RedirectToAction("Login");
        }

        // logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
