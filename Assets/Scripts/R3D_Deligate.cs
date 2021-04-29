using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class R3D_Deligate : MonoBehaviour
{
    public UnityEvent<RaycastHit> target;
    public delegate void OnSelected();
    // This is an event, allows to protect delegates
    // from external modification making them safer
    public event OnSelected onSelected;
    public void invokeSelected()
    {
        onSelected();
    }
    private void OnMouseDown()
    {
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity))
        {
            target.Invoke(hit);
            Debug.Log("Did Hit");
        }
        else
        {
            Debug.Log("Did not Hit");
        }
        
    }
}