using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeAgainstCamera : MonoBehaviour
{
    public bool infiniteExpand = false;
    //Material thisMaterial;
    Vector3 originalScale;
    // Start is called before the first frame update
    void Awake()
    {
       // thisMaterial = this.GetComponent<Renderer>().material;
        originalScale = this.transform.localScale;

    }

    // Update is called once per frame
    void Update()
    {
       float rotationalScale = Mathf.Abs(Quaternion.Dot(this.transform.rotation, Camera.main.transform.rotation));
        //thisMaterial.color = new Color(1, 1, 1, Mathf.Abs(Quaternion.Dot(this.transform.rotation, Camera.main.transform.rotation)));
        this.transform.localScale = originalScale * rotationalScale;
        if (infiniteExpand)
        {
            Plane plane = new Plane(Camera.main.transform.forward, Camera.main.transform.position);

            float dist = Mathf.Abs(plane.GetDistanceToPoint(transform.position));
            if (dist < 1f && rotationalScale > 0.75)
            {
                //Debug.Log(Mathf.Abs(dist));
                transform.localScale *= 1 / (dist * dist);
            }
        }
    }
}
