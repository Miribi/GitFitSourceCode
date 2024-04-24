using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using GitFitProj.Model;
using Microsoft.EntityFrameworkCore;
using GitFitProj.Controllers;
using System.IdentityModel.Tokens.Jwt;

namespace EADproject.Controllers
{
    /// <summary>
    /// FitController with all the GET, PUT ... implementations.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class FitController : ControllerBase
    {
        private readonly GitFitContext _context;
        private readonly IStringLocalizer<FitController> _localizer;

        /// <summary>
        /// Database connection and Localizer.
        /// </summary>
        public FitController(GitFitContext context, IStringLocalizer<FitController> localizer, IConfiguration configuration)
        {
            _context = context;
            _localizer = localizer;
           
        }
        /// <summary>
        /// Gets or sets the unique identifier for the user.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<UserModel>> GetUser(int id)
        {
            var user = await _context.UserModel.FindAsync(id);
            if (user == null)
            {
                return NotFound(_localizer["User not found."]);
            }
            return Ok(user);
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
        /// <summary>
        /// Gets or sets the unique identifier for the user.
        /// </summary>
        [HttpPost("signup")]
        public async Task<ActionResult<UserModel>> SignUp([FromBody]UserModel model)
        {
            if (await _context.UserModel.AnyAsync(u => u.Email == model.Email))
            {
                return Conflict(_localizer["This account already exists. Please reset your password."]);
            }

            model.Password = HashPassword(model.Password);
            _context.UserModel.Add(model);
            await _context.SaveChangesAsync();

            // The ID will be generated automatically when saving changes if configured correctly
            return CreatedAtAction(nameof(GetUser), new { id = model.UserId }, model);
        }

        private bool VerifyPassword(string providedPassword, string storedHash)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(providedPassword));
            return Convert.ToBase64String(hashedBytes) == storedHash;
        }
        /// <summary>
        /// Gets or sets the unique identifier for the user.
        /// </summary>
        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(string email, string password)
        {
            var user = await _context.UserModel.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                return NotFound(_localizer["User not found. Please check your details or sign up."]);
            }

            if (!VerifyPassword(password, user.Password))
            {
                return Unauthorized(_localizer["Invalid password."]);
            }
            return Ok("Login Succesfull");
           
        }


        /// <summary>
        /// Gets or sets the unique identifier for the user.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.UserModel.FindAsync(id);
            if (user == null)
            {
                return NotFound(_localizer["User not found."]);
            }

            _context.UserModel.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(_localizer["User deleted successfully."]);
        }





        /// <summary>
        /// Gets or sets the unique identifier for the user.
        /// </summary>
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateUserDetails(int id, [FromBody] UserModel userDetails)
        {
            var user = await _context.UserModel.FindAsync(id);
            if (user == null)
            {
                return NotFound(_localizer["User not found."]);
            }

                            // Updating user details
            user.Firstname = userDetails.Firstname ?? user.Firstname;
            user.Lastname = userDetails.Lastname ?? user.Lastname;
            user.Gender = userDetails.Gender ?? user.Gender;
            user.Email = userDetails.Email ?? user.Email;
            user.DateOfBirth = userDetails.DateOfBirth != default ? userDetails.DateOfBirth : user.DateOfBirth;
            user.Height = userDetails.Height != default ? userDetails.Height : user.Height;
            user.Weight = userDetails.Weight != default ? userDetails.Weight : user.Weight;
            user.DailyStepGoal = userDetails.DailyStepGoal != default ? userDetails.DailyStepGoal : user.DailyStepGoal;
            user.WeightGoal = userDetails.WeightGoal != default ? userDetails.WeightGoal : user.WeightGoal;
            user.PreferredActivityType = userDetails.PreferredActivityType ?? user.PreferredActivityType;
            user.PreferredIntensity = userDetails.PreferredIntensity != default ? userDetails.PreferredIntensity : user.PreferredIntensity;

            await _context.SaveChangesAsync();
            return Ok(_localizer["User updated successfully."]);
        }
        /// <summary>
        /// Gets or sets the unique identifier for the user.
        /// </summary>
        [HttpGet("bmi/{userId}")]
        public async Task<ActionResult<object>> GetBmi(int userId)
        {
            var user = await _context.UserModel.FindAsync(userId);
            if (user == null)
            {
                return NotFound(_localizer["User not found."]);
            }

            double? bmi = user.CalculateBmi();
            string bmiCategory = user.GetBmiCategory(bmi.Value);  // Ensure CalculateBmi and GetBmiCategory methods are implemented

            return Ok(new { Bmi = bmi, Category = bmiCategory });
        }
        /// <summary>
        /// Gets or sets the unique identifier for the user.
        /// </summary>
        [HttpPost("activity")]
        public async Task<ActionResult<ActivityModel>> CreateActivity([FromBody] ActivityModel activity)
        {
            var validActivityTypes = new List<string> { "Running", "Weights", "Cycling", "Swimming", "Yoga", "Zumba", "Other Workout" };
            if (activity.ActivityDate > DateTime.UtcNow)
            {
                return BadRequest(_localizer["Activity date cannot be in the future."]);
            }

            if (!validActivityTypes.Contains(activity.ActivityType))
            {
                return BadRequest(new
                {
                    Error = _localizer["Invalid activity type. Valid types are:"] + " " + String.Join(", ", validActivityTypes)
                });
            }

            var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized(_localizer["You need to be logged in to perform this action."]);
            }

            int userId = int.Parse(userIdClaim.Value);
            var user = await _context.UserModel.FindAsync(userId);
            if (user == null)
            {
                return NotFound(_localizer["User not found."]);
            }

            activity.UserId = userId;
            _context.Activity.Add(activity);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetActivity), new { id = activity.LogId }, activity);
        }
        /// <summary>
        /// Gets stepgoal for the user.
        /// </summary>
        [HttpGet("stepgoal/{userId}")]
        public async Task<ActionResult<int>> GetStepGoal(int userId)
        {
            var user = await _context.UserModel.FindAsync(userId);
            if (user == null)
            {
                return NotFound(_localizer["User not found."]);
            }

            return Ok(user.DailyStepGoal);
        }
        /// <summary>
        /// Gets or sets the unique identifier for the user.
        /// </summary>
        [HttpPut("stepgoal/{userId}")]
        public async Task<IActionResult> SetStepGoal(int userId, [FromBody] int stepGoal)
        {
            var user = await _context.UserModel.FindAsync(userId);
            if (user == null)
            {
                return NotFound(_localizer["User not found."]);
            }

            if (stepGoal <= 0)
            {
                return BadRequest(_localizer["Step goal must be a positive number."]);
            }

            user.DailyStepGoal = stepGoal;
            await _context.SaveChangesAsync();

            return Ok(_localizer["Step goal updated successfully."]);
        }

        /// <summary>
        /// Gets or sets the unique identifier for the user.
        /// </summary>
        [HttpGet("activity/{id}")]
        public async Task<ActionResult<ActivityModel>> GetActivity(int id)
        {
            var activity = await _context.Activity.FindAsync(id);
            if (activity == null)
            {
                return NotFound(_localizer["Activity not found."]);
            }

            return Ok(activity);
        }
    }
}

