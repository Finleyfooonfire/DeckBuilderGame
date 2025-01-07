using UnityEngine;

public class MouseFollow : MonoBehaviour
{
    // Public reference to the custom camera
    public Camera customCamera;

    // Public reference to the camera as a GameObject (instead of a Cinemachine Virtual Camera)
    public GameObject cameraObject;

    // Custom variable to control the percentage of the screen bounds the cube can move within
    [Range(0f, 1f)] public float movePercentageX = 1f;
    [Range(0f, 1f)] public float movePercentageY = 1f;

    public Vector3 screenPosition;
    public Vector3 worldPosition;
    private Plane plane = new Plane(Vector3.forward, 0);  // Plane for 2D screen to world space conversion

    // Cached camera settings
    private float cameraHeight;
    private float cameraWidth;

    public Vector3 startPosition = new Vector3(0f, 0f, 0f);
    private Vector3 cameraStartPosition;

    // Variables for smooth transition (resetting)
    public float resetSpeed = 1f;  // How fast the cube and camera reset

    private void Start()
    {
        // Check if the custom camera is assigned
        if (customCamera == null)
        {
            Debug.LogError("Custom Camera is not assigned!");
        }

        // Check if the camera GameObject is assigned
        if (cameraObject == null)
        {
            Debug.LogError("Camera Object is not assigned!");
        }

        // Store the starting positions of the cube and camera GameObject
        transform.position = startPosition;
        cameraStartPosition = cameraObject.transform.position;

        // Update the camera bounds
        UpdateCameraBounds();
    }

    private void Update()
    {
        // Ensure that the custom camera is assigned before proceeding
        if (customCamera == null)
        {
            return;  // Exit if no camera is assigned to avoid errors
        }

        // Get the mouse position in screen space
        screenPosition = Input.mousePosition;

        // Create a ray from the screen position using the custom camera
        Ray ray = customCamera.ScreenPointToRay(screenPosition);

        // If the ray hits the plane, get the intersection point in world space
        if (plane.Raycast(ray, out float distance))
        {
            worldPosition = ray.GetPoint(distance);
        }

        // Clamp the cube's position within the camera bounds
        worldPosition = ClampPositionWithinBounds(worldPosition);

        // Set the object's position to the calculated and clamped world position
        transform.position = worldPosition;

        // Gradually reset the cube position back to the start position
        transform.position = Vector3.Lerp(transform.position, startPosition, resetSpeed * Time.deltaTime);

        // Gradually reset the Camera object position back to its start position
        cameraObject.transform.position = Vector3.Lerp(cameraObject.transform.position, cameraStartPosition, resetSpeed * Time.deltaTime);
    }

    // Method to calculate and update the camera's bounds in world space
    void UpdateCameraBounds()
    {
        // The height of the camera's view at a given distance
        cameraHeight = 2f * customCamera.orthographicSize;

        // The width of the camera's view at a given distance (aspect ratio is the ratio of width to height)
        cameraWidth = cameraHeight * customCamera.aspect;
    }

    // Method to clamp the cube's position within the camera's bounds, limited by the move percentage
    Vector3 ClampPositionWithinBounds(Vector3 targetPosition)
    {
        // Calculate the min and max bounds based on the camera's view size and the movePercentage
        float minX = customCamera.transform.position.x - cameraWidth * movePercentageX;
        float maxX = customCamera.transform.position.x + cameraWidth * movePercentageX;

        float minY = customCamera.transform.position.y - cameraHeight * movePercentageY;
        float maxY = customCamera.transform.position.y + cameraHeight * movePercentageY;

        // Clamp the target position within the bounds
        targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
        targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);

        return targetPosition;
    }
}
