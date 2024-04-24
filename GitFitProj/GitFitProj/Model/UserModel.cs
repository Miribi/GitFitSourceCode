using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace GitFitProj.Model
{
    /// <summary>
    /// Class UserModel has User Details stored and all realted Features.
    /// </summary>
    public class UserModel
    {

        /// <summary>
        /// Gets or sets the unique identifier for the user.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }


        /// <summary>
        /// Gets or sets the First Name for the user.
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Firstname { get; set; } = string.Empty;


        /// <summary>
        /// Gets or sets the Last Name for the user.
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Lastname { get; set; } = string.Empty;


        /// <summary>
        /// Gets or sets the Gender for the user.
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Gender { get; set; } = string.Empty;


        /// <summary>
        /// Gets or sets the Email for the user.
        /// </summary>
        [Required]
        [EmailAddress]
        [StringLength(50)]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the DOB for the user.
        /// </summary>
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yy}", ApplyFormatInEditMode = true)]
        public DateTime DateOfBirth { get; set; }


        /// <summary>
        /// Gets or sets the Password for the user.
        /// </summary>
        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;



        /// <summary>
        /// Gets or sets the Height for the user.
        /// </summary>
        public int? Height { get; set; }


        /// <summary>
        /// Gets or sets the Weight for the user.
        /// </summary>
        public int? Weight { get; set; }


        /// <summary>
        /// Gets or sets the StepGoal for the user.
        /// </summary>
        public int? DailyStepGoal { get; set; } = 10000; // Default step goal
        /// <summary>
        /// Gets or sets the WeightGoal for the user.
        /// </summary>
        public double? WeightGoal { get; set; } // User's target weight


        /// <summary>
        /// Get or set activity preferences properties.
        /// </summary>
        public string? PreferredActivityType { get; set; } // e.g., Running, Walking, Cycling


        /// <summary>
        /// Gets or sets the Intensity for the user.
        /// </summary>
        public string? PreferredIntensity { get; set; } // e.g., Light, Moderate, Vigorous


        /// <summary>
        /// Gets or sets the BMI for the user.
        /// </summary>
        public double? CalculateBmi()
        {
            if (Height == null || Weight == null)
            {
                return null;
            }
            double heightInMeters = (double)Height / 100.0; // assuming Height is stored in centimeters
            return (double)Weight / (heightInMeters * heightInMeters);
        }

        /// <summary>
        /// determinig BMI Category for the user.
        /// </summary>
        public string GetBmiCategory(double bmi)
        {
            if (bmi < 18.5)
                return "Underweight";
            if (bmi >= 18.5 && bmi < 25)
                return "Normal weight";
            if (bmi >= 25 && bmi < 30)
                return "Overweight";
            return "Obesity";
        }
    }
}