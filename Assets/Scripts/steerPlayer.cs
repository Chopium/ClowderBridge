using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class steerPlayer : MonoBehaviour
{
    public float multiplier = 1;
    private Rigidbody target;
    private void OnTriggerEnter(Collider c)
    {
        if (c.gameObject.CompareTag("Player"))
        {
            target = c.attachedRigidbody;
        }
    }
    private void OnTriggerExit(Collider c)
    {
        if (c.gameObject.CompareTag("Player"))
        {
            target = null;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }
    Vector3 difference;
    float distance;
    // Update is called once per frame
    void FixedUpdate()
    {
        if (target != null)
        {
            
            difference = this.transform.position - target.transform.position;
            distance = Mathf.Pow(500- Mathf.Abs(difference.y),3) * ( Mathf.Abs(difference.x) + Mathf.Abs(difference.z));
            Debug.Log(Mathf.Abs(difference.y));
           // Debug.Log(distance);
            //difference.y = 0;
            difference = difference.normalized;
            target.AddForce(distance * difference * multiplier);
            target.AddForce(-Camera.main.transform.right -Camera.main.transform.up * distance * multiplier);
        }
    }
}
