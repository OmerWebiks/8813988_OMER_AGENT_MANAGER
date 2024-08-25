using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManagementOfMossadAgentsAPI.Del;
using ManagementOfMossadAgentsAPI.Models;
using ManagementOfMossadAgentsAPI.Services;
using ManagementOfMossadAgentsAPI.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ManagementOfMossadAgentsAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AgentsController : ControllerBase
    {
        private readonly ManagementOfMossadAgentsDbContext _context;
        private readonly ServiceAgent _serviceAgent;
        private readonly ServiceMove _serviceMove = new ServiceMove();

        public AgentsController(ManagementOfMossadAgentsDbContext context)
        {
            _context = context;
            _serviceAgent = new ServiceAgent(_context);
        }

        // GET: api/Agents
        // קבלת רשימה של כל הסוכנים
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Agent>>> GetAgents()
        {
            return await _context.Agents.Include(a => a.Location).ToListAsync();
        }

        // יצירת סוכן חדש
        // POST: api/Agents
        [HttpPost]
        public async Task<IActionResult> PostAgent(Agent agent)
        {
            var result = _context.Agents.Add(agent);
            await _context.SaveChangesAsync();

            return StatusCode(StatusCodes.Status201Created, new { id = result.Entity.Id });
        }

        // קביעת מיקום התחלתי של הסוכן
        // PUT: api/Agents/{id}/pin
        [HttpPut("{id}/pin")]
        public async Task<ActionResult<Agent>> PutAgent(int id, Location location)
        {
            return await UpdateLocationPin(id, location);
        }

        // עדכון מיקום של סוכן בהצבה הראשונה
        async Task<ActionResult<Agent>> UpdateLocationPin(int id, Location location)
        {
            // הבאת הסוכן עם המיקום
            var agent = await _context
                .Agents.Include(a => a.Location)
                .FirstOrDefaultAsync(a => a.Id == id);
            // בדיקה האם נמצא הסוכן
            if (agent == null)
            {
                return NotFound();
            }
            if (agent.Location != null)
                return StatusCode(
                    StatusCodes.Status400BadRequest,
                    new { error = "Agent already has a location." }
                );

            agent.Location = location;
            _context.Locations.Add(location);
            _context.SaveChanges();
            await _serviceAgent.MissionCheckAgent(agent);

            return agent;
        }

        // הזזת סוכן למיקום מסויים
        //// PUT: api/Agents/{id}/move
        [HttpPut("{id}/move")]
        public async Task<ActionResult<Agent>> PutAgent(int id, ServiceMove location)
        {
            var dictionaryMove = _serviceMove.MoveDictionary[location.Location];

            Location loc = new Location(dictionaryMove[0], dictionaryMove[1]);
            // שליחה לפונקציה שמעדכנת את המיקום
            return await UpdateLocationMove(id, loc);
        }

        // עדכון מיקום של סוכן
        async Task<ActionResult<Agent>> UpdateLocationMove(int id, Location location)
        {
            // הבאת הסוכן עם המיקום
            var agent = await _context
                .Agents.Include(a => a.Location)
                .FirstOrDefaultAsync(a => a.Id == id);
            if (agent == null)
                return NotFound();
            // בדיקה האם הסוכן לא במשימה כרגע
            if (agent.Status == Enum.AgentStatus.Status.IN_ACTIVITY.ToString())
            {
                return StatusCode(
                    StatusCodes.Status400BadRequest,
                    new { error = "Agent is in activity" }
                );
            }

            // לבדוק שנתנו לו הצבה ראשונה
            if (agent.Location == null)
            {
                return StatusCode(
                    StatusCodes.Status400BadRequest,
                    new { error = "Agent does not have a location" }
                );
            }
            // בדיקה האם הסוכן כבר בקצה
            if (CalculateDistanceToTarget.IfMoveOutOfRange(agent.Location, location))
                return StatusCode(
                    400,
                    new
                    {
                        error = "It is not possible to move the agent in this direction",
                        location = agent.Location
                    }
                );
            agent.Location.X += location.X;
            agent.Location.Y += location.Y;

            _context.SaveChanges();
            await _serviceAgent.CheckMoveAgent(agent);
            return agent;
        }
    }
}
