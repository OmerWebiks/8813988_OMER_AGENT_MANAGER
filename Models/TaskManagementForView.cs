namespace ManagementOfMossadAgentsAPI.Models
{
    public class TaskManagementForView
    {
        public int IdMission { get; set; }
        public string AgentName { get; set; }
        public int XAgent { get; set; }
        public int YAgent { get; set; }
        public string TargetName { get; set; }
        public string PositionTarget { get; set; }
        public int XTarget { get; set; }
        public int YTarget { get; set; }
        public double TimeLeft { get; set; }
        public double Distance { get; set; }
    }
}
