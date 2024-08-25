using ManagementOfMossadAgentsAPI.Models;
using Microsoft.EntityFrameworkCore;
using static System.Net.Mime.MediaTypeNames;

namespace ManagementOfMossadAgentsAPI.Del
{
    public class ManagementOfMossadAgentsDbContext : DbContext
    {
        // יצירת הטבלאות ב DB עם שמות של הטבלאות
        public DbSet<Agent> Agents { get; set; }
        public DbSet<Target> Targets { get; set; }
        public DbSet<Mission> Missions { get; set; }
        public DbSet<Location> Locations { get; set; }

        public ManagementOfMossadAgentsDbContext(
            DbContextOptions<ManagementOfMossadAgentsDbContext> options
        )
            : base(options)
        {
            if (
                Database.EnsureCreated()
                && Targets.Count() == 0
                && Agents.Count() == 0
                && Missions.Count() == 0
            )
            {
                Console.WriteLine("DbContext:ManagementOfMossadAgentsDbContext: true");

                //SeedAgent();
                //SeedTarget();
                //SeedMission();
            }
        }

        public void SeedAgent()
        {
            Agents.Add(
                new Agent
                {
                    Nickname = "Zara",
                    Location = new Location(50, 40),
                    Status = Enum.AgentStatus.Status.DORMANT.ToString(),
                    PhotoUrl = "fdndfdfn"
                }
            );
            SaveChanges();
        }

        public void SeedTarget()
        {
            Targets.Add(
                new Target
                {
                    Name = "Zara",
                    Position = "Manager",
                    Location = new Location(30, 20),
                    Status = Enum.TargetStatus.Status.LIVE.ToString(),
                }
            );
            SaveChanges();
        }

        public void SeedMission()
        {
            Missions.Add(
                new Mission
                {
                    Agent = Agents.First(),
                    Target = Targets.First(),
                    TimeLeft = 50,
                    Status = Enum.MissionStatus.Status.PROPOSAL.ToString(),
                }
            );
        }
    }
}
