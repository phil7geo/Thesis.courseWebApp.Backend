using Thesis.courseWebApp.Backend.Data;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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

            // Check if the user exists
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == username);

            if (user != null)
            {
                // Extract the user's favorite courses from the database
                var favoriteCourses = user.FavouriteCourses.Split(',').ToList();

                // If the user has favorite courses, add them to the result list
                var resultCourses = new List<string>();
                if (favoriteCourses.Any())
                {
                    resultCourses.AddRange(favoriteCourses);
                }

                var userSearchCourses = await GetUserSearchCourses(user.Id);

                // Add courses from UserSearches to the result list
                resultCourses.AddRange(userSearchCourses);

                // If there are no favorite courses and no UserSearches courses, return appropriate message
                if (!resultCourses.Any())
                {
                    return new List<string> { "No favorite courses or UserSearches courses found." };
                }

                return resultCourses;
            }

            // If the user doesn't exist, return appropriate message
            return new List<string> { "User not found." };
        }

        private async Task<List<string>> GetUserSearchCourses(int userId)
        {
            try
            {
                // Query the database to get courses from UserSearches based on User_id
                var userSearchCourses = await _dbContext.UserSearches
                    .Where(us => us.UserId == userId)
                    .Select(us => us.SearchQuery)
                    .ToListAsync();

                // Extract Subject elements from Search_query
                var subjectElements = userSearchCourses
                    .SelectMany(us =>
                    {
                        try
                        {
                            var searchQuery = JsonConvert.DeserializeObject<Dictionary<string, object>>(us);

                            // Check if searchQuery is not null and contains "Subject" key
                            if (searchQuery != null && searchQuery.ContainsKey("Subject"))
                            {
                                var subjects = searchQuery["Subject"] as JArray;

                                if (subjects != null)
                                {
                                    // Use List to filter out duplicates
                                    var uniqueSubjects = subjects
                                        .Select(se => se?.ToString())
                                        .Distinct()
                                        .ToList();

                                    return uniqueSubjects;
                                }
                            }
                        }
                        catch (JsonException ex)
                        {
                            Console.Error.WriteLine($"Error deserializing SearchQuery: {ex.Message}");
                        }

                        return Enumerable.Empty<string>();
                    })
                    .Where(se => se != null)
                    .ToList();

                return subjectElements.ToList();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error in GetUserSearchCourses: {ex.Message}");
                return new List<string>();
            }
        }

    }
}
