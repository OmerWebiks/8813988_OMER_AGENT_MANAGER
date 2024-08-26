using System.ComponentModel.DataAnnotations;
using ManagementOfMossadAgentsAPI.api.Enum;

namespace ManagementOfMossadAgentsAPI.Models
{
    public class Target
    {
        [Key]
        public int? Id { get; set; }
        public string Name { get; set; }
        public string Position { get; set; }
        public Location? Location { get; set; }
        public string? Status { get; set; }
        public string PhotoUrl { get; set; }

        public Target()
        {
            Status = TargetStatus.Status.LIVE.ToString();
        }
    }
}
