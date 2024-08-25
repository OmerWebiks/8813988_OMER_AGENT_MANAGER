using ManagementOfMossadAgentsAPI.Models;

namespace ManagementOfMossadAgentsAPI.Utils;

public class GeneralFunctions
{
    // פונקציה שבודקת את המרחק בין סוכן למטרה
    public static double Distance(Location locationTarget, Location locationAgent)
    {
        return Math.Sqrt(
            Math.Pow(locationTarget.X - locationAgent.X, 2)
                + Math.Pow(locationTarget.Y - locationAgent.Y, 2)
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

    // פונקציה לבדיקה כמה זמן נשאר למשימה
    public async Task<double> TimeLeft(Location locationAgent, Location locationTarget)
    {
        double distance = Distance(locationTarget, locationAgent);
        return distance / 5;
    }
}
