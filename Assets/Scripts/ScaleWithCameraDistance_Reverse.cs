using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleWithCameraDistance_Reverse : MonoBehaviour
{
    public float objectScale = 1.0f;
    private Vector3 initialScale;


    // set the initial scale, and setup reference camera
    void Awake()
    {
        // record initial scale, use this as a basis
        initialScale = transform.localScale;
    }

    //[ExecuteInEditMode]
    void Update()
    {
        Plane plane = new Plane(Camera.main.transform.forward, Camera.main.transform.position);
        float dist = plane.GetDistanceToPoint(transform.position);
        if (dist < 1)
        {
            transform.localScale = initialScale * 1 / dist;
        }
    }
}
