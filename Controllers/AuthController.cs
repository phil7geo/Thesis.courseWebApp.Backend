using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
//using System.IdentityModel.Tokens.Jwt;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Thesis.courseWebApp.Backend.Models;
using Thesis.courseWebApp.Backend.Data;
using System.Security.Cryptography;
using BCrypt.Net;

namespace Thesis.courseWebApp.Backend.Controllers
{
    [ApiController]
    [Route("api")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthController> _logger;

        //private readonly EmailService _emailService;

        public AuthController(IConfiguration configuration, AppDbContext dbContext, ILogger<AuthController> logger)
        {
            _configuration = configuration;
            _dbContext = dbContext;
            _logger = logger;
            //_emailService = emailService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationModel model)
        {
            using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    // Your registration logic goes here
                    var newUser = new User { Username = model.Username };
                    _dbContext.Users.Add(newUser);

                    var result = await _dbContext.SaveChangesAsync();

                    await transaction.CommitAsync(); // Commit the transaction

                    Console.WriteLine($"Number of changes: {result}");
                    _logger.LogInformation($"User added successfully: {newUser.Id}");

                    return Ok(new { Success = true, Message = "Registration successful", Username = model.Username });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync(); // Rollback the transaction in case of an exception
                    _logger.LogError($"Error adding user: {ex.Message}\n{ex.StackTrace}");
                    throw; // Rethrow the exception to ensure it's not silently ignored
                }
            }
        }



        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            // login logic

            // Check if the user exists
            //  replace this with your actual user repository or database check
            //var user = _dbContext.Users.SingleOrDefault(u => u.Username == model.Username);

            if (model.Username == null || model.Username == null ||  !IsValidLogin(model))
            {
                return BadRequest(new { Message = "Invalid username or password" });
            }

            // Validate other login inputs
            if (!IsValidLogin(model))
            {
                return BadRequest(new { Message = "Invalid login data" });
            }

            // For simplicity, let's assume login is successful
            // Generate and return a JWT token as a response

            var token = GenerateJwtToken(model.Username);

            return Ok(new { Success = true, Username = model.Username, Token = token });
        }

        [HttpPost("password-reset")]
        public async Task<IActionResult> ResetPassword([FromBody] PasswordResetModel model)
        {
            // Check if the email exists
            //var user = await _dbContext.Users.SingleOrDefaultAsync(u => u.Email == model.Email);

            if (model.Email == null)
            {
                return BadRequest(new { Message = "Email not found" });
            }

            // Validate other reset password inputs
            if (!IsValidPasswordReset(model))
            {
                return BadRequest(new { Message = "Invalid password reset data" });
            }

            // Generate a unique reset token or link
            var resetToken = GenerateResetToken();

            // For simplicity, let's assume the reset link is sent successfully
            // In a real-world scenario, you'd send an email with the reset link
            // For example: SendResetEmail(user.Email, resetToken);
            //_emailService.SendResetEmail(model.Email, resetToken);

            // Update the user's password (for example, assuming it's stored hashed in the database)
            string password = HashPassword(model.NewPassword); // Implement hashing logic

            // Save changes to the database
            //await _dbContext.SaveChangesAsync();

            // Return a response indicating success
            return Ok(new { Success = true, Message = "Password reset successful. Check your email for the reset link", NewPassword = model.NewPassword, HashedPassword = password, ResetToken = resetToken });
        }

        private bool CheckIfUserExists(string username)
        {
            // Replace this with your actual user repository or database check
            // Return true if the user exists, false otherwise
            return false;
        }

        private bool IsValidRegistration(RegistrationModel model)
        {
            // Validate registration inputs here
            // For example, check if the username and password meet certain criteria
            // Return true if valid, false otherwise
            return true;
        }

        private bool IsValidLogin(LoginModel model)
        {
            // Validate login inputs here
            // For example, check if the username and password meet certain criteria
            // Return true if valid, false otherwise
            return true;
        }

        private string GenerateJwtToken(string username)
        {
            var keyBytes = new byte[32]; // 32 bytes = 256 bits
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(keyBytes);
            }

            var securityKey = new SymmetricSecurityKey(keyBytes);
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(2), // Token expiration time
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private bool IsValidPasswordReset(PasswordResetModel model)
        {
            // Check if the old and new passwords meet certain criteria

            // Check if the old password is provided
            if (string.IsNullOrWhiteSpace(model.OldPassword))
            {
                return false;
            }

            // Check if the new password is provided
            if (string.IsNullOrWhiteSpace(model.NewPassword))
            {
                return false;
            }

            // Check if the new password meets minimum length requirement
            if (model.NewPassword.Length < 8)
            {
                return false;
            }

            // If all checks pass, return true
            return true;
        }

        private string GenerateResetToken()
        {
            // enerate a unique reset token or link
            // use a GUID
            return Guid.NewGuid().ToString();
        }

        private string HashPassword(string password)
        {
            // logic to hash the password
            // use a secure hashing algorithm like bcrypt
            // Hash the password using bcrypt
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            return hashedPassword;
        }


    }

}
