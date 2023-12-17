using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Thesis.courseWebApp.Backend.Models;

namespace Thesis.courseWebApp.Backend.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class ContactController : ControllerBase
    {
        [HttpPost("contact/send-message")]
        public IActionResult SendMessage([FromBody] ContactMessageModel model)
        {
            // contact message handling logic goes here
            // Save the message to a database, send an email, etc.

            // For simplicity, let's assume the message is sent successfully
            return Ok(new { Message = "Message sent successfully" });
        }
    }

}
