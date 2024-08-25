using ManagementOfMossadAgentsAPI.Del;
using ManagementOfMossadAgentsAPI.Models;
using ManagementOfMossadAgentsAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ManagementOfMossadAgentsAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class GeneralViewController : ControllerBase
    {
        private readonly ManagementOfMossadAgentsDbContext _context;

        public GeneralViewController(ManagementOfMossadAgentsDbContext context)
        {
            _context = context;
        }

        [HttpGet("GetGeneral")]
        public async Task<ActionResult<GeneralView>> GetGeneralView()
        {
            GeneralView generalView = new GeneralView();
            generalView.CountAgent = _context.Agents.Count();
            generalView.CountTarget = _context.Targets.Count();
            generalView.CountMission = _context.Missions.Count();
            generalView.AgentActive = _context
                .Agents.Where(a => a.Status == Enum.AgentStatus.Status.IN_ACTIVITY.ToString())
                .Count();
            generalView.TargetLive = _context
                .Targets.Where(t => t.Status == Enum.TargetStatus.Status.LIVE.ToString())
                .Count();
            generalView.MissionActive = _context
                .Missions.Where(m => m.Status == Enum.MissionStatus.Status.ASSIGNED.ToString())
                .Count();
            generalView.RadioBetweenAgentToTarget =
                generalView.CountAgent / generalView.CountTarget;
            generalView.RadioBetweenAgentToMission = 50;
            return Ok(generalView);
        }
    }
}
