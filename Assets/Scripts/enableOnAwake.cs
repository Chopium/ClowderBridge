using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enableOnAwake : MonoBehaviour
{
    public List<GameObject> Targets;
    public bool ThisEnabled = true;

    private void Awake()
    {
        if(ThisEnabled && Targets != null)
        {
            foreach (var item in Targets)
            {
                item.SetActive(true);
            }
        }
    }
}
