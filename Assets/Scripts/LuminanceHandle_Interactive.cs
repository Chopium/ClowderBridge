using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class LuminanceHandle_Interactive : MonoBehaviour
{

    public SpriteRenderer visualizer;
    public SpriteRenderer outline;
    public ColorPicker_Interactable target;
    public Transform axis;
    [Range(0, 1)]
    public float val = 0.5f;
    float val_last = -1f;

    // Start is called before the first frame update
    public void OnMouseDown()
    {
        Debug.Log("DOWN");
        lastMousePosition = Input.mousePosition;
    }
    private Vector3 lastMousePosition;
    private Vector3 deltaMousePosition;
    public Vector2 sensitivity = Vector2.one;
    private void OnMouseDrag()
    {
        //Debug.Log("HERE");
        deltaMousePosition = Input.mousePosition - lastMousePosition;
        val += deltaMousePosition.y * sensitivity.y;
        val = val < 0 ? 0 : val;
        val = val > 1 ? 1 : val;
        lastMousePosition = Input.mousePosition;
    }

    Color outlineColor;
    public float handleLength = 30;
    public void setValue(float input)
    {
        val = input;
        //axis.transform.localRotation = Quaternion.Euler(0, 0, Mathf.Lerp(handleLength, -handleLength, val));
    }
    void Update()
    {
        if (val_last != val)
        {
            if (target != null)
            {
                //Debug.Log("HERE");
                target.val = val;
            }
            visualizer.color = Color.Lerp(Color.black, Color.white, val);
            axis.transform.localRotation = Quaternion.Euler(0, 0, Mathf.Lerp(handleLength, -handleLength, val));
            outline.color = val > 0.75f ? Color.Lerp(Color.white, Color.gray, Mathf.InverseLerp(0.75f, 1f, val)) : Color.white; //&& sat < 0.2f)
            val_last = val;
        }

    }
}
