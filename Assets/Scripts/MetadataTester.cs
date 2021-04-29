using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetadataTester : MonoBehaviour
{
    public string fileID;
    // Start is called before the first frame update
    void Start()
    {
        JSONObject content = new JSONObject();

        content.AddField("position", "blah");
        content.AddField("rotation", "5,1,5");


        JSONObject metadata = ClowderHelper.CreateMetadataPost( content );


        ClowderBridge clowder = GetComponent<ClowderBridge>();


        Debug.Log( "object " + metadata );
        //clowder.PostMetaData( fileID, metadata);


        // clowder.PostMetaData( fileID, metadata);
        // clowder.DeleteMetaData( fileID, "ncsa.unity");
        // clowder.UpdateMetaData( fileID, metadata);
        // clowder.printMetaData( fileID);
        // StartCoroutine( clowder.GetExtractorMetaData( fileID, "ncsa.unity", result => {
        //     print( "GET RESULT: " + result[0]["content"] );
        // }));
    }

}