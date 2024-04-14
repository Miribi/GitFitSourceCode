
using System.Security.Cryptography;
using GitFitProj.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using GitFitProj.Controllers;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860




namespace EADproject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FitController : ControllerBase
    {
        private readonly DataBaseController _context;

        public FitController(DataBaseController context)
        {
            _context = context;
        }


        // GET api/<FitController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
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
                // Add any other fields you need.
            };

            // Add the new user to your DbContext.
            _context.UserModel.Add(user);

            // Save the changes to the database.
            await _context.SaveChangesAsync();

            // Return a CreatedAtAction response. This includes a Location header that points to the new user.
            return CreatedAtAction(nameof(UserModel), new { id = user.UserId }, user);
        }

        // This is the GetUser action method that returns the created user.
        [HttpGet("{id}")]
        public async Task<ActionResult<UserModel>> GetUser(int id)
        {
            var user = await _context.UserModel.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // Other controller actions...

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
                return Conflict("This account is not find. Please try again.");
            }

            _context.UserModel.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent(); // Return a 204 No Content status
        }




    }



}

