using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class disableIfEditor : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        #if UNITY_EDITOR || UNITY_STANDALONE
        this.gameObject.SetActive(false);
        #endif

        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }


}