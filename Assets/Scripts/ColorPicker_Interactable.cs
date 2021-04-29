using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorPicker_Interactable : MonoBehaviour
{
    public SpriteRenderer visualizer;
    public SpriteRenderer[] outlines;
    public Material target;
    public Ornament_2020_Interface ornament;
    public LuminanceHandle_Interactive lum_handle;
    public int currentIndex = 1;
    [Range(0, 1)]
    public float hue;
    [Range(0, 1)]
    public float sat;
    [Range(0, 1)]
    public float val;
    private AudioSource dragSound;
    public void OnMouseDown()
    {
        dragSound = SoundBoard.Instance.getAudioSource("click_drag");
        dragSound.transform.position = this.transform.position;
        AveragedDelta = new Vector2[averageCount];
        currentCount = 0;
        for (int i = 0; i < AveragedDelta.Length; i++)
        {
            AveragedDelta[i] = Vector2.zero;
        }

        lastMousePosition = Input.mousePosition;
        currentMomentum = Vector2.zero;
    }
    private Vector3 lastMousePosition;
    private Vector3 deltaMousePosition;
    public Vector2 sensitivity = Vector2.one;
    private void OnMouseDrag()
    {
        deltaMousePosition = Input.mousePosition - lastMousePosition;
        hue += deltaMousePosition.x * sensitivity.x;
        hue = hue < 0 ? 1 : hue;
        hue = hue > 1 ? 0 : hue;

        sat += deltaMousePosition.y * sensitivity.y;
        sat = sat < 0 ? 0 : sat;
        sat = sat > 1 ? 1 : sat;

        if (currentCount >= averageCount-1)
        {
            currentCount = 0;
        }
        AveragedDelta[currentCount] = deltaMousePosition;
        currentCount++;

        lastMousePosition = Input.mousePosition;
    }

    public int averageCount = 5;
    int currentCount;
    Vector2[] AveragedDelta;

    private Vector2 currentMomentum = Vector2.zero;
    public float ballInertia = 1;
    private void OnMouseUp()
    {
        currentMomentum = Vector3.zero;
        for (int i = 0; i < AveragedDelta.Length; i++)
        {
            currentMomentum += AveragedDelta[i];
        }
        currentMomentum /= averageCount;
        currentMomentum *= ballInertia;
        SoundBoard.Instance.playSound("click_yes");
        Destroy(dragSound);
    }

    Color outlineColor;
    Color currentColor;
    Color lastColor;
    public int InertiaDecayRate = 10;
    public void setColor(Color input)
    {
        input.a = 1f;
        Color.RGBToHSV(input, out hue, out sat, out val);
        lum_handle.setValue(val);
    }

    void Update()
    {
        currentColor = Color.HSVToRGB(hue, sat, val);
        if (lastColor != currentColor)
        {
            if (ornament != null)
            {
                ornament.setColor(currentIndex, currentColor);
            }
            if (target != null)
            {
                //Debug.Log("HERE");
                target.SetFloat("_hue", hue);
                target.SetFloat("_sat", sat);
                target.SetFloat("_val", val);
            }
            if (visualizer != null)
            {
                visualizer.color = currentColor;
            }
            if (outlines != null)
            {
                outlineColor = val > 0.75f ? Color.Lerp(Color.white, new Color(0.95f, 0.95f, 0.95f), Mathf.InverseLerp(0.75f, 1f, val) - (sat * 4)) : Color.white; //&& sat < 0.2f)

                foreach (var item in outlines)
                {
                    item.color = outlineColor;
                }
            }
            lastColor = currentColor;
        }

        if (Mathf.Abs(currentMomentum.x) > 0.1)
        {
            hue += currentMomentum.x * sensitivity.x;
            hue = hue < 0 ? 1 : hue;
            hue = hue > 1 ? 0 : hue;
            currentMomentum.x -= currentMomentum.x * sensitivity.x * InertiaDecayRate;
        }
        if (Mathf.Abs(currentMomentum.y) > 0.1)
        {
            sat += currentMomentum.y * sensitivity.y;
            sat = sat < 0 ? 0 : sat;
            sat = sat > 1 ? 1 : sat;
            currentMomentum.y -= currentMomentum.y * sensitivity.y * InertiaDecayRate;
        }
    }
}
