using UnityEngine;

public class CameraReturnToOriginalPosition : MonoBehaviour
{
    public Transform target; // The object to track (e.g., camera)
    public float smoothSpeed = 1f; // Speed at which the camera moves back to its original position
    private Vector3 originalPosition; // Store the original position of the camera

    private void Start()
    {
        // Store the initial position of the target (camera) when the game starts
        if (target != null)
        {
            originalPosition = target.position;
        }
    }

    private void Update()
    {
        // Only proceed if the target is not null
        if (target != null)
        {
            // Smoothly move the target (camera) towards the original position
            target.position = Vector3.Lerp(target.position, originalPosition, smoothSpeed * Time.deltaTime);
        }
    }
}
