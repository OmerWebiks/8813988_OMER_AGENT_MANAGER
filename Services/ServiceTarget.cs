using System.Reflection;
using ManagementOfMossadAgentsAPI.api.Del;
using ManagementOfMossadAgentsAPI.api.Enum;
using ManagementOfMossadAgentsAPI.api.Utils;
using ManagementOfMossadAgentsAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ManagementOfMossadAgentsAPI.api.Services;

public class ServiceTarget
{
    private readonly ManagementOfMossadAgentsDbContext _context;
    private readonly GeneralFunctions _generalFunctions = new GeneralFunctions();

    public ServiceTarget(ManagementOfMossadAgentsDbContext context)
    {
        _context = context;
    }

    // פונקציה שעוברת על כל הסוכנים עבור מטרה אחת
    public async Task MissionCheckTarget(Target target)
    {
        // הבאת כל הסוכנים
        var agents = await _context.Agents.Include(t => t.Location).ToListAsync();

        foreach (var agent in agents)
        {
            // בדיקה האם הסוכן רדום והוא הוצב במיקום
            if (agent.Status == AgentStatus.Status.DORMANT.ToString() && agent.Location != null)
            {
                // בדיקה של המרחק בין המטרה לסוכן
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
        }
        await _context.SaveChangesAsync();
    }

    // פונקציה שבודקת האם ההצעות של המטרה עדין תקינות
    public async Task CheckMissionsTarget(Target target)
    {
        var missions = await _context
            .Missions.Include(a => a.Agent)
            .ThenInclude(l => l.Location)
            .Include(t => t.Target)
            .ThenInclude(l => l.Location)
            .Where(m => m.Target.Id == target.Id)
            .ToListAsync();
        if (missions != null)
        {
            foreach (Mission mission in missions)
            {
                double distance = GeneralFunctions.Distance(
                    mission.Target.Location,
                    mission.Agent.Location
                );
                if (distance > 200)
                {
                    _context.Missions.Remove(mission);
                }
                else
                {
                    mission.TimeLeft = distance / 5.0;
                }
            }
            await _context.SaveChangesAsync();
            await MissionCheckTarget(target);
        }
    }

    // פונקציה שמביאה את כל המטרות
    public async Task<List<TargetView>> GetTargets()
    {
        var targets = await _context.Targets.Include(t => t.Location).ToListAsync();
        List<TargetView> targetView = new List<TargetView>();
        foreach (var target in targets)
        {
            targetView.Add(
                new TargetView
                {
                    Id = target.Id,
                    Name = target.Name,
                    Position = target.Position,
                    Status = target.Status,
                    X = target.Location.X,
                    Y = target.Location.Y,
                    PhotoUrl = target.PhotoUrl
                }
            );
        }
        return targetView;
    }
}
