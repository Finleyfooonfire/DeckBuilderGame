using UnityEngine;

public class MouseFollow : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public Vector3 screenPositon;
    public Vector3 worldPositon;
    Plane plane = new Plane(Vector3.forward, 0);

    // Update is called once per frame
    void Update()
    {
        screenPositon = Input.mousePosition;

        Ray ray = Camera.main.ScreenPointToRay(screenPositon);

        if (plane.Raycast(ray, out float distance))
        {
            worldPositon = ray.GetPoint(distance);


        }

        transform.position = worldPositon;
    }
}
