using Microsoft.AspNetCore.Mvc;
using AssetTrack.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using AssetTrack.Models;

namespace AssetTrack.Controllers
{
    public class PeminjamanAdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PeminjamanAdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // INDEX
        public IActionResult Index(string search, List <string> status)
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
            {
                return RedirectToAction("Login", "Auth");
            }

            IQueryable<Peminjaman> peminjaman = _context.Peminjamans
                .Include(p => p.Asset)
                .Where(p =>
                    p.Status == "Pending" ||
                    p.Status == "Disetujui")
                .AsQueryable();

            // SEARCH NAMA PEMINJAM
            if (!string.IsNullOrEmpty(search))
            {
                peminjaman = peminjaman.Where(p => 
                    p.NamaPeminjam != null &&
                    p.NamaPeminjam.ToLower().Contains(search.ToLower()));
            }

            //FILTER STATUS
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

        // DETAIL
        public IActionResult Detail(int id)
        {
            var peminjaman = _context.Peminjamans
                .Include(p => p.Asset) // akan ikut mengambil data aset yang dipinjam
                .FirstOrDefault(p => p.Id == id);

            if (peminjaman == null)
            {
                return NotFound();
            }

            return View(peminjaman);
        }

        // APPROVE
        [HttpPost]
        public IActionResult Approve(int id, Peminjaman data)
        {
            var peminjaman = _context.Peminjamans
                .Include(p => p.Asset)
                .FirstOrDefault(p => p.Id == id);

            if (peminjaman == null)
            {
                return NotFound();
            }

            // VALAIDASI STOK
            if (peminjaman.JumlahPinjam > peminjaman.Asset.Jumlah)
            {
                peminjaman.Status = "Ditolak";
                peminjaman.Notes = "Stok asset tidak mencukupi";

                _context.SaveChanges();

                TempData["Error"] = "Pengajuan otomatis ditolak karena stok tidak mencukupi.";

                return RedirectToAction("Index");
            }

            // UBAH STATUS
            peminjaman.Status = "Disetujui";

            // SIMPAN NOTES
            peminjaman.Notes = data.Notes;

            // KURANGI STOK ASSET
            peminjaman.Asset.Jumlah -= peminjaman.JumlahPinjam;

            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // REJECT
        [HttpPost]
        public IActionResult Reject(int id, Peminjaman data)
        {
            var peminjaman = _context.Peminjamans.Find(id);

            if (peminjaman == null)
            {
                return NotFound();
            }

            peminjaman.Status = "Ditolak";
            peminjaman.Notes = data.Notes;

            _context.SaveChanges();

            return RedirectToAction("Index");
        }

    }
}
