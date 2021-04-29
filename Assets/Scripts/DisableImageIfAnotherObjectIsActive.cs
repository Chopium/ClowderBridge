using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableImageIfAnotherObjectIsActive : MonoBehaviour
{
    public GameObject target;
    // Start is called before the first frame update
    void OnEnable()
    {
        if(target.activeInHierarchy)
        {
            this.GetComponent<Renderer>().enabled = false;
            StartCoroutine(waitForMoment());
        }
    }

    IEnumerator waitForMoment()
    {
        while (target.activeInHierarchy)
            yield return null;
        this.GetComponent<Renderer>().enabled = true;
    }
}