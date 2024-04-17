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

        public int Steps { get; set; }
        public string ActivityType { get; set; }
        public double Distance { get; set; }
    }
}