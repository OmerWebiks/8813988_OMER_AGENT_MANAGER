using ManagementOfMossadAgentsAPI.Del;
using ManagementOfMossadAgentsAPI.Models;
using Microsoft.EntityFrameworkCore;

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
            double minDistance = 1000;
            Target thisTarget = null;

            var targets = await _context.Targets.Include(a => a.Location).ToListAsync();

            foreach (Target target in targets)
            {
                if (
                    target.Status == Enum.TargetStatus.Status.LIVE.ToString()
                    && target.Location != null
                )
                {
                    var distance = Math.Sqrt(
                        Math.Pow(agent.Location.X - target.Location.X, 2)
                            + Math.Pow(agent.Location.Y - target.Location.Y, 2)
                    );

                    if (distance <= 200)
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
