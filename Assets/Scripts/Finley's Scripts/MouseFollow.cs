using UnityEngine;

public class MouseFollow : MonoBehaviour
{
    public Camera raycastingcamera;
    public GameObject cinemachinecamera;
    [Range(0f, 1f)] public float movePercentageX = 1f;
    [Range(0f, 1f)] public float movePercentageY = 1f;
    public Vector3 screenPosition;
    public Vector3 worldPosition;
    private Plane plane = new Plane(Vector3.forward, 0);
    private float heightofcamera;
    private float widthofcamera;
    public Vector3 defaultposition = new Vector3(0f, 0f, 0f);
    private Vector3 cameraStartPosition;
    public float resettingspeed = 1f; 

    private void Start()
    {
        if (raycastingcamera == null)
        {
            //Debug.LogError("Custom Camera is not assigned!");
        }

        if (cinemachinecamera == null)
        {
           // Debug.LogError("Camera Object is not assigned!");
        }
        transform.position = defaultposition;
        cameraStartPosition = cinemachinecamera.transform.position;
        UpdateCameraBounds();
    }

    private void Update()
    {
        if (raycastingcamera == null)
        {
            return;
        }
        screenPosition = Input.mousePosition;
        Ray ray = raycastingcamera.ScreenPointToRay(screenPosition);
        if (plane.Raycast(ray, out float distance))
        {
            worldPosition = ray.GetPoint(distance);
        }
        worldPosition = ClampPositionWithinBounds(worldPosition);
        transform.position = worldPosition;
        transform.position = Vector3.Lerp(transform.position, defaultposition, resettingspeed * Time.deltaTime);
        cinemachinecamera.transform.position = Vector3.Lerp(cinemachinecamera.transform.position, cameraStartPosition, resettingspeed * Time.deltaTime);
    }
    void UpdateCameraBounds()
    {
        heightofcamera = 2f * raycastingcamera.orthographicSize;
        widthofcamera = heightofcamera * raycastingcamera.aspect;
    }

    Vector3 ClampPositionWithinBounds(Vector3 targetPosition)
    {
        float minX = raycastingcamera.transform.position.x - widthofcamera * movePercentageX;
        float maxX = raycastingcamera.transform.position.x + widthofcamera * movePercentageX;
        float minY = raycastingcamera.transform.position.y - heightofcamera * movePercentageY;
        float maxY = raycastingcamera.transform.position.y + heightofcamera * movePercentageY;
        targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
        targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);

        return targetPosition;
    }
}
