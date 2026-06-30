using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Consistenant
{
    public class CameraClamp : MonoBehaviour
    {
        // Define the bounds for clamping
        public float minX = -10f;
        public float maxX = 10f;
        public float minY = -10f;
        public float maxY = 10f;
        public float minZ = -10f;
        public float maxZ = 10f;

        void LateUpdate()
        {
            // Get the camera's current position
            Vector3 clampedPosition = transform.localPosition;

            // Clamp the position to the specified bounds
            clampedPosition.x = Mathf.Clamp(clampedPosition.x, minX, maxX);
            clampedPosition.y = Mathf.Clamp(clampedPosition.y, minY, maxY);
            clampedPosition.z = Mathf.Clamp(clampedPosition.z, minZ, maxZ);

            // Set the camera's position to the clamped position
            transform.localPosition = clampedPosition;
        }
    }
}
