using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class setMaxAngularVelocity : MonoBehaviour
{
    public float maxVelocity =7f;
    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.GetComponent<Rigidbody>().maxAngularVelocity = maxVelocity;
    }

}
