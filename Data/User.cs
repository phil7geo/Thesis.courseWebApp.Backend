using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

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

        [JsonIgnore]
        public UserSession Session { get; set; }

        [JsonIgnore]
        public UserSearch Search { get; set; }
    }
}
