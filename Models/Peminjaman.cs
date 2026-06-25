using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AssetTrack.Models
{
    public class Peminjaman
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Nama peminjam wajib diisi")]
        public string NamaPeminjam { get; set; } = string.Empty;


        [Required(ErrorMessage = "Asset wajib dipilih")]
        public int? AssetId { get; set; }


        [ForeignKey("AssetId")]
        public Asset? Asset { get; set; }


        [Required(ErrorMessage = "Jumlah Pinjam pinjam wajib diisi")]
        [Range(1, int.MaxValue, ErrorMessage = "Jumlah pinjam minimal 1")]
        public int? JumlahPinjam { get; set; }


        [Required(ErrorMessage = "Tanggal pinjam wajib diisi")]
        public DateTime? TanggalPinjam { get; set; }


        [Required(ErrorMessage = "Tanggal kembali wajib diisi")]
        public DateTime? TanggalKembali { get; set; }

        [Required(ErrorMessage = "Lokasi wajib diisi")]
        public string Lokasi { get; set; } = string.Empty;

        [Required(ErrorMessage = "Deskripsi wajib diisi")]
        public string Deskripsi { get; set; } = string.Empty;

        public string Status { get; set; } = "Pending";

        public string? Notes { get; set; }

        public string? KondisiBarang { get; set; }

        public string? KeteranganPengembalian { get; set; }
    }

}
