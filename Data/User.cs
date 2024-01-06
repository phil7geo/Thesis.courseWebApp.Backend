using System.ComponentModel.DataAnnotations.Schema;

namespace Thesis.courseWebApp.Backend.Data
{
    public class User
    {
        [Column("Id")]
        public int Id { get; set; }

        [Column("Username")]
        public string Username { get; set; }

        [Column("Email")]
        public string Email { get; set; }

        [Column("Hashed_password")]
        public string Password { get; set; }

        public UserSession Session { get; set; }
    }
}
