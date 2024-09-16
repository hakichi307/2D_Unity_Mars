using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float smoothSpeed = 0.125f;
    public Vector3 offset;
    [Header("Camera bounds")]
    public Vector3 minCamerabounds;
    public Vector3 maxCamerabounds;

    private void FixedUpdate()
    {
        // Desired position is the target position + offset
        Vector3 desiredPosition = target.localPosition + offset;
        var localPosition = transform.localPosition;

        // Smooth the camera movement
        Vector3 smoothedPosition = Vector3.Lerp(localPosition, desiredPosition, smoothSpeed);

        // Update camera's position
        localPosition = smoothedPosition;

        // Set localPosition.x to follow the target's x position
        localPosition.x = target.localPosition.x + 8;

        // Clamp only the y and z positions of the camera
        localPosition = new Vector3(
            localPosition.x, // Follow the player's x position directly
            Mathf.Clamp(localPosition.y, minCamerabounds.y, maxCamerabounds.y), // Clamp Y
            Mathf.Clamp(localPosition.z, minCamerabounds.z, maxCamerabounds.z)  // Clamp Z
        );

        // Apply the new position to the camera
        transform.localPosition = localPosition;
    }

    public void SetTarget(Transform targetToSet)
    {
        target = targetToSet;
    }
}
