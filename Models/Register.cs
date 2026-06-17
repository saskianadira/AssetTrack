using System.ComponentModel.DataAnnotations;

namespace AssetTrack.Models
{
    public class Register
    {
        [Required(ErrorMessage = "Nama wajib diisi")]
        public string Nama { get; set; } = string.Empty;

        [Required(ErrorMessage = "Username wajib diisi")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password wajib diisi")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Konfirmasi password wajib diisi")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}