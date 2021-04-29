using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DownloadWrapper : MonoBehaviour
{
    public Transform t;
    public float desiredSize = 0.03f;
    public bool moved = false;
    public string fileID;

    // Update is called once per frame
    void Update()
    {
        if ( GetComponentInChildren<MeshFilter>() && !moved ) {
            // wrapper.transform.position = wrapper.GetComponentInChildren<Renderer>().bounds.center * -0.5f;
            Vector3 size = GetComponentInChildren<MeshFilter>().mesh.bounds.size;
            float maxDimension = Mathf.Max( Mathf.Max( size.x, size.y ), size.z );
            float scaleMultiplier = desiredSize / maxDimension;
            transform.localScale = Vector3.one * scaleMultiplier;

            Vector3 whereYouWantMe = t.position;
            Vector3 offset = transform.position - transform.TransformPoint(GetComponentInChildren<MeshFilter>().mesh.bounds.center);
            transform.position = whereYouWantMe + offset;
        

            moved = true;
        }
    }
}
