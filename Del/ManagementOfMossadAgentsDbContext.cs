using Microsoft.EntityFrameworkCore;

namespace ManagementOfMossadAgentsAPI.Del
{
    public class ManagementOfMossadAgentsDbContext : DbContext
    {
        public ManagementOfMossadAgentsDbContext(
            DbContextOptions<ManagementOfMossadAgentsDbContext> options
        )
            : base(options)
        {
            Console.WriteLine(
                "DbContext:ManagementOfMossadAgentsDbContext: " + Database.EnsureCreated()
            );
        }
    }
}
