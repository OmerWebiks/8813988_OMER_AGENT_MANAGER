using System.Reflection;
using ManagementOfMossadAgentsAPI.api.Del;
using ManagementOfMossadAgentsAPI.api.Enum;
using ManagementOfMossadAgentsAPI.api.Utils;
using ManagementOfMossadAgentsAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ManagementOfMossadAgentsAPI.api.Services;

public class ServiceAgent
{
    private readonly ManagementOfMossadAgentsDbContext _context;
    private readonly GeneralFunctions _generalFunctions = new GeneralFunctions();

    public ServiceAgent(ManagementOfMossadAgentsDbContext context)
    {
        _context = context;
    }

    // פונקציה שעוברת על כל המטרות עבור סוכן אחד
    public async Task MissionCheckAgent(Agent agent)
    {
        // הבאת כל המטרות
        var targets = await _context.Targets.Include(t => t.Location).ToListAsync();

        foreach (var target in targets)
        {
            // בדיקה האם המטרה חייה
            if (target.Status == TargetStatus.Status.LIVE.ToString() && target.Location != null)
            {
                // בדיקה של המרחק בין הסוכן למטרה
                var distance = GeneralFunctions.Distance(target.Location, agent.Location);

                // בדיקה האם המרחק בין הסוכן למטרה פחות מ 200
                if (distance <= 200)
                {
                    // בדיקה שהמשימה הזאת כבר לא קיימת
                    Mission? mission = await _context.Missions.FirstOrDefaultAsync(p =>
                        p.Target.Id == target.Id && p.Agent.Id == agent.Id
                    );
                    if (mission == null)
                    {
                        var newMission = new Mission
                        {
                            Target = target,
                            Agent = agent,
                            TimeLeft = distance / 5.0
                        };
                        await _context.Missions.AddAsync(newMission);
                    }
                }
            }
            await _context.SaveChangesAsync();
        }
    }

    // פונקציה שבודקת את התקינות של ההצעות לסוכן לאחר שהוא זז
    public async Task CheckMoveAgent(Agent agent)
    {
        var missions = await _context
            .Missions.Include(a => a.Agent)
            .ThenInclude(l => l.Location)
            .Include(t => t.Target)
            .ThenInclude(l => l.Location)
            .Where(m => m.Agent.Id == agent.Id)
            .ToListAsync();
        if (missions != null)
        {
            foreach (var mission in missions)
            {
                double distance = GeneralFunctions.Distance(
                    mission.Target.Location,
                    mission.Agent.Location
                );
                if (distance > 200)
                {
                    _context.Missions.Remove(mission);
                    mission.TimeLeft = distance / 5.0;
                    await _context.SaveChangesAsync();
                }
            }
        }
        await MissionCheckAgent(agent);
    }

    //פונקציה שמחזירה את הנתונים של סוכן אחד עבור התצוגה
    public async Task<List<AgentForView>> GetAgentForView()
    {
        List<AgentForView> agentsForView = new List<AgentForView>();

        var agents = await _context.Agents.Include(l => l.Location).ToListAsync();
        if (agents != null)
        {
            foreach (var agent in agents)
            {
                AgentForView agentForView = new AgentForView();
                agentForView.id = agent.Id;
                agentForView.Nickname = agent.Nickname;
                agentForView.Status = agent.Status;
                if (agent.Location != null)
                {
                    agentForView.X = agent.Location.X;
                    agentForView.Y = agent.Location.Y;
                }
                agentForView.CountLiquidations = agent.CountLiquidations;

                Mission? mission = await _context.Missions.FirstOrDefaultAsync(m =>
                    m.Agent.Id == agent.Id && m.Status == MissionStatus.Status.ASSIGNED.ToString()
                );
                if (mission != null)
                {
                    agentForView.TimeLeft = mission.TimeLeft;
                    agentForView.MissionId = mission.Id;
                }
                agentsForView.Add(agentForView);
            }
        }
        return agentsForView;
    }
}
