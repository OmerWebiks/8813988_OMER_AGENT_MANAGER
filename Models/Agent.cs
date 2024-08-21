using System.ComponentModel.DataAnnotations;

namespace ManagementOfMossadAgentsAPI.Models
{
    public class Agent
    {
        [Key]
        public Guid? Id { get; set; }
        public string Nickname { get; set; }
        public Location? Location { get; set; }

        public string? Status { get; set; }
        public string Image { get; set; }

        public Agent()
        {
            Id = Guid.NewGuid();
            Status = Enum.AgentStatus.Status.DORMANT.ToString();
        }
    }
}
