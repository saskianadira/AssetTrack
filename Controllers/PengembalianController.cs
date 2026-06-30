using Microsoft.AspNetCore.Mvc;
using AssetTrack.Data;
using Microsoft.EntityFrameworkCore;

namespace AssetTrack.Controllers
{
    public class PengembalianController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PengembalianController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
            {
                return RedirectToAction("Login", "Auth");
            }

            var pengembalian = _context.Peminjamans
                .Include(p => p.Asset)
                .Where(p =>
                    p.Status == "Verifikasi" ||
                    p.Status == "Dikembalikan")
                .OrderByDescending(p => p.Id)
                .ToList();

            return View(pengembalian);
        }

        // HALAMAN VERIFIKASI PENGEMBALIAN
        public IActionResult Verifikasi(int id)
        {
            var pengembalian = _context.Peminjamans
                .Include(p => p.Asset)
                .FirstOrDefault(p => p.Id == id);

            if (pengembalian == null)
            {
                return NotFound();
            }

            return View(pengembalian);
        }

        [HttpPost]
        public IActionResult Verifikasi(AssetTrack.Models.Peminjaman data)
        {
            // VALIDASI KONDISI BARANG
            if (string.IsNullOrWhiteSpace(data.KondisiBarang))
            {
                ModelState.AddModelError("KondisiBarang", "Kondisi barang wajib dipilih");

                data.Asset = _context.Assets.FirstOrDefault(a => a.Id == data.AssetId);

                return View(data);
            }

            if (!ModelState.IsValid)
            {
                data.Asset = _context.Assets.FirstOrDefault(a => a.Id == data.AssetId);

                return View(data);
            }

            var pengembalian = _context.Peminjamans
                .Include(p => p.Asset)
                .FirstOrDefault(p => p.Id == data.Id);

            if (pengembalian == null)
            {
                return NotFound();
            }

            // UPDATE STATUS
            pengembalian.Status = "Dikembalikan";

            // SIMPAN KONDISI BARANG
            pengembalian.KondisiBarang = data.KondisiBarang.Trim();

            // SIMPAN KETERANGAN
            pengembalian.KeteranganPengembalian = data.KeteranganPengembalian;

            // LOGIC KONDISI BARANG
            if (data.KondisiBarang == "Usable")
            {
                pengembalian.Asset!.Jumlah += pengembalian.JumlahPinjam ?? 0;
            }

            _context.SaveChanges();

            TempData["Success"] = "Pengembalian berhasil diverifikasi";

            return RedirectToAction("Index");
        }
    }
}
