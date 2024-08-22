using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManagementOfMossadAgentsAPI.Del;
using ManagementOfMossadAgentsAPI.Models;
using ManagementOfMossadAgentsAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ManagementOfMossadAgentsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AgentsController : ControllerBase
    {
        private readonly ManagementOfMossadAgentsDbContext _context;
        private readonly ServiceAgent _serviceAgent;

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
        public async Task<ActionResult<Agent>> PostAgent(Agent agent)
        {
            _context.Agents.Add(agent);
            await _context.SaveChangesAsync();

            return agent;
        }

        // קביעת מיקום התחלתי של הסוכן
        // PUT: api/Agents/{id}/pin
        [HttpPut("{id}/pin")]
        public async Task<ActionResult<Agent>> PutAgent(Guid? id, Location location)
        {
            return await UpdateLocationPin(id, location);
        }

        // עדכון מיקום של סוכן בהצבה הראשונה
        async Task<ActionResult<Agent>> UpdateLocationPin(Guid? id, Location location)
        {
            var agent = await _context
                .Agents.Include(a => a.Location)
                .FirstOrDefaultAsync(a => a.Id == id);
            if (agent == null)
            {
                return NotFound();
            }
            if (agent.Location == null)
            {
                agent.Location = location;
                _context.Locations.Add(location);
                _context.SaveChanges();
                await _serviceAgent.MissionCheckAgent(agent);
            }
            else
            {
                return StatusCode(
                    StatusCodes.Status400BadRequest,
                    new { error = "Agent already has a location." }
                );
            }

            return agent;
        }

        // הזזת סוכן למיקום מסויים
        //// PUT: api/Agents/{id}/move
        [HttpPut("{id}/move")]
        public async Task<ActionResult<Agent>> PutAgent(Guid? id, ServiceMove location)
        {
            ServiceMove serviceMove = new ServiceMove();
            var dictionaryMove = serviceMove.MoveDictionary[location.Location];

            Location loc = new Location(dictionaryMove[0], dictionaryMove[1]);
            // שליחה לפונקציה שמעדכנת את המיקום
            return await UpdateLocationMove(id, loc);
        }

        // עדכון מיקום של סוכן
        async Task<ActionResult<Agent>> UpdateLocationMove(Guid? id, Location location)
        {
            var agent = await _context
                .Agents.Include(a => a.Location)
                .FirstOrDefaultAsync(a => a.Id == id);
            if (agent == null)
            {
                return NotFound();
            }
            if (agent.Status != Enum.AgentStatus.Status.IN_ACTIVITY.ToString())
            {
                if (agent.Location == null)
                {
                    return StatusCode(
                        StatusCodes.Status400BadRequest,
                        new { error = "Agent does not have a location" }
                    );
                }
                else
                {
                    agent.Location.x += location.x;
                    agent.Location.y += location.y;
                }
                _context.SaveChanges();

                return agent;
            }
            return StatusCode(
                StatusCodes.Status400BadRequest,
                new { error = "Agent is in activity" }
            );
        }

        //// PUT: api/Agents/5
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutAgent(Guid? id, Agent agent)
        //{
        //    if (id != agent.Id)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(agent).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!AgentExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}

        //// DELETE: api/Agents/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteAgent(Guid? id)
        //{
        //    var agent = await _context.Agents.FindAsync(id);
        //    if (agent == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.Agents.Remove(agent);
        //    await _context.SaveChangesAsync();

        //    return NoContent();
        //}

        private bool AgentExists(Guid? id)
        {
            return _context.Agents.Any(e => e.Id == id);
        }
    }
}
