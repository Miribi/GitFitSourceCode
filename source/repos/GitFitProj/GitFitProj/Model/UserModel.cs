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


        public int DailyStepGoal { get; set; } = 10000; // Default step goal
        public double WeightGoal { get; set; } // User's target weight

        // Add activity preferences properties
        public string PreferredActivityType { get; set; } // e.g., Running, Walking, Cycling

        // You can also add a field for the intensity preference if relevant
        public string PreferredIntensity { get; set; } // e.g., Light, Moderate, Vigorous



        public double CalculateBmi()
        {
            double heightInMeters = Height / 100.0; // assuming Height is stored in centimeters
            return Weight / (heightInMeters * heightInMeters);
        }

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



