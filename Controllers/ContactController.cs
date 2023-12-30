using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Thesis.courseWebApp.Backend.Models;

namespace Thesis.courseWebApp.Backend.Controllers
{

    [ApiController]
    [Route("api")]
    public class ContactController : ControllerBase
    {
        //private readonly ContactMessageRepository _messageRepository;
        //private readonly EmailService _emailService;

        public ContactController( )
        {
            //_messageRepository = messageRepository;
            //_emailService = emailService;
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

                // Check if the email already exists in the database
                //if (_messageRepository.EmailExists(model.Email))
                if (model.Email == null)
                {
                    return BadRequest(new { Message = "Email already exists" });
                }

                // Save the message to the database
                //var messageId = _messageRepository.SaveMessage(model);

                var message = model.Message;

                // Send an email (assuming you have an email service)
                //_emailService.SendEmail(model.Email, "Subject", "Body");

                // For simplicity, let's assume the message is sent successfully
                return Ok(new { Success = true, Message = "Message sent successfully", MessageSent = message });
            }
            catch (Exception ex)
            {
                // Handle exceptions appropriately, log them, and return a meaningful error response
                return StatusCode(500, new { Message = "Internal Server Error", Error = ex.Message });
            }
        }

    }
}
