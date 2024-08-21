namespace ManagementOfMossadAgentsAPI.Models
{
    public class Location
    {
        public Guid Id { get; set; }
        public int x { get; set; }
        public int y { get; set; }

        public Location(int x, int y)
        {
            this.Id = Guid.NewGuid();
            this.x = x;
            this.y = y;
        }
    }
}
