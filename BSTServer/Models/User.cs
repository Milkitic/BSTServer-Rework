using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BSTServer.Models
{
    [Table("users")]
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        public string Role { get; set; }
    }

    public static class UserRoles
    {
        public const string Root = "ROOT";
        public const string Admin = "ADMIN";
        public const string User = "USER";
    }
}