using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Thesis.courseWebApp.Backend.Models;
using Thesis.courseWebApp.Backend.Data;

namespace Thesis.courseWebApp.Backend.Controllers
{

    [ApiController]
    [Route("api")]
    public class ContactController : ControllerBase
    {
        //private readonly ContactMessageRepository _messageRepository;
        //private readonly EmailService _emailService;
        private readonly AppDbContext _dbContext;

        public ContactController(AppDbContext dbContext)
        {
            //_messageRepository = messageRepository;
            //_emailService = emailService;
            _dbContext = dbContext;
        }

        [HttpPost("contact/send-message")]
        public async Task<IActionResult> SendMessage([FromBody] ContactMessageModel model)
        {
            try
            {
                // Validate the model
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { Message = "Invalid message format" });
                }

                // Save the message to the database
                var message = new Message
                {
                    FullName = model.FullName,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    MessageContent = model.Message,
                    CreatedAt = DateTime.UtcNow
                };

                // Add the user to the Messages DB table
                _dbContext.Messages.Add(message);
                // Save changes to the database
                await _dbContext.SaveChangesAsync();

                var messageContent = message.MessageContent;

                // For simplicity, let's assume the message is sent successfully
                return Ok(new { Success = true, Message = "Message sent successfully", MessageSent = messageContent });
            }
            catch (Exception ex)
            {
                // Handle exceptions appropriately, log them, and return a meaningful error response
                return StatusCode(500, new { Message = "Internal Server Error", Error = ex.Message });
            }
        }

    }
}
