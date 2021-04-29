using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InverseSetGameObjectActive : MonoBehaviour
{
    public GameObject[] targets;

    public void setTargetActive(bool input)
    {
        foreach(GameObject o in targets)
        {
            o.SetActive(!input);
        }
    }
}
