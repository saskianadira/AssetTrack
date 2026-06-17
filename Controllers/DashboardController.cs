using Microsoft.AspNetCore.Mvc;
using AssetTrack.Data;
using System.Linq;
using System.Numerics;

namespace AssetTrack.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
            {
                return RedirectToAction("Login", "Auth");
            }

            // TOTAL ASSET
            ViewBag.TotalAsset = _context.Assets.Count();

            // TOTAL STOK
            ViewBag.TotalStok = _context.Assets.Sum(a => a.Jumlah);

            // SEDANG DIPINJAM
            ViewBag.Dipinjam = _context.Peminjamans
                .Count(p => p.Status == "Disetujui");

            // VERIFIKASI PENGEMBALIAN
            ViewBag.VerifikasiPengembalian = _context.Peminjamans
                .Count(p => p.Status == "Verifikasi");

            // TOTAL SCRAP
            ViewBag.TotalScrap = _context.Peminjamans
                .Count(p => p.KondisiBarang == "Scrap");

            // TOTAL LOST
            ViewBag.Lost = _context.Peminjamans
                .Count(p => p.KondisiBarang == "Lost");

            return View();
        }
    }
}
