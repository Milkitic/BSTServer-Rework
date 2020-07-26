using System.ComponentModel.DataAnnotations;

namespace BSTServer.Models
{
    public class AuthenticateModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Md5Password { get; set; }
    }
}
