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
    public class TargetsController : ControllerBase
    {
        private readonly ManagementOfMossadAgentsDbContext _context;
        private readonly ServiceMove _serviceMove = new ServiceMove();
        private readonly ServiceTarget _serviceTarget;

        public TargetsController(ManagementOfMossadAgentsDbContext context)
        {
            _context = context;
            _serviceTarget = new ServiceTarget(_context);
        }

        // קבלת רשימה של כל המטרות
        // GET: api/Targets
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TargetView>>> GetTargets()
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

            return Ok(targetView);
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
            var dictionaryMove = _serviceMove.MoveDictionary[location.Location];

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
            if (target.Status != Enum.TargetStatus.Status.LIVE.ToString())
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { error = "Target is did" });
            }
            // בדיקה שיש כבר הצבה של המטרה
            if (target.Location == null)
            {
                return StatusCode(
                    StatusCodes.Status400BadRequest,
                    new { error = "Target does not have a location" }
                );
            }
            // בדיקה האם המטרה כבר בקצה
            if (GeneralFunctions.IfMoveOutOfRange(target.Location, location))
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

            _context.SaveChanges();
            await _serviceTarget.CheckMissionsTarget(target);
            return target;
        }
    }
}
