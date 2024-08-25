namespace ManagementOfMossadAgentsAPI.Models
{
    public class GeneralView
    {
        public int CountAgent { get; set; }
        public int AgentActive { get; set; }
        public int CountTarget { get; set; }
        public int TargetLive { get; set; }
        public int CountMission { get; set; }
        public int MissionActive { get; set; }
        public Double RadioBetweenAgentToTarget { get; set; }
        public Double RadioBetweenAgentToMission { get; set; }
    }
}
