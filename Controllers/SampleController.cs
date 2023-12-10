using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class SampleController : ControllerBase
{
    [HttpGet]
    public ActionResult<string> Get()
    {
        return "Hello from the backend!";
    }
}
