namespace ManagementOfMossadAgentsAPI.Models
{
    public class AgentForView
    {
        public int? id { get; set; }
        public string Nickname { get; set; }
        public int X { get; set; } = 0;
        public int Y { get; set; } = 0;
        public string Status { get; set; }
        public double TimeLeft { get; set; }
        public int CountLiquidations { get; set; } = 0;
        public int MissionId { get; set; } = 0;
    }
}
