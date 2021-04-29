using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class hideInEditor : MonoBehaviour
{
    public bool hide = false;

    private void Awake()
    {
        this.gameObject.SetActive(true);
    }

#if UNITY_EDITOR
    // Update is called once per frame
    void Update()
    {
            this.gameObject.SetActive(!hide);
    }
    #endif
}
