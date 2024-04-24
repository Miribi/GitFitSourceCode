using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace GitFitProj.Model
{
    /// <summary>
    /// Activity Model class to get and set Activities and related Features
    /// </summary>
    public class ActivityModel
    {
        /// <summary>
        /// Gets or sets the unique Log ID of User.
        /// </summary>
        [Key]
        public int LogId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the user.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets the the user from Usermodel.
        /// </summary>
        [ForeignKey("UserId")]
        public UserModel User { get; set; }

        /// <summary>
        /// Gets or sets WorkOut Date.
        /// </summary>
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime ActivityDate { get; set; }
        /// <summary>
        /// Gets or sets the Weight for the User.
        /// </summary>
        public int? Weight { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Steps must be above Zero.")]

        
        public int Steps { get; set; }



        /// <summary>
        /// Gets or sets the Workout Type for the user e.g., Running, Walking, Cycling. 
        /// </summary>
        public string ActivityType { get; set; }
        /// <summary>
        /// Gets or sets the Distance for the user.
        /// </summary>
        public double Distance { get; set; } // in kilometers or miles
        /// <summary>
        /// Gets or sets the Workout Intensity for the user.
        /// </summary>
        public string Intensity { get; set; }
        /// <summary>
        /// Gets or sets the Duration for the user.
        /// </summary>
        public TimeSpan Duration { get; set; } // Duration of the activity
        /// <summary>
        /// Gets or sets the Burned Calories for the user.
        /// </summary>
        public double CaloriesBurned { get; set; }
        /// <summary>
        /// Gets or sets the Avg Heartrate for the user.
        /// </summary>
        public int AverageHeartRate { get; set; }

        /// <summary>
        /// Calculates the estimated Burned Calories for the user.
        /// </summary>
        public void CalculateCaloriesBurned(int age, string gender, int? Weight)
        {
            if (Weight == null || AverageHeartRate == 0 || Duration == TimeSpan.Zero)
            {
                
                return;
            }

            double genderConstant = gender == "Male" ? 1 : 0.9;
            double durationInMinutes = Duration.TotalMinutes;

            CaloriesBurned = ((age * 0.2017 / genderConstant)
                              - (Weight.Value * 0.09036 / genderConstant) 
                              + (AverageHeartRate * 0.6309 / genderConstant)
                              - 55.0969) * durationInMinutes / 4.184;
        }



        /// <summary>
        /// Function to check the step goals for the user.
        /// </summary>
        public bool CheckStepGoal()
        {
            return Steps >= User.DailyStepGoal;
    
        }


        public enum WorkoutType
        {
            Running,
            Swimming,
            Cycling,
            Yoga
        }

        public enum IntensityLevel
        {
            Low,
            Medium,
            High
        }

    }


}
