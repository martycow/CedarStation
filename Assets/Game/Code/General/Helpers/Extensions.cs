using UnityEngine;

namespace Game.General
{
    public static class Extensions
    {
        public static Vector3 ToVector3(this Vector2 vector, bool yIsZ = true)
        {
            return yIsZ ? 
                new Vector3(vector.x, 0f, vector.y) : 
                new Vector3(vector.x, vector.y, 0f);
        }
    }
}