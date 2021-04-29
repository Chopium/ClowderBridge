using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class Billboard : MonoBehaviour
{
    public bool lockToYAxis = false;
    public Vector3 offset = new Vector3( 0,0,0);

    // Update is called once per frame
    //void Update()
    //{
    //    if ( Camera.main ) {
    //    Vector3 targetDirection =transform.position  - Camera.main.transform.position;
    //    Vector3 newDirection = Vector3.RotateTowards( this.transform.forward, targetDirection, 360, 0 );
    //    Quaternion rotation = Quaternion.LookRotation(newDirection);
    //    transform.rotation = rotation;
    //    }
    //}


    //Orient the camera after all movement is completed this frame to avoid jittering
    [ExecuteAlways]
    void LateUpdate()
    {
        //Quaternion.LookRotation(Camera.main.transform, Quaternion.ide)

        var lookPos = Camera.main.transform.position - transform.position;
        //lookPos.y = 0;
        if(!lockToYAxis)
        {
            this.transform.rotation = Quaternion.LookRotation(-lookPos);
        }
        else
        {
            this.transform.rotation = Quaternion.Euler(0,Quaternion.LookRotation(-lookPos).eulerAngles.y,0);
        }

        //transform.LookAt(Camera.main.transform.position);

        //transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward,
        //    Camera.main.transform.rotation * Vector3.up);
    }
}
