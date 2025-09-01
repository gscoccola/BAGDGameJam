using System.Collections.Generic;
using UnityEngine;

namespace PixelMate.Utilities
{
    public static class MathHelper
    {
        /// <summary>
        /// Returns evenly distributed positions around a point,
        /// all at the same distance, constrained to the XZ plane.
        /// </summary>
        /// <param name="center">The central point.</param>
        /// <param name="radius">Distance from the center.</param>
        /// <param name="count">Number of positions to generate.</param>
        public static List<Vector3> GetEvenCirclePositions(Vector3 center, float radius, int count)
        {
            var positions = new List<Vector3>(count);

            for (int i = 0; i < count; i++)
            {
                // Calculate the evenly spaced angle in radians
                float angle = (i / (float)count) * Mathf.PI * 2f;

                // Compute offsets
                float x = Mathf.Cos(angle) * radius;
                float z = Mathf.Sin(angle) * radius;

                // Keep Y from the center
                positions.Add(new Vector3(center.x + x, center.y, center.z + z));
            }

            return positions;
        }
    }
}
