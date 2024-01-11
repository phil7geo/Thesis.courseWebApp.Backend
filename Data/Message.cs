using System.ComponentModel.DataAnnotations.Schema;

namespace Thesis.courseWebApp.Backend.Models
{
    public class Message
    {
        [Column("Id")]
        public int Id { get; set; }

        [Column("Full_name")]
        public string FullName { get; set; }

        [Column("Email")]
        public string Email { get; set; }

        [Column("Phone_number")]
        public string PhoneNumber { get; set; }

        [Column("Message_content")]
        public string MessageContent { get; set; }

        [Column("Created_at")]
        public DateTime CreatedAt { get; set; }
    }
}
