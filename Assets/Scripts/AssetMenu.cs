using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AssetMenu : MonoBehaviour
{
    public GameObject LoadTarget;
    // Start is called before the first frame update

    //void Start()
    //{
    //    StartCoroutine( ClowderBridge.Instance.GetSpaceDatasets("5b5d42264f0c3808c8cb5b90", result=> {
    //        print( result );
    //        foreach (JSONObject j in result.list)
    //        {
    //            print(j["name"]);
    //            GameObject button = Instantiate( Resources.Load("AssetButton") as GameObject, transform );
    //            button.GetComponentInChildren<Text>().text = j["name"].str;
    //            button.GetComponent<Button>().onClick.AddListener( () => ButtonPressed(j));

    //        }
    //    }));
    //}

    void ButtonPressed( JSONObject json ) {
        print ( json );
        foreach ( Transform t in LoadTarget.transform ) {
            Destroy( t.gameObject );
        }
        GameObject wrapper = new GameObject();
        wrapper.transform.parent = LoadTarget.transform;

        
        ClowderBridge.Instance.DownloadFromURL( "https://cyprus.ncsa.illinois.edu/clowder/datasets/" + json["id"].str, wrapper );
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ClearChildren( GameObject obj )
    {
        Debug.Log( obj.transform.childCount);
        int i = 0;

        //Array to hold all child obj
        GameObject[] allChildren = new GameObject[obj.transform.childCount];

        //Find all child obj and store to that array
        foreach (Transform child in obj.transform)
        {
            allChildren[i] = child.gameObject;
            i += 1;
        }

        //Now destroy them
        foreach (GameObject child in allChildren)
        {
            DestroyImmediate(child.gameObject);
        }

        // Debug.Log(obj.transform.childCount);
    }
}

