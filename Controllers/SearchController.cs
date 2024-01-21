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
            try
            {
                // Query the Courses table based on the specified filters
                var resultsFromDatabase = _dbContext.Courses
                    .Where(c =>
                        (string.IsNullOrEmpty(criteria.Level) || c.Level == criteria.Level) &&
                        (criteria.Subject == null || criteria.Subject.All(s => c.Subject.Contains(s))) &&
                        (string.IsNullOrEmpty(criteria.Duration) || c.Duration == criteria.Duration) &&
                        (!criteria.OnSale || c.OnSale) &&
                        (criteria.Rating <= 0 || c.Rating >= criteria.Rating) &&
                        ((criteria.PriceRange == null || criteria.PriceRange.Length != 2) ||
                            (c.Price >= (decimal)criteria.PriceRange[0] && c.Price <= (decimal)criteria.PriceRange[1])) &&
                        (!criteria.Certification || c.Certification) &&
                        (criteria.Language == null || criteria.Language.Length == 0 || criteria.Language.Contains(c.Language)) &&
                        (string.IsNullOrEmpty(criteria.CourseFormat) || c.CourseFormat == criteria.CourseFormat) &&
                        (string.IsNullOrEmpty(criteria.Location) || c.Location == criteria.Location) &&
                        (string.IsNullOrEmpty(criteria.Town) || c.Town == criteria.Town)
                    )
                    .ToList();

                Console.WriteLine($"Criteria: {JsonConvert.SerializeObject(criteria)}");
                Console.WriteLine($"Subject: {criteria.Subject}");
                Console.WriteLine($"Duration: {criteria.Duration}");
                Console.WriteLine($"OnSale: {criteria.OnSale}");
                Console.WriteLine($"Certification: {criteria.Certification}");
                Console.WriteLine($"Rating: {criteria.Rating}");
                Console.WriteLine($"Language: {criteria.Language}");

                Console.WriteLine($"Generated SQL Query: {_dbContext.Courses.ToQueryString()}");
                Console.WriteLine($"Results from Courses Db table: {JsonConvert.SerializeObject(resultsFromDatabase)}");

                return Ok(new { Message = "Success", Results = resultsFromDatabase });
            }
            catch (Exception ex)
            {
                // Log detailed error message
                Console.WriteLine($"Error during database query: {ex.Message}");

                // Handle exceptions and return an appropriate error response
                return StatusCode(500, new { Message = "Internal Server Error", Error = ex.Message });
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

        
    }

}
