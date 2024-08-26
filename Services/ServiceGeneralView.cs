using ManagementOfMossadAgentsAPI.api.Del;
using ManagementOfMossadAgentsAPI.api.Enum;
using ManagementOfMossadAgentsAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ManagementOfMossadAgentsAPI.api.Services
{
    public class ServiceGeneralView
    {
        private readonly ManagementOfMossadAgentsDbContext _context;

        public ServiceGeneralView(ManagementOfMossadAgentsDbContext context)
        {
            _context = context;
        }

        public async Task<GeneralView> GetGeneralView()
        {
            GeneralView generalView = new GeneralView();
            generalView.CountAgent = _context.Agents.Count();
            generalView.CountTarget = _context.Targets.Count();
            generalView.CountMission = _context.Missions.Count();
            generalView.AgentActive = _context
                .Agents.Where(a => a.Status == AgentStatus.Status.IN_ACTIVITY.ToString())
                .Count();
            generalView.TargetLive = _context
                .Targets.Where(t => t.Status == TargetStatus.Status.LIVE.ToString())
                .Count();
            generalView.MissionActive = _context
                .Missions.Where(m => m.Status == MissionStatus.Status.ASSIGNED.ToString())
                .Count();
            try
            {
                generalView.RadioBetweenAgentToTarget =
                    generalView.CountAgent / generalView.CountTarget;
            }
            catch
            {
                generalView.RadioBetweenAgentToTarget = 0;
            }

            generalView.RadioBetweenAgentToMission = 50;

            return generalView;
        }
    }
}
