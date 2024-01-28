namespace Thesis.courseWebApp.Backend.Models
{
    public class FavoriteModel
    {
        public string Username { get; set; }
        public string CourseTitle { get; set; }
    }

    public class UsernameFavoriteRequest
    {
        public string Username { get; set; }
    }

}