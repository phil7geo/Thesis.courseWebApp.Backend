using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Thesis.courseWebApp.Backend.Controllers
{
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
}
