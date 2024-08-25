using System.Reflection;
using ManagementOfMossadAgentsAPI.Del;
using ManagementOfMossadAgentsAPI.Enum;
using ManagementOfMossadAgentsAPI.Models;
using ManagementOfMossadAgentsAPI.Utils;
using Microsoft.EntityFrameworkCore;

namespace ManagementOfMossadAgentsAPI.Services;

public class ServiceTarget
{
    private readonly ManagementOfMossadAgentsDbContext _context;

    public ServiceTarget(ManagementOfMossadAgentsDbContext context)
    {
        _context = context;
    }

    // פונקציה שעוברת על כל הסוכנים עבור מטרה אחת
    public async Task MissionCheckTarget(Target target)
    {
        double minDistance = 1000;
        Agent thisAgent = null;

        // הבאת כל הסוכנים
        var agents = await _context.Agents.Include(t => t.Location).ToListAsync();

        foreach (var agent in agents)
            // בדיקה האם הסוכן רדום והוא הוצב במיקום
            if (agent.Status == AgentStatus.Status.IN_ACTIVITY.ToString() && agent.Location != null)
            {
                // בדיקה של המרחק בין המטרה לסוכן
                var distance = CalculateDistanceToTarget.Distance(target, agent);
                // בדיקה האם המרחק בין הסוכן למטרה פחות מ 200
                if (distance <= 200)
                    if (distance <= minDistance)
                    {
                        minDistance = distance;
                        thisAgent = agent;
                    }
            }

        // בדיקה האם נמצא סוכן שעומד בקרטריונים של ההצעות למשימה
        if (thisAgent != null)
        {
            // הוספת ההצעה
            var newMission = new Mission { Target = target, Agent = thisAgent };
            _context.Missions.Add(newMission);
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
                double distance = CalculateDistanceToTarget.Distance(mission.Target, mission.Agent);
                if (distance > 200)
                {
                    _context.Missions.Remove(mission);
                }
            }
            await _context.SaveChangesAsync();
            await MissionCheckTarget(target);
        }
    }
}
