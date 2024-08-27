using ManagementOfMossadAgentsAPI.api.Del;
using ManagementOfMossadAgentsAPI.api.Enum;
using ManagementOfMossadAgentsAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ManagementOfMossadAgentsAPI.api.Utils;

public class GeneralFunctions
{
    private readonly ManagementOfMossadAgentsDbContext _context;

    public GeneralFunctions(ManagementOfMossadAgentsDbContext context)
    {
        _context = context;
    }

    // פונקציה שבודקת את המרחק בין סוכן למטרה
    public static double Distance(Location locationTarget, Location locationAgent)
    {
        return Math.Sqrt(
            Math.Pow(locationTarget.X - locationAgent.X, 2)
                + Math.Pow(locationTarget.Y - locationAgent.Y, 2)
        );
    }

    // פונקציה שבודקת האם הסוכן והמטרה באותו משבצת
    public static bool IsInSamePlace(Location locationOne, Location locationTwo)
    {
        if (locationOne.X == locationTwo.X && locationOne.Y == locationTwo.Y)
            return true;
        return false;
    }

    // פונקציה שבודקת האם הסוכן או המטרה כבר נמצאים בקצה
    public static bool IfMoveOutOfRange(Location Location, Location move)
    {
        return Location.X + move.X > 1000
            || Location.Y + move.Y > 1000
            || Location.X + move.X < 0
            || Location.Y + move.Y < 0;
    }

    // פונקציה לבדיקה כמה זמן נשאר למשימה
    public async Task<double> TimeLeft(Location locationAgent, Location locationTarget)
    {
        double distance = Distance(locationTarget, locationAgent);
        return distance / 5;
    }

    // פונקציה שבודקת האם סוכן ומטרה מתאים וא"כ מוסיפה אותם
    public async Task<bool> CheckAndAddMission(Target target, Agent agent)
    {
        if (
            agent.Status == AgentStatus.Status.DORMANT.ToString()
            && agent.Location != null
            && target.Location != null
            && target.Status == TargetStatus.Status.LIVE.ToString()
        )
        {
            var distance = Distance(target.Location, agent.Location);
            if (distance <= 200)
            {
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
                    return true;
                }
            }
        }
        await _context.SaveChangesAsync();
        return false;
    }

    // פונקציה שמקבלת סוכן ומטרה ובודקת האם האם ההצעה עדין תקינה
    public async Task RemoveInvalidMissions(Target target, Agent agent)
    {
        var missions = await _context
            .Missions.Include(a => a.Agent)
            .ThenInclude(l => l.Location)
            .Include(t => t.Target)
            .ThenInclude(l => l.Location)
            .Where(m => m.Target.Id == target.Id && m.Agent.Id == agent.Id)
            .ToListAsync();

        foreach (var mission in missions)
        {
            double distance = Distance(mission.Target.Location, mission.Agent.Location);
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
    }
}
