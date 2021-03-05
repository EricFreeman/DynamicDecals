using UnityEngine;
using System.Collections;

namespace LlockhamIndustries.ExtensionMethods
{
    public static class Vector3ExtensionMethods
    {
        //Calculation
        public static float DistanceXZ(this Vector3 PositionOne, Vector3 PositionTwo)
        {
            Vector2 positionOne = Vector2.zero;
            Vector2 positionTwo = Vector2.zero;

            positionOne.x = PositionOne.x;
            positionOne.y = PositionOne.z;

            positionTwo.x = PositionTwo.x;
            positionTwo.y = PositionTwo.z;

            return Vector2.Distance(positionOne, positionTwo);
        }
        public static Vector2 DirectionXZ(this Vector3 PositionOne, Vector3 PositionTwo)
        {
            Vector2 positionOne = Vector2.zero;
            Vector2 positionTwo = Vector2.zero;

            positionOne.x = PositionOne.x;
            positionOne.y = PositionOne.z;

            positionTwo.x = PositionTwo.x;
            positionTwo.y = PositionTwo.z;

            return (positionTwo - positionOne).normalized;
        }

        //Conversion
        public static Vector2 xz(this Vector3 Vector3)
        {
            Vector2 Vector2 = Vector2.zero;
            Vector2.x = Vector3.x;
            Vector2.y = Vector3.z;

            return Vector2;
        }
        public static Vector2 xy(this Vector3 Vector3)
        {
            Vector2 Vector2 = Vector2.zero;
            Vector2.x = Vector3.x;
            Vector2.y = Vector3.y;

            return Vector2;
        }
        public static Vector2 yz(this Vector3 Vector3)
        {
            Vector2 Vector2 = Vector2.zero;
            Vector2.x = Vector3.y;
            Vector2.y = Vector3.z;

            return Vector2;
        }
    }
}