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
using static GitFitProj.Model.ActivityModel;
using System.Diagnostics;

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
        /// Gets the BMI of the user based on Weight and Height.
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
            string bmiCategory = user.GetBmiCategory(bmi.Value);

            return Ok(new { Bmi = bmi, Category = bmiCategory });
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
        /// Creates a workout based on the selected workout type.
        /// </summary>

        [HttpGet("workouts")]
        public ActionResult GetWorkouts()
        {
            var workouts = Enum.GetNames(typeof(WorkoutType)).ToList();
            var intensities = Enum.GetNames(typeof(IntensityLevel)).ToList();

            return Ok(new { Workouts = workouts, Intensities = intensities });
        }


        [HttpPost("createActivity")]
        public async Task<ActionResult<ActivityModel>> CreateActivity([FromBody] ActivityModel activity)
        {
            if (activity == null)
            {
                return BadRequest("Invalid activity data.");
            }

            _context.Activity.Add(activity);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUser), new { id = activity.UserId }, activity);
        }

    }
}

