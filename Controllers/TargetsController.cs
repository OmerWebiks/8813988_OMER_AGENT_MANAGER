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
    [Route("[controller]")]
    [ApiController]
    public class TargetsController : ControllerBase
    {
        private readonly ManagementOfMossadAgentsDbContext _context;

        public TargetsController(ManagementOfMossadAgentsDbContext context)
        {
            _context = context;
        }

        // קבלת רשימה של כל המטרות
        // GET: api/Targets
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Target>>> GetTargets()
        {
            return await _context.Targets.ToListAsync();
        }

        // יצירת מטרה חדשה
        // POST: api/Targets
        [HttpPost]
        public async Task<ActionResult<Target>> PostTarget(Target target)
        {
            var result = _context.Targets.Add(target);
            await _context.SaveChangesAsync();

            return StatusCode(StatusCodes.Status201Created, new { id = result.Entity.Id });
        }

        // קביעת מיקום התחלתי של המטרה
        // PUT: api/Targets/{id}/pin
        [HttpPut("{id}/pin")]
        public async Task<ActionResult<Target>> PutTarget(int id, Location location)
        {
            return await UpdateLocationPin(id, location);
        }

        // עדכון מיקום של המטרה בהצבה הראשונה
        async Task<ActionResult<Target>> UpdateLocationPin(int id, Location location)
        {
            var target = await _context
                .Targets.Include(a => a.Location)
                .FirstOrDefaultAsync(a => a.Id == id);
            if (target == null)
            {
                return NotFound();
            }
            if (target.Location == null)
            {
                target.Location = location;
                _context.Locations.Add(location);
                _context.SaveChanges();
            }
            else
            {
                return StatusCode(
                    StatusCodes.Status400BadRequest,
                    new { error = "Agent already has a location." }
                );
            }

            return target;
        }

        // הזזת מטרה למיקום מסויים
        //// PUT: api/Targets/{id}/move
        [HttpPut("{id}/move")]
        public async Task<ActionResult<Target>> PutAgent(int id, ServiceMove location)
        {
            ServiceMove serviceMove = new ServiceMove();
            var dictionaryMove = serviceMove.MoveDictionary[location.Location];

            Location locationDict = new Location(dictionaryMove[0], dictionaryMove[1]);
            // שליחה לפונקציה שמעדכנת את המיקום
            return await UpdateLocationMove(id, locationDict);
        }

        // עדכון מיקום של מטרה
        async Task<ActionResult<Target>> UpdateLocationMove(int id, Location location)
        {
            // הבאת המקרה עם המיקום
            var target = await _context
                .Targets.Include(a => a.Location)
                .FirstOrDefaultAsync(a => a.Id == id);
            // בדיקה שהמטרה קיימת
            if (target == null)
            {
                return NotFound();
            }
            // בדיקה שהמטרה חייה
            if (target.Status == Enum.TargetStatus.Status.LIVE.ToString())
            {
                // בדיקה שיש כבר הצבה של המטרה
                if (target.Location == null)
                {
                    return StatusCode(
                        StatusCodes.Status400BadRequest,
                        new { error = "Target does not have a location" }
                    );
                }
                else
                {
                    // בדיקה האם המטרה כבר בקצה
                    if (
                        target.Location.X + location.X > 1000
                        || target.Location.Y + location.Y > 1000
                    )
                        return StatusCode(
                            StatusCodes.Status400BadRequest,
                            new
                            {
                                error = "It is not possible to move the target in this direction",
                                location = target.Location
                            }
                        );
                    target.Location.X += location.X;
                    target.Location.Y += location.Y;
                }
                _context.SaveChanges();

                return target;
            }
            return StatusCode(StatusCodes.Status400BadRequest, new { error = "Target is did" });
        }
    }
}
