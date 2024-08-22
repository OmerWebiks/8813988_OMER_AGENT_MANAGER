using ManagementOfMossadAgentsAPI.Del;
using ManagementOfMossadAgentsAPI.Models;

namespace ManagementOfMossadAgentsAPI.Services
{
    public class ServiceAgent
    {
        private readonly ManagementOfMossadAgentsDbContext _context;

        public ServiceAgent(ManagementOfMossadAgentsDbContext context)
        {
            _context = context;
        }

        // פונקציה שעוברת על כל המטרות עבור סוכן אחד
        public async Task MissionCheckAgent(Agent agent)
        {
            double minDistance = 0;
            Target thisTarget = null;

            var targets = _context.Targets.ToList();

            foreach (var target in targets)
            {
                var distance = Math.Sqrt(
                    Math.Pow(agent.Location.x - target.Location.x, 2)
                        + Math.Pow(agent.Location.y - target.Location.y, 2)
                );

                if (distance <= 200)
                {
                    if (target.Status == Enum.TargetStatus.Status.LIVE.ToString())
                    {
                        if (distance <= minDistance)
                        {
                            minDistance = distance;
                            thisTarget = target;
                        }
                    }
                }
            }
            if (thisTarget != null)
            {
                Mission? mission = _context.Missions.FirstOrDefault(mission =>
                    mission.Agent.Id == agent.Id
                );

                if (mission == null)
                {
                    mission = new Mission { Agent = agent, Target = thisTarget };
                    _context.Missions.Add(mission);
                }
                else
                {
                    mission.Target = thisTarget;
                    _context.Missions.Update(mission);
                }

                await _context.SaveChangesAsync();
            }
        }
    }
}
