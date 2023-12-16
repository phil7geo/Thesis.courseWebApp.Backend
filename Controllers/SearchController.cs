using Data;

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
    public IActionResult Search(
        [FromQuery] string level,
        [FromQuery] string subjects,
        [FromQuery] string duration,
        [FromQuery] bool onSale,
        [FromQuery] string rating),
        [FromQuery] string priceRange),
        [FromQuery] bool certification),
        [FromQuery] string language),
        [FromQuery] string courseFormat),
        [FromQuery] string location)
    {
        // search logic goes here
        // Process the query and return search results

    // For simplicity, let's assume the search is successful
        var results = _dbContext.Courses
            .Where(c => c.Level == level && c.Subjects.Contains(subjects) && c.Duration == duration)
            // Add more conditions based on your search criteria
            .ToList();

        return Ok(results);
}
}