using System.Reflection;
using ManagementOfMossadAgentsAPI.Del;
using ManagementOfMossadAgentsAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ManagementOfMossadAgentsAPI.Services
{
    public class ServiceTarget
    {
        private readonly ManagementOfMossadAgentsDbContext _context;

        public ServiceTarget(ManagementOfMossadAgentsDbContext context)
        {
            _context = context;
        }

        // פונקציה שעוברת על כל הסוכנים עבור מטרה אחת
        public async Task MissionCheckTarget(Target target)
        {
            double minDistance = 1000;
            Agent thisAgent = null;

            // הבאת כל הסוכנים
            var agents = await _context.Agents.Include(t => t.Location).ToListAsync();

            foreach (Agent agent in agents)
            {
                // בדיקה האם הסוכן רדום והוא הוצב במיקום
                if (
                    agent.Status == Enum.AgentStatus.Status.IN_ACTIVITY.ToString()
                    && agent.Location != null
                )
                {
                    // בדיקה של המרחק בין המטרה לסוכן
                    var distance = Distance(target, agent);
                    // בדיקה האם המרחק בין הסוכן למטרה פחות מ 200
                    if (distance <= 200)
                    {
                        if (distance <= minDistance)
                        {
                            minDistance = distance;
                            thisAgent = agent;
                        }
                    }
                }
            }
            // בדיקה האם נמצא סוכן שעומד בקרטריונים של ההצעות למשימה
            if (thisAgent != null)
            {
                // הוספת ההצעה
                Mission newMission = new Mission { Target = target, Agent = thisAgent };
                _context.Missions.Add(newMission);
            }

            await _context.SaveChangesAsync();
        }

        //// פונקציה שבודקת האם ההצעות של המטרה עדין תקינות
        //public async Task CheckMissionsTarget(Target target)
        //{
        //    var mission = await _context.Missions.Include(m => m.Target).Where(t => t.Id == target.Id).ToListAsync();
        //    if (mission != null)
        //    {
        //        foreach (Mission m in mission)
        //        {
        //            if (m.Target.Id == target.Id)
        //            {
        //                if (m.Status == Enum.MissionStatus.Status.PROPOSAL.ToString())
        //                {
        //                    _context.Missions.Remove(m);
        //                }
        //            }
        //        }
        //    }
        //}



        public double Distance(Target target, Agent agent)
        {
            return Math.Sqrt(
                Math.Pow(agent.Location.X - agent.Location.X, 2)
                    + Math.Pow(agent.Location.Y - agent.Location.Y, 2)
            );
        }
    }
}
