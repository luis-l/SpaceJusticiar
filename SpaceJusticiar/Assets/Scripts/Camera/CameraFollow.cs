using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
    // The transform to follow.
    public Transform followingTransform;

    public bool bFollow = true;

    // If the camera should be a bit ahead of the target.
    // Value of zero means that the target is on the camera center.
    // This only works in the target has a rigid body.
    public float aheadFactor = 0f;

    // The damping factor. Lower values means faster smoothing.
    public float damping = 1f;

    // For SmoothDamp funciton.
    private Vector3 _currentVelocity;

    // If the squared distance between the camera and follow target
    // is less than the squared snap distance, then the camera
    // will snap to the target since it is too far to follow with damping.
    public float snapDistanceSq = 50f * 50f;

    // Update is called once per frame
    void LateUpdate()
    {

        if (followingTransform != null && bFollow) {

            Vector3 newPos = followingTransform.position;

            Vector2 toTarget = followingTransform.position - transform.parent.position;

            // Camera is too far away for smoothing.
            // Snap to target.
            if (toTarget.sqrMagnitude > snapDistanceSq) {
                Vector3 snapPos = followingTransform.position;
                snapPos.z = transform.parent.position.z;
                transform.parent.position = snapPos;
            }

            // Get the direction towards the mouse from the target
            Vector3 currentMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 toMouse = (currentMousePos - followingTransform.position).normalized;

            // The ahead factor is scaled by the camera size as well.
            // This way the size of camera dictates how far out the camera should get ahead.
            newPos += toMouse * aheadFactor * Camera.main.orthographicSize / 8f;

            // Keep the camera z the same.
            newPos.z = transform.parent.position.z;

            // Damp the camera parent.
            transform.parent.position = Vector3.SmoothDamp(transform.parent.position, newPos, ref _currentVelocity, damping);
        }
    }
}
