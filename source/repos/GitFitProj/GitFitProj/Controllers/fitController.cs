
using System;
using System.Security.Cryptography;
using GitFitProj.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using GitFitProj.Controllers;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860


namespace EADproject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FitController : ControllerBase
    {
        private readonly DataBaseController _context;
        private readonly SymmetricSecurityKey _securityKey;
        public FitController(DataBaseController context)
        {
            _context = context;



            // Generate a random key with 32 bytes (256 bits)
            byte[] keyBytes = new byte[32];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(keyBytes);
            }
            _securityKey = new SymmetricSecurityKey(keyBytes);


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
            if (user == null)
            {
                return NotFound("User not found. Please Check your details or Sign Up");
            }

            // Verify the password
            if (!VerifyPassword(password, user.Password))
            {
                return Unauthorized("Invalid password.");
            } // add reset password if there is time in the end

            // Authentication successful
            var token = GenerateJwtToken(user, _securityKey);
            return Ok(token);
        }

        private bool VerifyPassword(string password, string hashedPassword)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            var hashedPasswordInput = Convert.ToBase64String(hashedBytes);

            return hashedPasswordInput == hashedPassword;
        }
        
private string GenerateJwtToken(UserModel user, SymmetricSecurityKey securityKey)
        {
            
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                // You can add more claims as needed
            };

            var token = new JwtSecurityToken(
                issuer: "GitFit Tracker",
                audience: "https://localhost:7288",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1), // Set token expiration time
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }





        




        [HttpPost("signup")]
        public async Task<ActionResult<UserModel>> SignUp(UserModel model)
        {
            
            // Check if the email is already used
            var existingUser = await _context.UserModel.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (existingUser != null)
            {
                return Conflict("This account already exists. Please reset Password.");
            }

            // Create a new user entity from the signup model.
            var user = new UserModel
            {
                Firstname = model.Firstname,
                Lastname = model.Lastname,
                Gender = model.Gender,
                Email = model.Email,
                DateOfBirth = model.DateOfBirth,
                Password = HashPassword(model.Password),
                Height = model.Height,
                Weight = model.Weight
            };

            // Add the new user to DbContext.
            _context.UserModel.Add(user);

            // Save the changes to the database.
            await _context.SaveChangesAsync();

            // Return a CreatedAtAction response. This includes a Location header that points to the new user.
            return CreatedAtAction(nameof(GetUser), new { id = user.UserId }, user);
        }



        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
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


        [HttpPut("update")]
        public async Task<IActionResult> UpdateUser(UserModel updatedUser)
        {
            // Retrieve the user ID from the authentication token
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized("User not authenticated.");
            }

            int userId;
            if (!int.TryParse(userIdClaim.Value, out userId))
            {
                return BadRequest("Invalid user ID in the token.");
            }

            // Find the user by ID
            var user = await _context.UserModel.FindAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Update the user details
            user.Firstname = updatedUser.Firstname;
            user.Lastname = updatedUser.Lastname;
            user.Gender = updatedUser.Gender;
            user.Email = updatedUser.Email;
            user.DateOfBirth = updatedUser.DateOfBirth;
            user.Height = updatedUser.Height;
            user.Weight = updatedUser.Weight;

            // Save changes to the database
            await _context.SaveChangesAsync();

            return NoContent(); // Return 204 No Content status indicating successful update
        }




    }



}




