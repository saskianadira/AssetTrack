using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AssetTrack.Data;
using AssetTrack.Models;
using System.Linq;
using System.Reflection.Metadata.Ecma335;

namespace AssetTrack.Controllers
{
    public class PeminjamanController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PeminjamanController(ApplicationDbContext context)
        {
            _context = context;
        }

        // INDEX
        public IActionResult Index(string search, List<string> status)
        {
            if (HttpContext.Session.GetString("UserRole") != "User")
            {
                return RedirectToAction("Login", "Auth");
            }

            var username = HttpContext.Session.GetString("UserUsername");

            var peminjaman = _context.Peminjamans
                .Include(peminjam => peminjam.Asset)
                .Where(p => p.NamaPeminjam == username)
                .AsQueryable();

            // SEARCH
            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();

                peminjaman = peminjaman.Where(p =>
                    p.NamaPeminjam.ToLower().Contains(search) ||
                    p.Asset.NamaAsset.ToLower().Contains(search));
            }

            // FILTER STATUS
            if (status != null && status.Any())
            {
                peminjaman = peminjaman.Where(p =>
                    status.Contains(p.Status));
            }

            // URUTAN TERBARU
            var result = peminjaman
                .OrderByDescending(p => p.Id)
                .ToList();

            return View(result);
        }

        // CREATE (GET)
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Assets = _context.Assets
                .Where(a => a.Jumlah > 0)
                .ToList();

            return View();
        }

        // CREATE (POST)
        [HttpPost]
        public IActionResult Create(Peminjaman peminjaman)
        {
            // ISI OTOMATIS DARI USER YANG LOGIN
            peminjaman.NamaPeminjam =
                HttpContext.Session.GetString("UserUsername") ?? "";

            ModelState.Remove("NamaPeminjam");

            // VALIDASI FORM
            if (!ModelState.IsValid)
            {
                ViewBag.Assets = _context.Assets
                    .Where(a => a.Jumlah > 0)
                    .ToList();

                return View(peminjaman);
            }

            // CEK STOK
            var asset = _context.Assets.Find(peminjaman.AssetId);

            if (peminjaman.JumlahPinjam > asset.Jumlah)
            {
                TempData["Error"] = "Jumlah pinjam melebihi stok tersedia.";

                ViewBag.Assets = _context.Assets
                    .Where(a => a.Jumlah > 0)
                    .ToList();

                return View(peminjaman);
            }

            // SIMPAN DATA
            _context.Peminjamans.Add(peminjaman);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // CANCEL
        public IActionResult Cancel(int id)
        {
            var peminjaman = _context.Peminjamans.Find(id);

            if (peminjaman == null)
            {
                return NotFound();
            }

            _context.Peminjamans.Remove(peminjaman);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // KEMBALIKAN
        public IActionResult Kembalikan(int id)
        {
            var peminjaman = _context.Peminjamans.Find(id);

            if (peminjaman == null)
            {
                return NotFound();
            }

            // UBAH STATUS
            peminjaman.Status = "Verifikasi";

            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // DETAIL
        public IActionResult Detail(int id)
        {
            var peminjaman = _context.Peminjamans
                .Include(p => p.Asset)
                .FirstOrDefault(peminjaman => peminjaman.Id == id);

            if (peminjaman == null)
            {
                return NotFound();
            }

            return View(peminjaman);
        }

    }
}
