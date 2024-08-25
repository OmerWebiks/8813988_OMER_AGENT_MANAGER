using ManagementOfMossadAgentsAPI.Del;
using ManagementOfMossadAgentsAPI.Enum;
using ManagementOfMossadAgentsAPI.Models;
using ManagementOfMossadAgentsAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ManagementOfMossadAgentsAPI.Controllers;

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
        return await _context
            .Missions.Include(a => a.Agent)
            .ThenInclude(l => l.Location)
            .Include(t => t.Target)
            .ThenInclude(l => l.Location)
            .ToListAsync();
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

        mission.Status = MissionStatus.Status.ASSIGNED.ToString();
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
