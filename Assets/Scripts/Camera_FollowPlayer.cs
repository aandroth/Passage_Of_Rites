using UnityEngine;

public class Camera_FollowPlayer : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] Vector3 offset;
    [SerializeField] float smoothTime = 0.3f;
    [SerializeField] Vector3 velocity = Vector3.zero;
    [SerializeField] float zOffset = -10f;

    // LateUpdate is called after all Update functions have been called
    private void LateUpdate()
    {
        if (target != null)
        {
            // Calculate the desired position for the camera
            Vector3 desiredPosition = target.position + offset;
            desiredPosition.z = zOffset; // Maintain a fixed z-offset for 2D view

            // Smoothly move the camera towards the desired position
            transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothTime);
        }
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}
