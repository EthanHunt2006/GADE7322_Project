using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class HoverLogic : MonoBehaviour
{

    public List<Vector3> hover_points = new List<Vector3>();

    int count_point = 0;

    Vector3 active_hover_point = new Vector3(0,0,0);

    Vector3 initial_position;

    float current_hover_timer = 0.0f;

    public float hover_timer = 5.0f;

    public float hover_speed = 2.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initial_position = transform.position;
        
    }

    // Update is called once per frame
    void Update()
    {

        if (Vector3.Distance(transform.position, active_hover_point) > 0.5f)
        {
            MoveToHoverPoint();
        }

        Hover();
        
    }

    void MoveToHoverPoint()
    {
        transform.position = Vector3.Lerp(transform.position, active_hover_point, Time.deltaTime * hover_speed);

    }

    void Hover ()
    {
        current_hover_timer -= Time.deltaTime;

        if (current_hover_timer < 0.0f)
        {
            DetermineHoverPoint();

            current_hover_timer = hover_timer;
        }
    }

    void DetermineHoverPoint ()
    {
        int pick_point = Random.Range(0, hover_points.Count);

        

        foreach(Vector3 point in hover_points)
        {

            if (pick_point == count_point)
            {
                active_hover_point = initial_position + point;
            }

            count_point++;

        }

        count_point = 0;
    }
}
