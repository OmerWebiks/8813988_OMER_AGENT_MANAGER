namespace ManagementOfMossadAgentsAPI.Services
{
    public class ServiceMove
    {
        public string Location { get; set; }

        public Dictionary<string, List<int>> MoveDictionary = new Dictionary<string, List<int>>
        {
            {
                "nw",
                new List<int> { 1, -1 }
            },
            {
                "n",
                new List<int> { 1, 0 }
            },
            {
                "ne",
                new List<int> { 1, 1 }
            },
            {
                "e",
                new List<int> { 0, 1 }
            },
            {
                "se",
                new List<int> { -1, 1 }
            },
            {
                "s",
                new List<int> { -1, 0 }
            },
            {
                "sw",
                new List<int> { -1, -1 }
            },
            {
                "w",
                new List<int> { 0, -1 }
            },
        };
    }
}
