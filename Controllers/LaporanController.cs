using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AssetTrack.Data;
using AssetTrack.Models;
using ClosedXML.Excel;
using System.IO;

namespace AssetTrack.Controllers
{
    public class LaporanController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LaporanController(ApplicationDbContext context)
        {
            _context = context;
        }

        // INDEX
        public IActionResult Index(
            string search,
            List<string> status,
            List<string> kondisi)
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
            {
                return RedirectToAction("Login", "Auth");
            }

            IQueryable<Peminjaman> laporan = _context.Peminjamans
                .Include(p => p.Asset)
                .AsQueryable();

            // SEARCH
            if (!string.IsNullOrEmpty(search))
            {
                laporan = laporan.Where(p =>
                    p.NamaPeminjam != null &&
                    p.NamaPeminjam.ToLower()
                    .Contains(search.ToLower()));
            }

            // FILTER STATUS 
            if (status != null && status.Any())
            {
                laporan = laporan.Where(p =>
                    p.Status != null &&
                    status.Contains(p.Status));
            }

            // FILTER KONDISI
            if (kondisi != null && kondisi.Any())
            {
                laporan = laporan.Where(p =>
                    p.KondisiBarang != null &&
                    kondisi.Contains(p.KondisiBarang));
            }

            // URUTAN TERBARU
            var result = laporan
                .OrderByDescending(p => p.Id)
                .ToList();

            return View(result);
        }

        public IActionResult ExportExcel()
        {
            var laporan = _context.Peminjamans
                .Include(p => p.Asset)
                .OrderByDescending(p => p.Id)
                .ToList();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Laporan");

                // HEADER
                worksheet.Cell(1, 1).Value = "No";
                worksheet.Cell(1, 2).Value = "Nama Peminjam";
                worksheet.Cell(1, 3).Value = "Asset";
                worksheet.Cell(1, 4).Value = "Jumlah";
                worksheet.Cell(1, 5).Value = "Status";
                worksheet.Cell(1, 6).Value = "Kondisi";
                worksheet.Cell(1, 7).Value = "Keterangan";

                int row = 2;
                int no = 1;

                foreach (var item in laporan)
                {
                    worksheet.Cell(row, 1).Value = no++;
                    worksheet.Cell(row, 2).Value = item.NamaPeminjam;
                    worksheet.Cell(row, 3).Value = item.Asset.NamaAsset;
                    worksheet.Cell(row, 4).Value = item.JumlahPinjam;
                    worksheet.Cell(row, 5).Value = item.Status;
                    worksheet.Cell(row, 6).Value = item.KondisiBarang;
                    worksheet.Cell(row, 7).Value = item.KeteranganPengembalian;

                    row++;
                }

                // STYLE HEADER
                worksheet.Range("A1:G1").Style.Font.Bold = true;

                worksheet.Range("A1:G1")
                    .Style.Alignment.Horizontal =
                    XLAlignmentHorizontalValues.Center;

                // UKURAN KOLOM
                worksheet.Column(1).Width = 8;
                worksheet.Column(2).Width = 25;
                worksheet.Column(3).Width = 25;
                worksheet.Column(4).Width = 25;
                worksheet.Column(5).Width = 20;
                worksheet.Column(6).Width = 20;
                worksheet.Column(7).Width = 40;

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);

                    var content = stream.ToArray();

                    return File(
                        content,
                            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            "LaporanAssetTrack.xlsx"
                        );
                }
            }
        }
    }
}
