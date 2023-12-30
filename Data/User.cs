using System.ComponentModel.DataAnnotations.Schema;

namespace Thesis.courseWebApp.Backend.Data
{
    public class User
    {
        [Column("Id")]
        public int Id { get; set; }

        [Column("Username")]
        public string Username { get; set; }
        // Add other properties as needed
    }
}
