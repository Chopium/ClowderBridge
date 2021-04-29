using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerformanceSettings : MonoBehaviour
{
    public int maxObjects = 1;

    private static PerformanceSettings instance;
    public static PerformanceSettings Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new PerformanceSettings();
            }
            return instance;
        }
    }

    private void Awake()
    {
        instance = this;
    }
}
