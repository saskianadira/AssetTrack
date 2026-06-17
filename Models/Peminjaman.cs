using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AssetTrack.Models
{
    public class Peminjaman
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Nama peminjam wajib diisi")]
        public string NamaPeminjam { get; set; }


        [Required(ErrorMessage = "Asset wajib dipilih")]
        public int AssetId { get; set; }


        [ForeignKey("AssetId")]
        public Asset? Asset { get; set; }


        [Required(ErrorMessage = "JumlahPinjam pinjam wajib diisi")]
        public int JumlahPinjam { get; set; }


        [Required(ErrorMessage = "Tanggal pinjam wajib diisi")]
        public DateTime TanggalPinjam { get; set; }


        [Required(ErrorMessage = "Tanggal kembali wajib diisi")]
        public DateTime TanggalKembali { get; set; }

        public string Lokasi { get; set; }

        public string Deskripsi { get; set; }

        public string Status { get; set; } = "Pending";

        public string? Notes { get; set; }

        public string? KondisiBarang { get; set; }

        public string? KeteranganPengembalian { get; set; }
    }

}
