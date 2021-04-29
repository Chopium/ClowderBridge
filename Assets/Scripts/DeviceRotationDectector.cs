using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(RectTransform))]
public class DeviceRotationDectector : MonoBehaviour
{
    public UnityEvent DimensionsChanged;
    public RectTransform rectTransform;
    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        if (DimensionsChanged == null)
            DimensionsChanged = new UnityEvent();

        //m_MyEvent.AddListener(Ping);
    }

    protected void OnRectTransformDimensionsChange()
    {
        //Debug.Log("SCREEN CHANGED, RECALIBRATING");
        DimensionsChanged.Invoke();
    }
}