using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace Thesis.courseWebApp.Backend.Data
{
    public class UserSession
    {
        [Column("Id")]
        public int Id { get; set; }

        [Column("User_id")]
        public int UserId { get; set; }

        [Column("Session_id")]
        public string SessionId { get; set; }

        [Column("Created_at")]
        public DateTime CreatedAt { get; set; }

        [ForeignKey("UserId")]
        [JsonIgnore]
        public User User { get; set; }
    }
}
