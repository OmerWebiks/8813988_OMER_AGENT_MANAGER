using System.ComponentModel.DataAnnotations;

namespace ManagementOfMossadAgentsAPI.Models
{
    public class Location
    {
        [Key]
        public Guid? Id { get; set; }

        [Range(0, 1000, ErrorMessage = "X must be between 0 and 1000")]
        public int x { get; set; }

        [Range(0, 1000, ErrorMessage = "Y must be between 0 and 1000")]
        public int y { get; set; }

        public Location(int x, int y)
        {
            this.Id = Guid.NewGuid();
            this.x = x;
            this.y = y;
        }
    }
}
