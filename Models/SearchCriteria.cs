namespace Thesis.courseWebApp.Backend.Models
{
    public class SearchCriteria
    {
        // Properties for search criteria
        public string Level { get; set; }
        public string[] Subjects { get; set; }
        public string Duration { get; set; }
        public bool OnSale { get; set; }
        public float Rating { get; set; }
        public float[] PriceRange { get; set; }
        public bool Certification { get; set; }
        public string[] Language { get; set; }
        public string CourseFormat { get; set; }
        public string Location { get; set; }

        // Add other properties as needed
    }
}