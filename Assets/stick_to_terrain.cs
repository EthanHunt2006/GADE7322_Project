using UnityEngine;

public class stick_to_terrain : MonoBehaviour
{
    Vector3 offset = new Vector3(0.0f, 1.0f, 0.0f);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Ray ray = new Ray(transform.position, Vector3.down);

        RaycastHit rit;

        Physics.Raycast(ray, out rit);

        transform.position = rit.point + offset;
        
    }
}
