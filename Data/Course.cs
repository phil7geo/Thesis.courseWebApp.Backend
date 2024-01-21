using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace Thesis.courseWebApp.Backend.Data
{

    public class Course
    {
        [Column("Id")]
        public int Id { get; set; }

        [Column("Title")]
        public string Title { get; set; }

        [Column("Subject")]
        public string Subject { get; set; }

        [Column("Level")]
        public string Level { get; set; }

        [Column("Duration")]
        public string Duration { get; set; }

        [Column("On_sale")]
        public bool OnSale { get; set; }

        [Column("Rating")]
        public float Rating { get; set; }

        [Column("Price")]
        public decimal Price { get; set; }

        [Column("Certification")]
        public bool Certification { get; set; }

        [Column("Language")]
        public string Language { get; set; }

        [Column("Course_format")]
        public string CourseFormat { get; set; }

        [Column("Location")]
        public string Location { get; set; }

        [Column("Town")]
        public string Town { get; set; }
    }
}