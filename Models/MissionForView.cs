namespace ManagementOfMossadAgentsAPI.Models
{
    public class MissionForView
    {
        public int Id { get; set; }
        public string Nickname { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public string TargetName { get; set; }
        public int TargetX { get; set; }
        public int TargetY { get; set; }
        public double TimeLeft { get; set; }
        public string Status { get; set; }
    }
}
