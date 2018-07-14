using System;
 
namespace func_rocket
{
    public class ControlTask
    {
        public static double TotalAngle;
 
        public static Turn ControlRocket(Rocket rocket, Vector target)
        {
            var distanceVector = new Vector(target.X - rocket.Location.X, target.Y - rocket.Location.Y);
 
            if (Math.Abs(distanceVector.Angle - rocket.Direction) < 0.5
                || Math.Abs(distanceVector.Angle - rocket.Velocity.Angle) < 0.5)
            {
                TotalAngle = (distanceVector.Angle - rocket.Direction + distanceVector.Angle - rocket.Velocity.Angle) /2;
            }
            else
            {
                TotalAngle = distanceVector.Angle - rocket.Direction;
            }
 
            if (TotalAngle < 0)
                return Turn.Left;
            return TotalAngle > 0 ? Turn.Right : Turn.None;
        }
    }
}