using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tilt_followers : MonoBehaviour
{
    public float maxrotation = 15f;
    float currentx = 0;
    float currenty = 0;
    private void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");


        currentx = Mathf.Clamp(currentx + moveHorizontal, -maxrotation, maxrotation);
        currenty = Mathf.Clamp(currenty -moveVertical, -maxrotation, maxrotation);

        //OuterRing.Rotate(new Vector3(0,0, moveHorizontal));
        //InnerRing.Rotate(new Vector3(-moveVertical, 0, 0));

        OuterRing.localRotation = Quaternion.Euler(0,0, currentx);
        InnerRing.localRotation = Quaternion.Euler(currenty, 0, 0);

        //if(OuterRing.localEulerAngles.z > maxrotation ||)
    }
    //float moveHorizontal = Input.GetAxis("Horizontal");
    //float moveVertical = Input.GetAxis("Vertical");

    //Vector3 movement = new Vector3(-moveHorizontal, 0.0f, -moveVertical);

    //tiltTarget.Rotate(moveVertical, 0f, moveHorizontal, Space.Self);

    public Transform tiltTarget;
    public Transform OuterRing;
    public Transform InnerRing;

}
