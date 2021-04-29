using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enableCanvasOnAwake : MonoBehaviour
{
    Canvas target;
    private void Awake()
    {
        target = this.GetComponent<Canvas>();
        target.enabled = true;
        target.worldCamera = Camera.main;
    }
}
