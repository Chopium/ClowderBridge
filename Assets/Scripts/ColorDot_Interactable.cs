using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class ColorDot_Interactable : MonoBehaviour
{
    //public ColorPicker_Interactable GUI;
    public SpriteRenderer ColorTarget;
    public UnityEvent<ColorDot_Interactable> mouseDown;
    // Start is called before the first frame update
    public bool isActivated;
    public int index = 0;
    public void setColor(Color target)
    {
        //target.a = 1f;
        if (target != ColorTarget.color)
        {
            target.a = 1f;
            ColorTarget.color = target;
        }
        
        //ColorTarget.color = new Color(ColorTarget.color.r, ColorTarget.color.g, ColorTarget.color.b, 1f);
    }
    private void OnMouseDown()
    {
        mouseDown.Invoke(this);

    }
}
