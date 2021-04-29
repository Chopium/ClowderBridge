using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class wobbleController : MonoBehaviour
{
#if UNITY_EDITOR
    public Shader target;

    public Vector3 goalNoiseScrollRate;
    public float goalExplodeAmount;
    
    // Update is called once per frame
    [ExecuteInEditMode]
    void Update()
    {
        if (!EditorApplication.isPlaying)
        {
            Shader.SetGlobalVector("_NoiseScrollRate", goalNoiseScrollRate);
            Shader.SetGlobalFloat("_NoiseDisplacePower", goalExplodeAmount);
        }
    }
#endif

}