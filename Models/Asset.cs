using System.ComponentModel.DataAnnotations;

namespace AssetTrack.Models
{
    public class Asset
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Nama asset wajib diisi")]
        public string NamaAsset { get; set; }

        [Required(ErrorMessage = "Kategori wajib diisi")]
        public string Kategori { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Jumlah harus lebih dari 0")]
        public int Jumlah { get; set; }

        [Required(ErrorMessage = "Foto wajib diupload")]
        public string? Foto { get; set; }
    }
}