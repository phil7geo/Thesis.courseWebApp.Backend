using HtmlAgilityPack;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Thesis.courseWebApp.Backend.Data;
using Thesis.courseWebApp.Backend.Models;
using System.Net;
using Newtonsoft.Json;
using Thesis.courseWebApp.Backend.Controllers;

namespace Thesis.courseWebApp.Backend.Controllers
{
    [ApiController]
    [Route("api")]
    public class SearchController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly RnnModelService _rnnModelService;
        private readonly DbSet<UserSearch> _userSearches;
        private readonly AuthController _authController;

        public SearchController(AppDbContext dbContext, RnnModelService rnnModelService, AuthController authController)
        {
            _dbContext = dbContext;
            _rnnModelService = rnnModelService;
            _userSearches = dbContext.Set<UserSearch>();
            _authController = authController;
        }

        [HttpPost("search")]
        public async Task<IActionResult> Search([FromBody] SearchCriteria criteria)
        {
            try
            {

                if (criteria == null)
                {
                    return BadRequest(new { Message = "Bad Request", Error = "Invalid search criteria" });
                }

                var resultsFromDatabase = await _dbContext.Courses
                    .Where(c =>
                        (string.IsNullOrEmpty(criteria.Level) || c.Level == criteria.Level) &&
                        (criteria.Subject == null || criteria.Subject.All(s => c.Subject.Contains(s))) &&
                        (string.IsNullOrEmpty(criteria.Duration) || c.Duration == criteria.Duration) &&
                        (!criteria.OnSale || c.OnSale) &&
                        (!criteria.Rating.HasValue || c.Rating >= criteria.Rating) &&
                              ((criteria.PriceRange == null || criteria.PriceRange.Length != 2) ||
                            (criteria.PriceRange[0] == null && criteria.PriceRange[1] == null) ||
                            (criteria.PriceRange[0] == null || c.Price >= (double)criteria.PriceRange[0]) &&
                            (criteria.PriceRange[1] == null || c.Price <= (double)criteria.PriceRange[1])) &&
                        (!criteria.Certification || c.Certification) &&
                        (criteria.Language == null || criteria.Language.Length == 0 || criteria.Language.Contains(c.Language)) &&
                        (string.IsNullOrEmpty(criteria.CourseFormat) || c.CourseFormat == criteria.CourseFormat) &&
                        (string.IsNullOrEmpty(criteria.Location) || c.Location == criteria.Location) &&
                        (string.IsNullOrEmpty(criteria.Town) || c.Town == criteria.Town)
                    )
                    .ToListAsync();

                Console.WriteLine($"Search Criteria in payload: {JsonConvert.SerializeObject(criteria)}");
                Console.WriteLine($"Results from the Database (Courses table): {JsonConvert.SerializeObject(resultsFromDatabase)}");

                if (resultsFromDatabase.Any())
                {
                    return Ok(new { Message = "Success", ActualMatchedResults = resultsFromDatabase });
                }

                resultsFromDatabase = await _dbContext.Courses
                    .Where(c =>
                        (criteria.Subject == null || criteria.Subject.Any(s => c.Subject.Contains(s))) &&
                        (string.IsNullOrEmpty(criteria.Level) || c.Level == criteria.Level) &&
                        (string.IsNullOrEmpty(criteria.Duration) || c.Duration == criteria.Duration)
                    )
                    .ToListAsync();

                var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                var userId = _authController.GetUserIdFromToken(token);

                try
                {
                    var criteriaJson = JsonConvert.SerializeObject(criteria);
                    var searchQuery = JsonConvert.DeserializeObject<Dictionary<string, object>>(criteriaJson);

                    // Extract keys with non-null values
                    var filterKeys = searchQuery
                        .Where(kv => kv.Value != null)
                        .Select(kv => kv.Key)
                        .ToList();

                    // Create the Filters string
                    var filters = string.Join(",", filterKeys);

                    // Save the Course User Search in the respective DB table         
                    var userSearch = new UserSearch
                    {
                        UserId = userId,
                        SearchQuery = JsonConvert.SerializeObject(criteria),
                        Filters = filters,
                        Timestamp = DateTime.UtcNow
                    };

                    if (userSearch != null)
                    {
                        _dbContext.UserSearches.Add(userSearch);
                        int savedChanges = await _dbContext.SaveChangesAsync();
                        Console.WriteLine($"Number of changes saved: {savedChanges}");
                    }

                }
                catch (DbUpdateException ex)
                {
                    // Handle database update exception
                    Console.WriteLine($"Error updating the database: {ex.Message}");
                }

                if (resultsFromDatabase.Any())
                {
                    return Ok(new { Message = "Success", SimilarMatchedResults = resultsFromDatabase });
                }

                return StatusCode(500, new { Message = "Internal Server Error", Error = "No results with the provided search filters" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during database query: {ex.Message}");
                return StatusCode(500, new { Message = "Internal Server Error", Error = ex.Message });
            }

        }


        [HttpPost("predictions")]
        public async Task<IActionResult> Predictions([FromBody] PredictionsRequest request)
        {
            try
            {
                var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
                Console.WriteLine($"User-username: {user}");
                if (user == null)
                {
                    return NotFound(new { Message = "User not found", Username = user });
                }

                // Implement logic to process user input and get predictions from RNN model
                var predictions = await GetPredictionsFromRNN(request.UserInput, user.Username);

                return Ok(new { Predictions = predictions });
            }
            catch (Exception ex)
            {
                // Handle exceptions and return an appropriate error response
                return StatusCode(500, new { Message = "Internal Server Error", Error = ex.Message });
            }
        }


        private async Task<List<string>> GetPredictionsFromRNN(string userInput,string username)
        {
            try
            {
                var preprocessedInput = _rnnModelService.PreprocessUserInput(userInput);
                var predictions = await _rnnModelService.Predict(username);
                var predictedData = _rnnModelService.ProcessPredictions(predictions);

                return predictedData;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error getting predictions: {ex.Message}");
                return new List<string>();
            }
        }


    }

}
