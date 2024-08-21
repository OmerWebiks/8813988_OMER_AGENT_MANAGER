using System.ComponentModel.DataAnnotations;

namespace ManagementOfMossadAgentsAPI.Models
{
    public class Mission
    {
        [Key]
        public Guid? Id { get; set; }
        public Agent Agent { get; set; }
        public Target Target { get; set; }
        public float TimeLeft { get; set; }
        public float? ExecutionTime { get; set; }

        // מגדיר קבוע של סטטוס
        [AllowedValues(typeof(Enum.MissionStatus.Status))]
        public string Status { get; set; }
    }
}
