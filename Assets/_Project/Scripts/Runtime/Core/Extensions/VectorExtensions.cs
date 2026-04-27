using UnityEngine;

namespace Markyu.LastKernel
{
    public static class VectorExtensions
    {
        /// <summary>
        /// Returns a new Vector3 with the Y component set to 0.
        /// Useful for snapping positions to the ground plane.
        /// </summary>
        public static Vector3 Flatten(this Vector3 vector)
        {
            return new Vector3(vector.x, 0f, vector.z);
        }

        /// <summary>Returns a copy of the vector with one or more components replaced.</summary>
        public static Vector3 With(this Vector3 v, float? x = null, float? y = null, float? z = null)
        {
            return new Vector3(x ?? v.x, y ?? v.y, z ?? v.z);
        }
    }
}

