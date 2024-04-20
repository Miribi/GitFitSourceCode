using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace GitFitProj.Model
{
    public class ActivityModel
    {
        [Key]
        public int LogId { get; set; }
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public UserModel User { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime ActivityDate { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Steps must be over Zero.")]
        public int Steps { get; set; }
        public string ActivityType { get; set; } // e.g., Running, Walking, Cycling
        public double Distance { get; set; } // in kilometers or miles
        public string Intensity { get; set; }
        public TimeSpan Duration { get; set; } // Duration of the activity
        public double CaloriesBurned { get; set; } // Estimated calories burned
        public int AverageHeartRate { get; set; } // Average during the activity


        public void CalculateCaloriesBurned(int age, string gender)
        {
            double genderConstant = gender == "Male" ? 1 : 0.9;
            double durationInMinutes = Duration.TotalMinutes;

            CaloriesBurned = ((age * 0.2017 / genderConstant)
                              - (User.Weight * 0.09036 / genderConstant)
                              + (AverageHeartRate * 0.6309 / genderConstant)
                              - 55.0969) * durationInMinutes / 4.184;
        }
        

        // Example of method to check step goals
        public bool CheckStepGoal()
        {
            return Steps >= User.DailyStepGoal;
        }
    }

}
