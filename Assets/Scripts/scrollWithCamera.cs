using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scrollWithCamera : MonoBehaviour
{
    public Vector2 ScrollSensitivity = new Vector2(.04f, .04f);
    public Vector2 textureOffset = Vector2.zero;
    public Material scrollingTexture;
    
    //pitch variables
    Vector3 fixedCameraAngle;
    bool upsideDown;
    float upsideDownAdjustment;

    float verticality;

    //azimuth variables
    Quaternion lastCameraRotation = Quaternion.identity;
    Quaternion currentCameraRotation = Quaternion.identity;
    Quaternion deltaCameraRotation = Quaternion.identity;


    // Start is called before the first frame update
    void Start()
    {
        lastCameraRotation = Camera.main.transform.rotation;
    }
    void Update()
    {

        //calculate camera pitch
        fixedCameraAngle = Quaternion.LookRotation(Camera.main.transform.forward, Vector3.up).eulerAngles;
        fixedCameraAngle.x = (fixedCameraAngle.x > 180) ? fixedCameraAngle.x - 360 : fixedCameraAngle.x;


        verticality = Vector3.Dot(Camera.main.transform.up.normalized, Vector3.up);
        if (verticality > 0)//this section handles when the camera flips upside down
        {
            upsideDown = false;
            //normal vertical scroll update
            textureOffset.y = -fixedCameraAngle.x * ScrollSensitivity.y;
        }
        else 
        {
            if (!upsideDown)//if first frame of upside down
            {
                upsideDown = true;
                //calculate offset
                upsideDownAdjustment = textureOffset.y - (fixedCameraAngle.x * ScrollSensitivity.y);//adjustment will be added to match previous position. will snap back if we do a 360 instead of undoing the original move!
            }
            textureOffset.y = (fixedCameraAngle.x * ScrollSensitivity.y) + upsideDownAdjustment;
        }
        //Debug.Log(((Mathf.Cos(verticality * Mathf.PI) / 2) + 0.5f));
        //Debug.Log(verticality);

        //calculate azimuth
        currentCameraRotation = Camera.main.transform.rotation;
        if (currentCameraRotation != lastCameraRotation)
        {
            deltaCameraRotation = currentCameraRotation * Quaternion.Inverse(lastCameraRotation);

            fixedCameraAngle = deltaCameraRotation.eulerAngles;
            fixedCameraAngle.y = (fixedCameraAngle.y > 180) ? fixedCameraAngle.y - 360 : fixedCameraAngle.y;

            if (!upsideDown)
            {
                textureOffset.x += fixedCameraAngle.y * ScrollSensitivity.x * Mathf.Abs(verticality);
            }
            else
            {
                textureOffset.x -= fixedCameraAngle.y * ScrollSensitivity.x * Mathf.Abs(verticality);
            }
            
            lastCameraRotation = currentCameraRotation;
        }   

        //update texture position
        scrollingTexture.SetTextureOffset("_MainTex", new Vector2(textureOffset.x, textureOffset.y));

    }

}
