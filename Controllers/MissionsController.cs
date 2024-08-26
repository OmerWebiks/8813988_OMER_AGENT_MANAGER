using ManagementOfMossadAgentsAPI.api.Del;
using ManagementOfMossadAgentsAPI.api.Enum;
using ManagementOfMossadAgentsAPI.api.Services;
using ManagementOfMossadAgentsAPI.api.Utils;
using ManagementOfMossadAgentsAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ManagementOfMossadAgentsAPI.api.Controllers;

[Route("[controller]")]
[ApiController]
public class MissionsController : ControllerBase
{
    private readonly ManagementOfMossadAgentsDbContext _context;
    private readonly ServiceMission _serviceMission;

    public MissionsController(ManagementOfMossadAgentsDbContext context)
    {
        _context = context;
        _serviceMission = new ServiceMission(_context);
    }

    // פונקציה שמחזירה את כל המשימות
    // GET: Missions
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Mission>>> GetMissions()
    {
        List<TaskManagementForView> MissionForView = await _serviceMission.GetMissionForView();

        return Ok(MissionForView);
    }

    // פונקציה לצוות סוכן למשימה
    // מנהל בקרה בלבד
    // PUT: Missions/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> PutMission(int id)
    {
        var mission = await _context
            .Missions.Include(t => t.Agent)
            .Include(t => t.Target)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (mission == null)
            return NotFound();
        // בדיקה האם הסוכן נמצא עדין בתווח של המטרה
        return await CheckMission(mission);
    }

    // פונקציה שבודקת את התקינות של המשימה
    private async Task<IActionResult> CheckMission(Mission mission)
    {
        var agent = await _context
            .Agents.Include(a => a.Location)
            .FirstOrDefaultAsync(a => a.Id == mission.Agent.Id);
        var target = await _context
            .Targets.Include(t => t.Location)
            .FirstOrDefaultAsync(t => t.Id == mission.Target.Id);
        if (await _serviceMission.CheckProprietaryMission(target, agent) == false)
        {
            _context.Missions.Remove(mission);
            await _context.SaveChangesAsync();
            return StatusCode(400, "The target is not near");
        }
        if (!await _serviceMission.CheckMissionIsEndedOrAssigned(mission))
        {
            return StatusCode(400, "The mission is ended or assigned");
        }

        mission.Status = MissionStatus.Status.ASSIGNED.ToString();
        //mission.ExecutionTime = TimeSpan
        agent.Status = AgentStatus.Status.IN_ACTIVITY.ToString();
        await _context.SaveChangesAsync();

        await _serviceMission.RemoveMission(mission);
        return StatusCode(201, mission);
    }

    // POST: Missions/Update
    // שרת סימולציה
    [HttpPost("Update")]
    public async Task PutMission()
    {
        await _serviceMission.MoveMissionsToTarget();
    }
}
