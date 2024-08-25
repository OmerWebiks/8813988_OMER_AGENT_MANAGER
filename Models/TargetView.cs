namespace ManagementOfMossadAgentsAPI.Models
{
    public class TargetView
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string Position { get; set; }
        public int X { get; set; } = 0;
        public int Y { get; set; } = 0;
        public string? Status { get; set; }
        public string PhotoUrl { get; set; }
    }
}
