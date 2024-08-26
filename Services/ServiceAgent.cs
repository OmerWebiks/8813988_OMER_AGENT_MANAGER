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
        double minDistance = 1000;
        Target thisTarget = null;

        // הבאת כל המטרות
        var targets = await _context.Targets.Include(t => t.Location).ToListAsync();

        foreach (var target in targets)
            // בדיקה האם המטרה חייה
            if (target.Status == TargetStatus.Status.LIVE.ToString() && target.Location != null)
            {
                // בדיקה של המרחק בין הסוכן למטרה
                var distance = GeneralFunctions.Distance(target.Location, agent.Location);

                // בדיקה האם המרחק בין הסוכן למטרה פחות מ 200
                if (distance <= 200)
                    if (distance <= minDistance)
                    {
                        minDistance = distance;
                        thisTarget = target;
                    }
            }

        // בדיקה האם נמצאה מטרה שעומדת בקרטריונים של ההצעות למשימה
        if (thisTarget != null)
        {
            // בדיקה האם יש כבר הצעה למשימה לאותו סוכן וא"כ להביא אותה
            var mission = _context.Missions.FirstOrDefault(mission => mission.Agent.Id == agent.Id);

            if (mission == null)
            {
                // הגדרה שאם אין הצעה למשימה להגדיר את ההצעה הנוכחית
                mission = new Mission { Agent = agent, Target = thisTarget };
                _context.Missions.Add(mission);
            }
            else
            {
                // אם יש הצעה של משימה כבר לעדכן אותה להצעה החדשה
                //mission.Target = thisTarget;
                //mission.TimeLeft = await _generalFunctions.TimeLeft(
                //    mission.Agent.Location,
                //    mission.Target.Location
                //);
                _context.Missions.Update(mission);
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
                    _context.SaveChanges();
                }
            }
        }
        await MissionCheckAgent(agent);
    }
}
