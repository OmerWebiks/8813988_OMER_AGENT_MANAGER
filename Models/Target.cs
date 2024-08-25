using System.ComponentModel.DataAnnotations;

namespace ManagementOfMossadAgentsAPI.Models
{
    public class Target
    {
        [Key]
        public int? Id { get; set; }
        public string Name { get; set; }
        public string Position { get; set; }
        public Location? Location { get; set; }
        public int X { get; set; } = 0;
        public int Y { get; set; } = 0;
        public string? Status { get; set; }
        public string PhotoUrl { get; set; }

        public Target()
        {
            Status = Enum.TargetStatus.Status.LIVE.ToString();
        }
    }
}
