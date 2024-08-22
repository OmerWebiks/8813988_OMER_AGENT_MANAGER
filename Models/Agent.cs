using System.ComponentModel.DataAnnotations;

namespace ManagementOfMossadAgentsAPI.Models
{
    public class Agent
    {
        [Key]
        public int Id { get; set; }
        public string Nickname { get; set; }
        public Location? Location { get; set; }
        public int X { get; set; } = 0;
        public int Y { get; set; } = 0;
        public string? Status { get; set; }
        public string PhotoUrl { get; set; }

        public Agent()
        {
            Status = Enum.AgentStatus.Status.DORMANT.ToString();
        }
    }
}
