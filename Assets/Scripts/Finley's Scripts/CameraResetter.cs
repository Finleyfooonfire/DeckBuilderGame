using UnityEngine;

public class CameraReturnToOriginalPosition : MonoBehaviour
{
    public Transform target;
    public float speed = 1f; 
    private Vector3 startposition; 

    private void Start()
    {
        if (target != null)
        {
            startposition = target.position;
        }
    }

    private void Update()
    {
        if (target != null)
        {
            target.position = Vector3.Lerp(target.position, startposition, speed * Time.deltaTime);
        }
    }
}