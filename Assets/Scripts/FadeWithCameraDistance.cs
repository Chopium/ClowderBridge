using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FadeWithCameraDistance : MonoBehaviour
{
   Renderer[] thisMaterial;
    TextMeshPro[] thisTextMesh;

    Vector3 originalScale;
    Color thisColor;
    // Start is called before the first frame update
    void Awake()
    {
        // thisMaterial = this.GetComponent<Renderer>().material;
        //originalScale = this.transform.localScale;
        thisMaterial = this.GetComponentsInChildren<Renderer>();
        thisTextMesh = this.GetComponentsInChildren<TextMeshPro>();
        thisColor = this.GetComponentInChildren<Renderer>().material.color;

    }
    float distance;
    // Update is called once per frame
    void Update()
    {
        //also we should default to non-animated interactivity for photos without metadata!
        distance = Vector3.Distance(Camera.main.transform.position, this.transform.position);


        //rough distance test
        if (distance < 1)
        {
            float opacityFactor = (Mathf.Lerp(-1,1,(Vector3.Distance(Camera.main.transform.position, this.transform.position))));

            foreach (Renderer r in thisMaterial)
            {
                r.material.color = thisColor * new Color(1, 1, 1, opacityFactor);
            }
            foreach (TextMeshPro t in thisTextMesh)
            {
                t.color = thisColor * new Color(1, 1, 1, opacityFactor);
            }
        }
        //thisMaterial.color = new Color(1, 1, 1, Mathf.Abs(Quaternion.Dot(this.transform.rotation, Camera.main.transform.rotation)));
        //this.transform.localScale = originalScale * Mathf.Abs(Quaternion.Dot(this.transform.rotation, Camera.main.transform.rotation));
    }
}
