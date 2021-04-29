using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class disableIfAR : MonoBehaviour
{
    #if !AR_MODE
    void Start()
    {
        this.gameObject.SetActive(false);
        Destroy(this.gameObject);
    }
    #endif

}
