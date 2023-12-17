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

namespace Thesis.courseWebApp.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration, AppDbContext dbContext)
        {
            _configuration = configuration;
            _dbContext = dbContext;
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegistrationModel model)
        {
            // Check if the user already exists
            //  replace this with your actual user repository or database check
            bool userExists = _dbContext.Users.Any(u => u.Username == model.Username);

            if (userExists)
            {
                return BadRequest(new { Message = "Username already exists" });
            }

            // Validate other registration inputs
            if (!IsValidRegistration(model))
            {
                return BadRequest(new { Message = "Invalid registration data" });
            }

            // registration logic goes here
            // Save the user to the database, hash the password, etc.

            // For simplicity, let's assume registration is successful
            return Ok(new { Message = "Registration successful" });
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel model)
        {
            // login logic goes here
            // Check if the username and password match, validate input, etc.

            // Check if the user exists
            //  replace this with your actual user repository or database check
            var user = _dbContext.Users.SingleOrDefault(u => u.Username == model.Username);

            if (user == null || !IsValidLogin(model, user))
            {
                return BadRequest(new { Message = "Invalid username or password" });
            }

            // Validate other login inputs
            if (!IsValidLogin(model, user))
            {
                return BadRequest(new { Message = "Invalid login data" });
            }

            // For simplicity, let's assume login is successful
            // Generate and return a JWT token as a response

            var token = GenerateJwtToken(model.Username);

            return Ok(new { Token = token });
        }

        [HttpPost("password-reset")]
        public IActionResult ResetPassword([FromBody] PasswordResetModel model)
        {
            // password reset logic goes here
            // Validate input, generate and send reset link/email, etc.

            // For simplicity, let's assume the reset is successful
            return Ok(new { Message = "Password reset successful" });
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

        private bool IsValidLogin(LoginModel model, User user)
        {
            // Validate login inputs here
            // For example, check if the username and password meet certain criteria
            // Return true if valid, false otherwise
            return true;
        }

        private string GenerateJwtToken(string username)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
            new Claim(ClaimTypes.Name, username),
            // add more claims as needed
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
    }

}
