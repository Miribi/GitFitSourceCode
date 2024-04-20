
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using GitFitProj.Model;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using GitFitProj.Controllers;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Localization;



// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860


namespace EADproject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FitController : ControllerBase
    {
        private readonly GitFitContext _context;
        private readonly IStringLocalizer<FitController> _localizer;
        private readonly SymmetricSecurityKey _securityKey;
        public FitController(GitFitContext context, IStringLocalizer<FitController> localizer, IConfiguration configuration)
        { // Generate a random key with 32 bytes (256 bits)
            _context = context;
            _localizer = localizer;
            var jwtKey = configuration["JwtConfig:Secret"];
            _securityKey = new SymmetricSecurityKey(Convert.FromBase64String(jwtKey));
        }





        // GET api/<FitController>/id
        [HttpGet("{id}")]
        public async Task<ActionResult<UserModel>> GetUser(int id)
        {
            var user = await _context.UserModel.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }


        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(string email, string password)
        {
            var user = await _context.UserModel.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null) return NotFound("User not found. Please Check your details or Sign Up");
            if (!VerifyPassword(password, user.Password)) return Unauthorized("Invalid password.");
            return Ok(GenerateJwtToken(user));
        }

        private bool VerifyPassword(string password, string hashedPassword)
        {
            using var sha256 = SHA256.Create();
            return Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(password))) == hashedPassword;
        }

        private string GenerateJwtToken(UserModel user)
        {
            var credentials = new SigningCredentials(_securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[] { new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()) };
            var token = new JwtSecurityToken(
                issuer: "GitFit Tracker",
                audience: "https://localhost:7288",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }




        [HttpPost("signup")]
        public async Task<ActionResult<UserModel>> SignUp(UserModel model)
        {
            if (await _context.UserModel.AnyAsync(u => u.Email == model.Email))
                return Conflict("This account already exists. Please reset Password.");
            model.Password = HashPassword(model.Password);
            _context.UserModel.Add(model);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetUser), new { id = model.UserId }, model);
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            return Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(password)));
        }




        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.UserModel.FindAsync(id);
            if (user == null)
            {
                return Conflict("This account was not found. Please try again.");
            }

            _context.UserModel.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent(); // Return a 204 No Content status
        }


        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateUserDetails(int id, [FromBody] UserModel userDetails)
        {
            var user = await _context.UserModel.FindAsync(id);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Update user details only if they are provided in the request
            user.Firstname = userDetails.Firstname ?? user.Firstname;
            user.Lastname = userDetails.Lastname ?? user.Lastname;
            user.Gender = userDetails.Gender ?? user.Gender;
            user.Email = userDetails.Email ?? user.Email;
            user.DateOfBirth = userDetails.DateOfBirth != default ? userDetails.DateOfBirth : user.DateOfBirth;
            user.Height = userDetails.Height != default ? userDetails.Height : user.Height;
            user.Weight = userDetails.Weight != default ? userDetails.Weight : user.Weight;

            // Update the password only if a new value is provided
            if (!string.IsNullOrEmpty(userDetails.Password))
            {
                user.Password = HashPassword(userDetails.Password);
            }

            // Save changes to the database
            await _context.SaveChangesAsync();

            return Ok("User details updated successfully.");
        }




        [HttpGet("bmi/{userId}")]
        public async Task<ActionResult<object>> GetBmi(int userId)
        {
            var user = await _context.UserModel.FindAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            double bmi = user.CalculateBmi();
            string bmiCategory = user.GetBmiCategory(bmi);

            return Ok(new { Bmi = bmi, Category = bmiCategory });
        }





        [HttpGet("activity/{id}")]
        public async Task<ActionResult<ActivityModel>> GetActivity(int id)
        {
            var activity = await _context.Activity.FindAsync(id);
            if (activity == null)
            {
                return NotFound("Activity not found.");
            }
            return Ok(activity);
        }



        //Create Activity
        [Authorize]
        [HttpPost("activity")]
        public async Task<ActionResult<ActivityModel>> CreateActivity([FromBody] ActivityModel activity)
        {

            if (activity.ActivityDate > DateTime.UtcNow)
            {
                return BadRequest(_localizer["BadRequest"]);
            }

            // Retrieve user ID from the JWT claim
            var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized("You need to be logged in to perform this action.");
            }

            // Convert the user ID claim to an integer
            if (!int.TryParse(userIdClaim.Value, out var userId))
            {
                return Unauthorized("Invalid user ID.");
            }

            var user = await _context.UserModel.FindAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Set the UserId from the JWT claim
            activity.UserId = userId;

            _context.Activity.Add(activity);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetActivity), new { id = activity.LogId }, activity);
        }

        [HttpGet("stepgoal/{userId}")]
        public async Task<ActionResult<int>> GetStepGoal(int userId)
        {
            var user = await _context.UserModel.FindAsync(userId);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            return user.DailyStepGoal;
        }

        // PUT api/<FitController>/stepgoal/{userId}
        [HttpPut("stepgoal/{userId}")]
        public async Task<IActionResult> SetStepGoal(int userId, [FromBody] int stepGoal)
        {
            var user = await _context.UserModel.FindAsync(userId);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            if (stepGoal <= 0)
            {
                return BadRequest("Step goal must be a positive number.");
            }

            user.DailyStepGoal = stepGoal;
            await _context.SaveChangesAsync();

            return Ok("Step goal updated successfully.");
        }

    }



}




