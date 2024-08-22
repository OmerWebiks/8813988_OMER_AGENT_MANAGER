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

        // עדכון מיקום של סוכן בהצבה הראשונה
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
    }
}
