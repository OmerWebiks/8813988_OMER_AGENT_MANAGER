using ManagementOfMossadAgentsAPI.Models;

namespace ManagementOfMossadAgentsAPI.Utils;

public class CalculateDistanceToTarget
{
    // פונקציה שבודקת את המרחק בין סוכן למטרה
    public static double Distance(Target target, Agent agent)
    {
        return Math.Sqrt(
            Math.Pow(target.Location.X - agent.Location.X, 2)
                + Math.Pow(target.Location.Y - agent.Location.Y, 2)
        );
    }

    // פונקציה שבודקת האם הסוכן והמטרה באותו משבצת
    public static bool IsInSamePlace(Location locationOne, Location locationTwo)
    {
        if (locationOne.X == locationTwo.X && locationOne.Y == locationTwo.Y)
            return true;
        return false;
    }

    // פונקציה שבודקת האם הסוכן או המטרה כבר נמצאים בקצה
    public static bool IfMoveOutOfRange(Location Location, Location move)
    {
        return Location.X + move.X > 1000
            || Location.Y + move.Y > 1000
            || Location.X + move.X < 0
            || Location.Y + move.Y < 0;
    }
}
