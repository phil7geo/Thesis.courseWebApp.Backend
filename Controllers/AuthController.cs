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
using System.Text.RegularExpressions;

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
                    // Validate registration inputs
                    if (!IsValidRegistration(model))
                    {
                        return BadRequest(new { Message = "Invalid registration data" });
                    }

                    // Check if the username already exists
                    if (CheckIfUsernameExists(model.Username) && CheckIfEmailExists(model.Email))
                    {
                        return BadRequest(new { Message = "Username and Email already exists" });
                    }
                    else if (CheckIfEmailExists(model.Email))
                    {
                        return BadRequest(new { Message = "Email already exists" });
                    }
                    else if (CheckIfUsernameExists(model.Username))
                    {
                        return BadRequest(new { Message = "Username already exists" });
                    }

                    // Hash the password
                    string hashedPassword = HashPassword(model.Password);

                    // Create a new User entity with email, username, and hashed password
                    var newUser = new User
                    {
                        Email = model.Email,
                        Username = model.Username,
                        Password = hashedPassword
                    };

                    // Add the user to the Users table
                    _dbContext.Users.Add(newUser);

                    // Save changes to the database
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
            // Validate other login inputs
            if (!IsValidLogin(model))
            {
                return BadRequest(new { Message = "Invalid login data" });
            }

            var user = await _dbContext.Users.SingleOrDefaultAsync(u => u.Username == model.Username);

            if (user == null || !VerifyPassword(model.Password, user.Password))
            {
                return BadRequest(new { Message = "Invalid username or password. Please use valid credentials" });
            }

            var sessionId = await GenerateAndStoreSession(user.Id);

            // Generate and return a JWT token as a response
            var token = GenerateJwtToken(model.Username);

            return Ok(new { Success = true, Username = model.Username, Token = token, SessionId = sessionId });
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

        private async Task<string> GenerateAndStoreSession(int userId)
        {
            var sessionId = Guid.NewGuid().ToString();

            var userSession = new UserSession
            {
                UserId = userId,
                SessionId = sessionId,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.UserSessions.Add(userSession);
            await _dbContext.SaveChangesAsync();

            return sessionId;
        }


        private bool CheckIfUsernameExists(string username)
        {
            var userByUsernameCount = _dbContext.Users.Count(u => u.Username == username) > 0;

            if (userByUsernameCount)
            {
                ModelState.AddModelError("username", "The username already exists");
                return true;
            }

            return false;
        }

        private bool CheckIfEmailExists(string email)
        {
            var userByEmailCount = _dbContext.Users.Count(u => u.Email == email) > 0;

            if (userByEmailCount)
            {
                ModelState.AddModelError("email", "The email already exists");
                return true;
            }

            return false;
        }



        private bool IsValidRegistration(RegistrationModel model)
        {
            if (!Regex.IsMatch(model.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                ModelState.AddModelError("email", "Enter a valid email address.");
                return false;
            }

            if (!Regex.IsMatch(model.Username, @"^(?=.*\d)[a-zA-Z0-9]{6,}$"))
            {
                ModelState.AddModelError("username", "Username must be at least 6 characters and contain at least one digit.");
                return false;
            }

            if (!Regex.IsMatch(model.Password, @"^(?=.*\d)(?=.*[a-zA-Z]).{6,}$"))
            {
                ModelState.AddModelError("password", "Password must contain at least 6 characters with at least one letter and one digit.");
                return false;
            }

            return true;
        }


        private bool IsValidLogin(LoginModel model)
        {
            if (!Regex.IsMatch(model.Username, @"^(?=.*\d)[a-zA-Z0-9]{6,}$"))
            {
                ModelState.AddModelError("username", "Username must be at least 6 characters and contain at least one digit.");
                return false;
            }

            if (!Regex.IsMatch(model.Password, @"^(?=.*\d)(?=.*[a-zA-Z]).{6,}$"))
            {
                ModelState.AddModelError("password", "Password must contain at least 6 characters with at least one letter and one digit.");
                return false;
            }

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

        private bool VerifyPassword(string enteredPassword, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(enteredPassword, hashedPassword);
        }


    }

}
