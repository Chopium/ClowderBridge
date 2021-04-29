using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//[ExecuteInEditMode] //this means an instance of the script will run in edit mode, for debuging purposes
public class LoadTotem : MonoBehaviour
{
    public GameObject totemPrefab;
    GameObject objectWrapper;

    public string clowderID;
    public string fileName;

    public bool loaded;

    private static List<LoadTotem> localTotems;
    public static List<LoadTotem> Totems {
        get {
            if ( localTotems == null ) {
                localTotems = new List<LoadTotem>();
            }
            return localTotems;
        }
    }
    public void DestroySelf()
    {
        Destroy(this.gameObject);
    }
    GameObject CloseTotem;

    public void Awake() {

        ClowderBridge.SceneCleared += DestroySelf; //when we clear the scene, we will want to delete any placed totems as well
        Totems.Add( this );
        dissolveAmount = 0;

        foreach ( Transform child in totemPrefab.transform ) {
            SetDissolve();
        }
    }
    void OnDestroy() {
        Totems.Remove( this );
        print("Totem Destroyed");
    }

    float dissolveAmount = 0;

    
    void Update() {

        //SetDissolve();
        //dissolveAmount += 1f * Time.deltaTime;
        //dissolveAmount = Mathf.Clamp( dissolveAmount, 0, 1 );
    }

    public void SetDissolve() {
        foreach ( Transform child in totemPrefab.transform ) {
            if ( child.gameObject.GetComponent<Renderer>() ) {
            child.gameObject.GetComponent<Renderer>().material.SetFloat("_Amount", 1-dissolveAmount  );
            }
        }
    }
    public void LoadObject() {
#if TRILIB_ENABLED
        if ( ClowderBridge.Instance.CurrentLoader != null && ClowderBridge.Instance.CurrentLoader.ObjectLoaded == false ) {
            return;
        }
#endif
        // print(  PerformanceSettings.Instance.maxObjects  );
        // print("loaded assets: " + ClowderBridge.Instance.LoadedAssets.Count);

        //Check if current object count exceeds PerformanceSettings
        if ( ClowderBridge.Instance.LoadedAssets != null && ClowderBridge.Instance.LoadedAssets.Count >= PerformanceSettings.Instance.maxObjects && !loaded ) {
            //ClowderBridge.Instance.reportAssets();
            //Destroy oldest model
            GameObject oldestObject = ClowderBridge.Instance.LoadedAssets[0];
            print( "oldest object " + oldestObject);
            foreach ( Transform t in oldestObject.transform ) {
                if ( t.gameObject.GetComponent<LoadTotem>() ) {
                    LoadTotem removedTotem = t.gameObject.GetComponent<LoadTotem>();
                    removedTotem.DestroyModel();
                }
            }
            ClowderBridge.Instance.LoadedAssets.RemoveAt(0);
        }

        if (CloseTotem == null)
        {
            //CloseTotem = Instantiate(Resources.Load("CloseTotem"), this.transform) as GameObject;
            //CloseTotem.GetComponent<R3D_Deligate>().onSelected += HideObject;
            
            //CloseTotem.transform.parent = this.transform.parent;//make sure this moves
            //CloseTotem.transform.localPosition = Vector3.zero;//make sure this moves
        }

        foreach ( LoadTotem totem in Totems ) {
            totem.totemPrefab.SetActive(true);
            if ( totem != this ) {
                totem.HideObject();
            }

        }
        if ( !loaded ) {
            objectWrapper = new GameObject( clowderID );
            objectWrapper.transform.position = transform.position;
            objectWrapper.transform.rotation = transform.rotation;

            transform.SetParent(objectWrapper.transform);

            ClowderBridge.Instance.DownloadFromURL( clowderID, objectWrapper );
            loaded = true;
        }
        ShowObject();

    }
    public void GUIHideObject()
    {
#if TRILIB_ENABLED
        if (ClowderBridge.Instance.CurrentLoader != null && ClowderBridge.Instance.CurrentLoader.ObjectLoaded == false)
        {
            HideObject();
        }
#endif
    }

    public void DestroyModel() {
        transform.parent = null;
        Destroy(objectWrapper.transform.root.gameObject);
        objectWrapper = null;
        loaded = false;
    }

    public void ShowObject() {
        if ( objectWrapper ) {
            foreach( Transform child in objectWrapper.transform ) {
                if ( child != this.transform ) {
                    child.gameObject.SetActive(true);
                }
            }
        }
        dissolveAmount = 0;
        SetDissolve();
        if (CloseTotem != null)
        {
            CloseTotem.SetActive(true);
            CloseTotem.transform.parent = this.transform.parent;
        }



            totemPrefab.SetActive(false);
    }
    public void HideObject() {
        if ( objectWrapper ) {
            foreach( Transform child in objectWrapper.transform ) {
                if ( child != this.transform ) {
                    child.gameObject.SetActive(false);
                }
            }
        }
        if ( totemPrefab == false ) {

        }
        if ( CloseTotem != null ) {
            CloseTotem.SetActive(false);
        }
        totemPrefab.SetActive( true);

    }


}
