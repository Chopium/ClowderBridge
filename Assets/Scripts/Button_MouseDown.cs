using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class Button_MouseDown : MonoBehaviour
{
    public UnityEvent onMouseDown;

    // Start is called before the first frame update
    private void OnMouseDown()
    {
        onMouseDown.Invoke();
    }
    public void doMouseDown()
    {
        onMouseDown.Invoke();
    }
}

