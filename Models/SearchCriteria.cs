namespace Thesis.courseWebApp.Backend.Models
{
    public class SearchCriteria
    {
        public string Level { get; set; }
        public string[] Subject { get; set; }
        public string Duration { get; set; }
        public bool OnSale { get; set; }
        public float Rating { get; set; }
        public float[] PriceRange { get; set; }
        public bool Certification { get; set; }
        public string[] Language { get; set; }
        public string CourseFormat { get; set; }
        public string Location { get; set; }
        public string Town { get; set; }
    }
}
