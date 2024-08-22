using ManagementOfMossadAgentsAPI.Del;
using ManagementOfMossadAgentsAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ManagementOfMossadAgentsAPI.Services
{
    public class ServiceAgent
    {
        private readonly ManagementOfMossadAgentsDbContext _context;

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

            foreach (Target target in targets)
            {
                // בדיקה האם המטרה חייה
                if (
                    target.Status == Enum.TargetStatus.Status.LIVE.ToString()
                    && target.Location != null
                )
                {
                    // בדיקה של המרחק בין הסוכן למטרה
                    var distance = Math.Sqrt(
                        Math.Pow(agent.Location.X - target.Location.X, 2)
                            + Math.Pow(agent.Location.Y - target.Location.Y, 2)
                    );
                    // בדיקה האם המרחק בין הסוכן למטרה פחות מ 200
                    if (distance <= 200)
                    {
                        if (distance <= minDistance)
                        {
                            minDistance = distance;
                            thisTarget = target;
                        }
                    }
                }
            }
            // בדיקה האם נמצאה מטרה שעומדת בקרטריונים של ההצעות למשימה
            if (thisTarget != null)
            {
                // בדיקה האם יש כבר הצעה למשימה לאותו סוכן וא"כ להביא אותה
                Mission? mission = _context.Missions.FirstOrDefault(mission =>
                    mission.Agent.Id == agent.Id
                );

                if (mission == null)
                {
                    // הגדרה שאם אין הצעה למשימה להגדיר את ההצעה הנוכחית
                    mission = new Mission { Agent = agent, Target = thisTarget };
                    _context.Missions.Add(mission);
                }
                else
                {
                    // אם יש הצעה של משימה כבר לעדכן אותה להצעה החדשה
                    mission.Target = thisTarget;
                    _context.Missions.Update(mission);
                }

                await _context.SaveChangesAsync();
            }
        }
    }
}
