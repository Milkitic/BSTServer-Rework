using System.ComponentModel.DataAnnotations;

namespace BSTServer.Models
{
    public class AuthenticateModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }

    public class SignUpModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Password2 { get; set; }

        [Required]
        public string InviteCode { get; set; }

        public string Captcha { get; set; }
    }
}
