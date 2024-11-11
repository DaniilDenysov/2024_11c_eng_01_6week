using System.Collections.Generic;
using UnityEngine;

namespace Validation
{
    public static class RaycastStepValidator
    {
        /// <summary>
        /// Checks if movement in the given direction is valid by raycasting and checking if a sufficient portion is unobstructed.
        /// </summary>
        /// <param name="startPosition">Starting position of the movement</param>
        /// <param name="direction">Direction to validate</param>
        /// <param name="stepSize">Distance to check</param>
        /// <param name="sampleCount">Number of rays to cast for checking the edge</param>
        /// <param name="obstacleLayerMask">Layer mask to detect obstacles</param>
        /// <returns>True if enough space is unobstructed in the given direction</returns>
        public static bool IsValid(Vector3 startPosition, Vector3 direction, float stepSize, int sampleCount, LayerMask obstacleLayerMask)
        {
            int clearSampleCount = 0;

            Vector3 perpendicular = Vector3.Cross(direction, Vector3.forward).normalized;
            float sampleSpacing = stepSize / (sampleCount - 1);

            for (int i = 0; i < sampleCount; i++)
            {
                Vector3 sampleOffset = perpendicular * (i * sampleSpacing - (stepSize / 2));
                Vector3 rayOrigin = startPosition + sampleOffset;

                if (!Physics.Raycast(rayOrigin, direction, stepSize, obstacleLayerMask))
                {
                    clearSampleCount++;
                }
            }

            float clearRatio = (float)clearSampleCount / sampleCount;
            return clearRatio > 0.6f;
        }
    }
}
