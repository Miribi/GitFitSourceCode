using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace GitFitProj.Model
{
    public class ActivityModel
    {
        /// <summary>
        /// Gets or sets the unique identifier for the user.
        /// </summary>
        [Key]
        public int LogId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the user.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the user.
        /// </summary>
        [ForeignKey("UserId")]
        public UserModel User { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the user.
        /// </summary>
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime ActivityDate { get; set; }
        /// <summary>
        /// Gets or sets the unique identifier for the user.
        /// </summary>
        public int? Weight { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Steps must be over Zero.")]
        /// <summary>
        /// Gets or sets the unique identifier for the user. 
        /// </summary>
        public int Steps { get; set; }
        public string ActivityType { get; set; } // e.g., Running, Walking, Cycling
        /// <summary>
        /// Gets or sets the unique identifier for the user.
        /// </summary>
        public double Distance { get; set; } // in kilometers or miles
        /// <summary>
        /// Gets or sets the unique identifier for the user.
        /// </summary>
        public string Intensity { get; set; }
        /// <summary>
        /// Gets or sets the unique identifier for the user.
        /// </summary>
        public TimeSpan Duration { get; set; } // Duration of the activity
        /// <summary>
        /// Gets or sets the unique identifier for the user.
        /// </summary>
        public double CaloriesBurned { get; set; } // Estimated calories burned
        /// <summary>
        /// Gets or sets the unique identifier for the user.
        /// </summary>
        public int AverageHeartRate { get; set; } // Average during the activity

        /// <summary>
        /// Gets or sets the unique identifier for the user.
        /// </summary>
        public void CalculateCaloriesBurned(int age, string gender, int? Weight)
        {
            if (Weight == null || AverageHeartRate == 0 || Duration == TimeSpan.Zero)
            {
                // Handle the null or zero case appropriately.
                return;
            }

            double genderConstant = gender == "Male" ? 1 : 0.9;
            double durationInMinutes = Duration.TotalMinutes;

            CaloriesBurned = ((age * 0.2017 / genderConstant)
                              - (Weight.Value * 0.09036 / genderConstant) // Notice the use of Value here
                              + (AverageHeartRate * 0.6309 / genderConstant)
                              - 55.0969) * durationInMinutes / 4.184;
        }



        /// <summary>
        /// Gets or sets the unique identifier for the user.
        /// </summary>
        public bool CheckStepGoal()
        {
            return Steps >= User.DailyStepGoal;
        }
    }

}
