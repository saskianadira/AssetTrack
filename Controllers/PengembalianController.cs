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

        // INDEX
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
            pengembalian.KondisiBarang = data.KondisiBarang?.Trim();

            // SIMPAN KETERANGAN PENGEMBALIAN
            pengembalian.KeteranganPengembalian = data.KeteranganPengembalian;

            // LOGIC KONDISI BARANG
            if (data.KondisiBarang == "Usable")
            {
                // stok kembali
                pengembalian.Asset.Jumlah += pengembalian.JumlahPinjam ?? 0;
            }

            else if (data.KondisiBarang == "Scrap")
            {
                // stok tidak kembali
            }

            else if (data.KondisiBarang == "Lost")
            {
                // stok tidak kembali
            }

                _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
