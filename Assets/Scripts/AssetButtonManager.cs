using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetButtonManager : MonoBehaviour
{

    public GameObject mainObject;
    public float defaultScale = 1f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ButtonPressed( GameObject obj ) {
        print("Button was pressed woohoo");
    }

    public void UpdateObject( Mesh mesh ) {
        mainObject.GetComponent<MeshFilter>().mesh = mesh;
        Vector3 extents = mainObject.GetComponent<Renderer>().bounds.extents;

        float maxExtent = Mathf.Max( extents.x, Mathf.Max(extents.y,extents.z) );
        float scaleMultiplier = defaultScale / maxExtent;
        Vector3 currentScale = mainObject.transform.localScale;
        mainObject.transform.localScale = scaleMultiplier * currentScale;

        mainObject.transform.localPosition = new Vector3(0, mainObject.GetComponent<Renderer>().bounds.extents.y / 8, 0 );
    }
}
