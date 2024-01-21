using System;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace Thesis.courseWebApp.Backend.Data
{
    public class UserSearch
    {
        [Column("Id")]
        public int Id { get; set; }

        [Column("User_id")]
        public int UserId { get; set; }

        [Column("Search_query")]
        public string SearchQuery { get; set; }

        [Column("Filters")]
        public string Filters { get; set; }

        [Column("Timestamp")]
        public DateTime Timestamp { get; set; }

        [ForeignKey("UserId")]
        [JsonIgnore]
        public User User { get; set; }
    }
}
