using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Thesis.courseWebApp.Backend.Data;
using Thesis.courseWebApp.Backend.Models;

namespace Thesis.courseWebApp.Backend.Controllers

{
    [ApiController]
    [Route("api")]
    public class FavoriteController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public FavoriteController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost("add-favourite")]
        public async Task<IActionResult> AddFavorite([FromBody] FavoriteModel model)
        {
            try
            {
                Console.WriteLine($"Received request to add favorite for user: {model.Username}");

                // Get the user based on the username from check-auth response
                var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == model.Username);

                if (user != null)
                {
                    // Convert the user's FavouriteCourses to a list, add the new course, and convert back to a delimited string
                    var favoriteCoursesList = user.FavouriteCourses.Split(',').ToList();
                    favoriteCoursesList.Add(model.CourseTitle);
                    user.FavouriteCourses = string.Join(',', favoriteCoursesList);

                    // Save changes to the database
                    await _dbContext.SaveChangesAsync();

                    return Ok(new { Message = "Course added to favorites", Favorites = favoriteCoursesList });
                }

                return NotFound(new { Message = "User not found" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Internal Server Error", Error = ex.Message });
            }
        }


        [HttpPost("remove-favourite")]
        public async Task<IActionResult> RemoveFavorite([FromBody] FavoriteModel model)
        {
            try
            {
                // Get the user based on the username from check-auth response
                var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == model.Username);

                if (user != null)
                {
                    // Convert the user's FavouriteCourses to a list, remove the course, and convert back to a delimited string
                    var favoriteCoursesList = user.FavouriteCourses.Split(',').ToList();
                    favoriteCoursesList.Remove(model.CourseTitle);
                    user.FavouriteCourses = string.Join(',', favoriteCoursesList);

                    // Save changes to the database
                    await _dbContext.SaveChangesAsync();

                    return Ok(new { Message = "Course removed from favorites", Favorites = favoriteCoursesList });
                }

                return NotFound(new { Message = "User not found" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Internal Server Error", Error = ex.Message });
            }
        }


        [HttpPost("get-favourites")]
        public async Task<IActionResult> GetFavorites([FromBody] UsernameFavoriteRequest request)
        {
            try
            {
                // Get the user based on the username from check-auth response
                var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == request.Username);

                if (user != null)
                {
                    // Convert the user's FavouriteCourses to a list before returning
                    var favoriteCoursesList = user.FavouriteCourses.Split(',').ToList();

                    // Return the user's favorite courses
                    return Ok(new { Favorites = favoriteCoursesList });
                }

                return NotFound(new { Message = "User not found" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Internal Server Error", Error = ex.Message });
            }
        }

    }
}