using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations;
namespace GitFitProj.Model
{
    public class UserModel
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [StringLength(50)]
        public string Firstname { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Lastname { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Gender { get; set; } = string.Empty;


        [Required]
        [EmailAddress]
        [StringLength(50)]
        public string Email { get; set; } = string.Empty;


        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yy}", ApplyFormatInEditMode = true)]
        public DateTime DateOfBirth { get; set; }



        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;




        [Required]
        public int Height { get; set; }


        [Required]
        public int Weight { get; set; }

        
    }


}
