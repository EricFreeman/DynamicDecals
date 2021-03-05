using UnityEngine;
using System.Collections;

namespace LlockhamIndustries.ExtensionMethods
{
    public static class Vector2ExtensionMethods
    {
        //Direction between
        public static bool DirectionBetween(this Vector2 Direction, Vector2 CounterClockwiseLimit, Vector2 ClockwiseLimit)
        {
            return (Direction.GetClockwiseAngleTo(ClockwiseLimit) < Direction.GetClockwiseAngleTo(CounterClockwiseLimit));
        }

        //Get angle
        public static float GetClockwiseAngleTo(this Vector2 from, Vector2 to)
        {
            float angle = Vector2.Angle(from, to);
            if (Vector2.Dot(from.RotateClockwise(90), to) < 0)
            {
                angle = 360 - angle;
            }
            return angle;
        }
        public static float GetCounterClockwiseAngleTo(this Vector2 from, Vector2 to)
        {
            float angle = Vector2.Angle(from, to);
            if (Vector2.Dot(from.RotateClockwise(90), to) > 0)
            {
                angle = 360 - angle;
            }
            return angle;
        }

        //Rotate Direction
        public static Vector2 RotateClockwise(this Vector2 v, float angle)
        {
            var ca = Mathf.Cos(Mathf.Deg2Rad * -angle);
            var sa = Mathf.Sin(Mathf.Deg2Rad * -angle);
            return new Vector2(ca * v.x - sa * v.y, sa * v.x + ca * v.y);
        }
        public static Vector2 RotateCounterClockwise(this Vector2 v, float angle)
        {
            var ca = Mathf.Cos(Mathf.Deg2Rad * angle);
            var sa = Mathf.Sin(Mathf.Deg2Rad * angle);
            return new Vector2(ca * v.x - sa * v.y, sa * v.x + ca * v.y);
        }
        public static Vector2 RotateTowards(this Vector2 v, Vector2 GoalDirection, float angle)
        {
            //Make sure we aren't overRotating.
            angle = Mathf.Min(angle, Vector2.Angle(v, GoalDirection));
            if (v.GetClockwiseAngleTo(GoalDirection) <= 180)
            {
                return v.RotateClockwise(angle);
            }
            else
            {
                return v.RotateCounterClockwise(angle);
            }
        }

        //Direction Lerps
        public static Vector2 LerpDirectionTowards(this Vector2 from, Vector2 to, float amount)
        {
            float angle = Vector2.Angle(from, to);
            Vector2 fromCross = RotateClockwise(from, 90);
            if (Vector2.Dot(fromCross, to) > 0)
            {
                angle *= -1;
            }
            return RotateClockwise(from, angle * amount);
        }
        public static Vector2 LerpClockwiseTowards(this Vector2 from, Vector2 to, float amount)
        {
            return from.RotateClockwise(from.GetClockwiseAngleTo(to) * amount);
        }
        public static Vector2 LerpCounterClockwiseTowards(this Vector2 from, Vector2 to, float amount)
        {
            return from.RotateCounterClockwise(from.GetCounterClockwiseAngleTo(to) * amount);
        }

        //AngleToVector2
        public static Vector2 AngleToVector2(this float Angle)
        {
            //Returns a Vector2 from an Angle. 
            Angle *= Mathf.Deg2Rad;
            return new Vector2(Mathf.Sin(Angle), Mathf.Cos(Angle));
        }

        //RotateAroundPoint
        public static Vector2 RotatePointAroundPivot(this Vector2 Point, Vector2 Pivot, Quaternion Rotation)
        {
            //Convert vector2s to vector3s
            Vector3 point = new Vector3(Point.x, 0, Point.y);
            Vector3 pivot = new Vector3(Pivot.x, 0, Pivot.y);

            //Rotate
            Vector3 Direction = point - pivot;
            Direction = Rotation * Direction;

            //Convert back to vector2
            return (Direction + pivot).xz();
         }
    }
}