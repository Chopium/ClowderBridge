using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockyFaceRotate : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    float newAngle;
    Vector3 workingAngle;
    // Update is called once per frame
    private void Update()
    {
        //var lookDelta = Camera.main.transform.position - this.transform.position;
        
        ////this.transform.localRotation =
        //workingAngle = Quaternion.FromToRotation(-transform.up.normalized, lookDelta.normalized).eulerAngles;
        //Debug.Log(newAngle);
        //workingAngle.y = Mathf.Round(workingAngle.y / 90) * 90;
        ////newAngle = Mathf.Round(workingAngle.y / 90) * 90;
        //Debug.Log(newAngle);
        //this.transform.localRotation = Quaternion.Euler(90,workingAngle.y,0);
        this.transform.LookAt(Camera.main.transform.position, Camera.main.transform.forward);
    }
}
