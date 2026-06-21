using Microsoft.AspNetCore.Mvc;
using AssetTrack.Data;
using AssetTrack.Models;
using System.Linq;

namespace AssetTrack.Controllers
{
    public class AssetController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AssetController(ApplicationDbContext context)
        {
            _context = context;
        }

        // INDEX
        public IActionResult Index(string search)
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
            {
                return RedirectToAction("Login", "Auth");
            }

            var assets = _context.Assets.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                assets = assets.Where(a =>
                a.NamaAsset.Contains(search) ||
                a.Kategori.Contains(search));
            }

            return View(assets.ToList());
        }

        // CREATE (GET)
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // CREATE (POST)
        [HttpPost]
        public IActionResult Create(Asset asset, IFormFile fotoFile)
        {
            ModelState.Remove("Foto");

            if (fotoFile == null)
            {
                ModelState.AddModelError("Foto", "Foto wajib diupload");
            }

            if (!ModelState.IsValid)
            {
                return View(asset);
            }

            string fileName = Guid.NewGuid().ToString()
                + Path.GetExtension(fotoFile.FileName);

            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string filePath = Path.Combine(folderPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                fotoFile.CopyTo(stream);
            }

            asset.Foto = fileName;

            _context.Assets.Add(asset);
            _context.SaveChanges();

            TempData["Success"] = "Aset berhasil ditambahkan";

            return RedirectToAction("Index");
        }

        // EDIT (GET)
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var asset = _context.Assets.Find(id);

            if (asset == null)
            {
                return NotFound();
            }

            return View(asset);
        }

        // EDIT (POST)
        [HttpPost]
        public IActionResult Edit(Asset asset, IFormFile? fotoFile)
        {
            ModelState.Remove("Foto");

            var existingAsset = _context.Assets.Find(asset.Id);

            if (existingAsset == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                asset.Foto = existingAsset.Foto;
                return View(asset);
            }

            // update data
            existingAsset.NamaAsset = asset.NamaAsset;
            existingAsset.Kategori = asset.Kategori;
            existingAsset.Jumlah = asset.Jumlah;

            // jika upload foto baru
            if (fotoFile != null)
            {
                string fileName = Guid.NewGuid().ToString()
                    + Path.GetExtension(fotoFile.FileName);

                string folderPath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "images");

                // jika folder belum ada
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                string filePath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    fotoFile.CopyTo(stream);
                }

                // update foto baru
                existingAsset.Foto = fileName;
            }

            _context.SaveChanges();

            TempData["Success"] = "Asset berhasil diperbarui";

            return RedirectToAction("Index");
        }

        // DELETE
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var asset = _context.Assets.Find(id);

            if (asset == null)
            {
                return NotFound();
            }

            // hapus foto dari folder images
            if (!string.IsNullOrEmpty(asset.Foto))
            {
                string fotoPath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "images",
                    asset.Foto);

                if (System.IO.File.Exists(fotoPath))
                {
                    System.IO.File.Delete(fotoPath);
                }
            }

            // hapus data dari database
            _context.Assets.Remove(asset);
            _context.SaveChanges();

            TempData["Success"] = "Aset berhasil dihapus";

            return RedirectToAction("Index");
        }
    }
}