using System.ComponentModel.DataAnnotations;

namespace ManagementOfMossadAgentsAPI.Models
{
    public class Agent
    {
        [Key]
        public Guid? Id { get; set; }
        public string Nickname { get; set; }
        public Location? Location { get; set; }

        // מגדיר קבוע של סטטוס
        [AllowedValues(typeof(Enum.AgentStatus.Status))]
        public string? Status { get; set; }
        public string Image { get; set; }
    }
}
