using HtmlAgilityPack;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Thesis.courseWebApp.Backend.Data;
using Thesis.courseWebApp.Backend.Models;
using System.Net;

namespace Thesis.courseWebApp.Backend.Controllers
{
    [ApiController]
    [Route("api")]
    public class SearchController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly RnnModelService _rnnModelService;

        public SearchController(AppDbContext dbContext, RnnModelService rnnModelService)
        {
            _dbContext = dbContext;
            _rnnModelService = rnnModelService;
        }

        [HttpPost("search")]
        public async Task<IActionResult> Search([FromBody] SearchCriteria criteria)      
        {
            // existing search logic with database queries
            //var resultsFromDatabase = _dbContext.Courses
            //    .Where(c => c.Level == criteria.Level && criteria.Subjects.Any(s => c.Subjects.Contains(s)) && c.Duration == criteria.Duration)
            //    .ToList();


            try
            {
                //Web scraping logic to get results from e-course sites
                var resultsFromWeb = await GetResultsFromWeb(criteria);

                //// Combine results from the database and web scraping
                //var allResults = resultsFromDatabase.Concat(resultsFromWeb).ToList();

                //return Ok(allResults);

                return Ok(new { Message = "Success", Results = resultsFromWeb });
            }
            catch (Exception ex)
            {
                // Handle exceptions and return an appropriate error response
                return StatusCode(500, new { Message = "Internal Server Error", Error = ex.Message
            });
            }
        }

        [HttpPost("predictions")]
        public async Task<IActionResult> Predictions([FromBody] PredictInput input)
        {
            try
            {
                // Implement logic to process user input and get predictions from RNN model
                var predictions = await GetPredictionsFromRNN(input.UserInput);

                return Ok(new { Predictions = predictions });
            }
            catch (Exception ex)
            {
                // Handle exceptions and return an appropriate error response
                return StatusCode(500, new { Message = "Internal Server Error", Error = ex.Message });
            }
        }


        private async Task<List<string>> GetPredictionsFromRNN(string userInput)
        {
            try
            {
                var preprocessedInput = _rnnModelService.PreprocessUserInput(userInput);
                var predictions = await _rnnModelService.Predict(preprocessedInput);
                var predictedData = _rnnModelService.ProcessPredictions(predictions);

                return predictedData;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error getting predictions: {ex.Message}");
                return new List<string>();
            }
        }

        private async Task<List<Course>> GetResultsFromWeb(SearchCriteria criteria)
        {
            // Implement web scraping logic here
            // Example using HtmlAgilityPack and Udemy as a reference (modify as needed)
            var udemyResults = await ScrapeUdemy(criteria);

            // Add more web scraping logic for other e-course sites

            return udemyResults;
        }

        private async Task<List<Course>> ScrapeUdemy(SearchCriteria criteria)
        {
            var udemyResults = new List<Course>();

            // Example Udemy URL (modify based on Udemy's search URL structure)
            var udemyUrl = $"https://www.udemy.com/courses/search/?q={criteria.Subjects}&price={criteria.PriceRange}&language={criteria.Language}";

            // Send HTTP request and load HTML content
            using (var httpClient = new HttpClient())
            {
                // Add User-Agent header
                httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");

                try
                {
                    var htmlContent = await httpClient.GetStringAsync(udemyUrl);

                    // Parse HTML content using HtmlAgilityPack
                    var htmlDocument = new HtmlDocument();
                    htmlDocument.LoadHtml(htmlContent);

                    // Extract course information from HTML nodes
                    // Modify this part based on the actual structure of Udemy's search results page
                    var courseNodes = htmlDocument.DocumentNode.SelectNodes("//div[@class='course-card--container']");
                    if (courseNodes != null)
                    {
                        foreach (var courseNode in courseNodes)
                        {
                            var courseTitle = courseNode.SelectSingleNode(".//h4")?.InnerText.Trim();
                            var courseLink = courseNode.SelectSingleNode(".//a")?.GetAttributeValue("href", "");

                            // Add more fields as needed

                            var udemyCourse = new Course
                            {
                                Title = courseTitle,
                                Link = courseLink,
                                // Add more fields
                            };

                            udemyResults.Add(udemyCourse);
                        }
                    }
                }
                catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Forbidden)
                {
                    // Handle 403 Forbidden error
                    return udemyResults; // or handle it accordingly
                }
            }

            return udemyResults;
        }
    }

}
