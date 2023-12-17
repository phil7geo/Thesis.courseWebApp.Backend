//using Data;
using HtmlAgilityPack;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Thesis.courseWebApp.Backend.Data;
using Thesis.courseWebApp.Backend.Models;

namespace Thesis.courseWebApp.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SearchController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public SearchController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(
            [FromQuery] string level,
            [FromQuery] string subjects,
            [FromQuery] string duration,
            [FromQuery] bool onSale,
            [FromQuery] string rating,
            [FromQuery] string priceRange,
            [FromQuery] bool certification,
            [FromQuery] string language,
            [FromQuery] string courseFormat,
            [FromQuery] string location)
        {
            // Your existing search logic with database queries
            var resultsFromDatabase = _dbContext.Courses
                .Where(c => c.Level == level && c.Subjects.Contains(subjects) && c.Duration == duration)
                // Add more conditions based on your search criteria
                .ToList();

            // Web scraping logic to get results from e-course sites
            var resultsFromWeb = await GetResultsFromWeb(level, subjects, duration, onSale, rating, priceRange, certification, language, courseFormat, location);

            // Combine results from the database and web scraping
            var allResults = resultsFromDatabase.Concat(resultsFromWeb).ToList();

            return Ok(allResults);
        }

        private async Task<List<Course>> GetResultsFromWeb(string level, string subjects, string duration, bool onSale, string rating, string priceRange, bool certification, string language, string courseFormat, string location)
        {
            // Implement web scraping logic here
            // Example using HtmlAgilityPack and Udemy as a reference (modify as needed)
            var udemyResults = await ScrapeUdemy(level, subjects, duration, onSale, rating, priceRange, certification, language, courseFormat, location);

            // Add more web scraping logic for other e-course sites

            return udemyResults;
        }

        private async Task<List<Course>> ScrapeUdemy(string level, string subjects, string duration, bool onSale, string rating, string priceRange, bool certification, string language, string courseFormat, string location)
        {
            var udemyResults = new List<Course>();

            // Example Udemy URL (modify based on Udemy's search URL structure)
            var udemyUrl = $"https://www.udemy.com/courses/search/?q={subjects}&price={priceRange}&language={language}";

            // Send HTTP request and load HTML content
            using (var httpClient = new HttpClient())
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

            return udemyResults;
        }
    }

}
