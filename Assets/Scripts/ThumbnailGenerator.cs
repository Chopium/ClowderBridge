using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ThumbnailGenerator : MonoBehaviour
{
    public string fileID = "5d2606f74f0c85e2f58426ff";
    public string url = "";
    GameObject wrap;
    // Start is called before the first frame update
    void Start()
    {
        wrap = new GameObject();
        wrap.transform.position = new Vector3( 5, 2, 1 );
        // ClowderBridge.Instance.;
        // GetComponent<ClowderBridge>().DownloadAsset( fileID, wrap );
        GetComponent<ClowderBridge>().DownloadFromURL( url, wrap );
    }

    // Update is called once per frame
    void Update()
    {
        if ( wrap.GetComponentInChildren<JsonRef>() ) {
            GetThumbnail();
        }
    }
    void GetThumbnail() {
        // AssetPreview.GetAssetPreview( wrap );
        // print("hello");
    }  
}
