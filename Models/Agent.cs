using System.ComponentModel.DataAnnotations;
using ManagementOfMossadAgentsAPI.api.Enum;

namespace ManagementOfMossadAgentsAPI.Models
{
    public class Agent
    {
        [Key]
        public int Id { get; set; }
        public string Nickname { get; set; }
        public Location? Location { get; set; }
        public string? Status { get; set; }
        public string PhotoUrl { get; set; }
        public int CountLiquidations { get; set; } = 0;

        public Agent()
        {
            Status = AgentStatus.Status.DORMANT.ToString();
        }
    }
}
