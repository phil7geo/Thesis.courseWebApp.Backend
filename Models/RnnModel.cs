using Thesis.courseWebApp.Backend.Data;
using Microsoft.EntityFrameworkCore;

namespace Thesis.courseWebApp.Backend.Models
{
    public class RnnModel
    {
        private readonly AppDbContext _dbContext; 

        public RnnModel(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<string>> GetPredictions(string username)
        {
            Console.WriteLine($"Username: {username}");

            // Check if the user exists and fetch their favorite courses
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == username);

            if (user != null)
            {
                Console.WriteLine($"User found: {user.Username}");

                // Extract the user's favorite courses from the database
                var favoriteCourses = user.FavouriteCourses.Split(',').ToList();
                Console.WriteLine($"Favorite Courses: {string.Join(", ", favoriteCourses)}");

                // If the user has favorite courses, return them
                if (favoriteCourses.Any())
                {
                    return favoriteCourses;
                }
            }

            // If the user doesn't exist or has no favorite courses, return some default values
            return new List<string> { "No favorite courses found." };
        }

    }
}
