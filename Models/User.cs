using System.ComponentModel.DataAnnotations;

namespace AssetTrack.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Nama Lengkap wajib diisi")]
        public string Nama { get; set; } = string.Empty;

        [Required(ErrorMessage = "Username wajib diisi")]
        [MinLength(5, ErrorMessage = " Username minimal diisi dengan 5 karakter")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password wajib diisi")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Role wajib di pilih")]
        public string Role { get; set; } = string.Empty; // admin / user
    }
}