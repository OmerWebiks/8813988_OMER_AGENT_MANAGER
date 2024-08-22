using System.ComponentModel.DataAnnotations;

namespace ManagementOfMossadAgentsAPI.Models
{
    public class Location
    {
        [Key]
        public int? Id { get; set; }

        [Range(0, 1000, ErrorMessage = "X must be between 0 and 1000")]
        public int X { get; set; }

        [Range(0, 1000, ErrorMessage = "Y must be between 0 and 1000")]
        public int Y { get; set; }

        public Location(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
    }
}
