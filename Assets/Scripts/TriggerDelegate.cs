using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDelegate : MonoBehaviour
{
    public UnityEvent TriggerEnter;

    private void OnTriggerEnter(Collider c)
    {
        if (c.gameObject.CompareTag("Player"))
        {
            TriggerEnter.Invoke();
        }
        
    }
}
