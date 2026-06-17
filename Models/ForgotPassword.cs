using System.ComponentModel.DataAnnotations;

namespace AssetTrack.Models
{
    public class ForgotPassword
    {
        public string Username { get; set; } = string.Empty;

        public string NewPassword { get; set; } = string.Empty;

        public string ConfirmPassword { get; set; } = string.Empty;
    }
}