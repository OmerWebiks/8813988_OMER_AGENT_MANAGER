using ManagementOfMossadAgentsAPI.Del;
using ManagementOfMossadAgentsAPI.Enum;
using ManagementOfMossadAgentsAPI.Models;
using ManagementOfMossadAgentsAPI.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace ManagementOfMossadAgentsAPI.Services;

public class ServiceMission
{
    private readonly ManagementOfMossadAgentsDbContext _context;

    public ServiceMission(ManagementOfMossadAgentsDbContext context)
    {
        _context = context;
    }

    // פונקציה שמזיזה את הסוכן לכיון המטרה ומחזירה ערך בוליאני אם הצליחה לו לא
    public async Task<bool> MoveMission(Target target, Agent agent)
    {
        if (target != null && agent != null)
        {
            if (agent.Location.X > target.Location.X)
                agent.Location.X--;
            if (agent.Location.X < target.Location.X)
                agent.Location.X++;
            if (agent.Location.Y > target.Location.Y)
                agent.Location.Y--;
            if (agent.Location.Y < target.Location.Y)
                agent.Location.Y++;
            await _context.SaveChangesAsync();
            return true;
        }

        return false;
    }

    // פונקציה שבודקת האם הסוכן עדין בתווח של המטרה
    public async Task<bool> CheckProprietaryMission(Target target, Agent agent)
    {
        if (target != null && agent != null)
        {
            var distance = GeneralFunctions.Distance(target.Location, agent.Location);

            if (distance <= 200)
                return true;

            return false;
        }

        return false;
    }

    // פונקציה שבודקת האם היה כבר ציוות למשימה הזאת או שהמשימה הזאת כבר נגמרה
    public async Task<bool> CheckMissionIsEndedOrAssigned(Mission mission)
    {
        if (
            mission.Status == Enum.MissionStatus.Status.ENDED.ToString()
            || mission.Status == Enum.MissionStatus.Status.ASSIGNED.ToString()
        )
        {
            return false;
        }
        return true;
    }

    // בדיקה האם הסוכן והמטרה באותו משבצת
    public async Task IsInSameArea(Mission mission)
    {
        var target = _context
            .Targets.Include(t => t.Location)
            .FirstOrDefault(t => t.Id == mission.Target.Id);
        var agent = _context
            .Agents.Include(a => a.Location)
            .FirstOrDefault(a => a.Id == mission.Agent.Id);
        if (
            target.Location != null
            && agent.Location != null
            && GeneralFunctions.IsInSamePlace(target.Location, agent.Location)
        )
        {
            mission.Status = MissionStatus.Status.ENDED.ToString();
            _context.Missions.Update(mission);
            target.Status = TargetStatus.Status.DEAD.ToString();
            _context.Targets.Update(target);
            agent.Status = AgentStatus.Status.DORMANT.ToString();
            agent.CountLiquidations += 1;
            //TimeSpan timeSpan = DateTime.Now - mission.ExecutionTime;
            _context.Agents.Update(agent);

            await _context.SaveChangesAsync();
        }
    }

    // פונקציה שמקבלת סוכן ומטרה ומוחקת את כל המשימות שהיו בהצעה לציוות אותו סוכן ומטרה
    public async Task RemoveMission(Mission UpdatedMission)
    {
        var missions = _context.Missions.Include(a => a.Agent).Include(t => t.Target).ToList();

        if (missions != null)
        {
            foreach (var mission in missions)
            {
                if (mission.Target.Id == UpdatedMission.Target.Id)
                {
                    if (mission == UpdatedMission)
                    {
                        _context.Missions.Update(mission);
                        continue;
                    }
                    _context.Missions.Remove(mission);
                }
            }
            await _context.SaveChangesAsync();
        }

        //List<Mission>? missions = _context
        //    .Missions.Where(mission =>
        //        mission.Agent.Id == agent.Id && mission.Target.Id == target.Id
        //    )
        //    .ToList();
        //_context.Missions.RemoveRange(missions);
        await _context.SaveChangesAsync();
    }

    public async Task MoveMissionsToTarget()
    {
        var missions = await _context
            .Missions.Include(m => m.Target)
            .ThenInclude(t => t.Location)
            .Include(m => m.Agent)
            .ThenInclude(a => a.Location)
            .Where(m => m.Status == Enum.MissionStatus.Status.ASSIGNED.ToString())
            .ToListAsync();
        foreach (var mission in missions)
        {
            await MoveMission(mission.Target, mission.Agent);
            await IsInSameArea(mission);
        }
    }

    // פונקציה שמביאה את כל המשימות לציוות בשביל ה Mvc
    public async Task<List<TaskManagementForView>> GetMissionForView()
    {
        var missions = await _context
            .Missions.Include(a => a.Agent)
            .ThenInclude(l => l.Location)
            .Include(t => t.Target)
            .ThenInclude(l => l.Location)
            .Where(m => m.Status == MissionStatus.Status.PROPOSAL.ToString())
            .ToListAsync();

        List<TaskManagementForView> missionListForView = new List<TaskManagementForView>();

        foreach (var mission in missions)
        {
            missionListForView.Add(
                new TaskManagementForView
                {
                    IdMission = mission.Id,
                    AgentName = mission.Agent.Nickname,
                    XAgent = mission.Agent.Location.X,
                    YAgent = mission.Agent.Location.Y,
                    TargetName = mission.Target.Name,
                    XTarget = mission.Target.Location.X,
                    YTarget = mission.Target.Location.Y,
                    PositionTarget = mission.Target.Position,
                    TimeLeft = mission.TimeLeft,
                    Distance = GeneralFunctions.Distance(
                        mission.Target.Location,
                        mission.Agent.Location
                    ),
                }
            );
        }
        return missionListForView;
    }
}
