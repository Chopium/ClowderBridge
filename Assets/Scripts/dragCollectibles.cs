using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dragCollectibles : MonoBehaviour
{
    Collider SweepCollider;
    // Start is called before the first frame update
    void Start()
    {
        SweepCollider = GetComponent<Collider>();
    }

    // Update is called once per frame
    private void OnTriggerEnter(Collider c)
    {
        if(c.gameObject.layer == 10)
        {
            StartCoroutine(Collect(c.gameObject));
        }
        
    }
    public IEnumerator Collect(GameObject g)
    {
        while (g.activeInHierarchy)
        {
            Debug.Log("MOVING CUE");
            g.transform.position = Vector3.MoveTowards(g.transform.position, this.transform.position, Time.fixedDeltaTime/1000);
            yield return new WaitForFixedUpdate();
        }
    }
}
