using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleWithCameraDistance : MonoBehaviour
{
    public float objectScale = 1.0f;
    private Vector3 initialScale;
    

    // set the initial scale, and setup reference camera
    void Start()
    {
        // record initial scale, use this as a basis
        initialScale = transform.localScale;
    }

    //[ExecuteInEditMode]
    void Update()
    {
        Plane plane = new Plane(Camera.main.transform.forward, Camera.main.transform.position);
        float dist = plane.GetDistanceToPoint(transform.position);
        transform.localScale = Vector3.one * Mathf.Clamp((initialScale.x * dist*dist * objectScale), 0.01f, .7f);
    }
}