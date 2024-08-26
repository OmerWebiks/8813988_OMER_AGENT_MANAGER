using System.ComponentModel.DataAnnotations;
using ManagementOfMossadAgentsAPI.api.Enum;

namespace ManagementOfMossadAgentsAPI.Models
{
    public class Mission
    {
        [Key]
        public int Id { get; set; }
        public Agent Agent { get; set; }
        public Target Target { get; set; }
        public double TimeLeft { get; set; } = 0;
        public TimeSpan ExecutionTime { get; set; }

        //// מגדיר קבוע של סטטוס
        //[AllowedValues(typeof(Enum.MissionStatus.Status))]
        public string Status { get; set; }

        public Mission()
        {
            Status = MissionStatus.Status.PROPOSAL.ToString();
        }
    }
}
