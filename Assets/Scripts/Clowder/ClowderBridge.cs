//using SimpleFileBrowser;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
#if TRILIB_ENABLED
using TriLib;
#endif
using UnityEngine;
using UnityEngine.Networking;
public class ClowderBridge : MonoBehaviour
{
    #region Singleton / Start / Awake
    private static ClowderBridge instance;
    public static ClowderBridge Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<ClowderBridge>();
                if (instance == null)
                {
                    GameObject bridge = new GameObject("ClowderBridge");
                    instance = bridge.AddComponent<ClowderBridge>();

                }
            }
            return instance;
        }
    }

    void Awake()
    {
        instance = this;
#if TRILIB_ENABLED
        ClowderLoadOptions = ScriptableObject.CreateInstance<AssetLoaderOptions>();
        #endif
    }
    void Start()
    {
    }
    protected void OnGUI()
    {
        //var centeredRect = new Rect(Screen.width / 2f - 100f, Screen.height / 2f - 25f, 200f, 50f);
        //if (!FinishedLoading)
        //{

        //    GUIStyle loadstyle = new GUIStyle();
        //    loadstyle.fontSize = 30;
        //    loadstyle.normal.textColor = Color.white;
        //    GUI.Label(centeredRect, "LOADING", loadstyle);
        //}
    }
#endregion

    public enum ClowderType { unassigned, file, dataset, space, folder, collection };
    public string CLOWDERURL = "https://cyprus.ncsa.illinois.edu/clowder/api/";
    public string API_KEY = string.Empty;

#region Scene Management
    public List<GameObject> LoadedAssets;

    public delegate void SceneClearedDelegate();
    public static event SceneClearedDelegate SceneCleared;
    //unload all clowder assets in scene
    public void ClearScene()
    {
        if (LoadedAssets.Count < 1)
        {
            Console.Instance.sendCommand("reload");
        }
        foreach (GameObject o in LoadedAssets)
        {
            Destroy(o.transform.root.gameObject);//we added additional wrappers to datasets so we destroy the tranform root (highest level) of each asset.
        }
        GameObject[] totems = GameObject.FindGameObjectsWithTag("Totem");
        foreach (GameObject o in totems)
        {
            Destroy(o);
        }
        LoadedAssets.Clear();
        //QRScanner.Instance.showcrop = true;

        SceneCleared();
    }
    //destory target after so many seconds
    IEnumerator DelayedDestroy(Component target, int seconds)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(target);
    }

#endregion

#region GET, POST, PUT, DELTE / Server Commands
    public IEnumerator GetRequest(string uri, System.Action<JSONObject> callback = null)
    {
        //bool requestFinished = false;
        bool requestErrorOccurred = false;

        //UnityWebRequest request = UnityWebRequest.Get(uri+ "?key="+ API_KEY);
        UnityWebRequest request = UnityWebRequest.Get(uri + "&key=" + API_KEY);
        //Debug.Log(uri + "?key=" + API_KEY);
        yield return request.SendWebRequest();

        //requestFinished = true;
        if (request.isNetworkError)
        {
            Debug.Log("Something went wrong, and returned error: " + request.error);
            requestErrorOccurred = true;
        }
        else
        {

            if (request.responseCode == 200) //success
            {

                JSONObject j = new JSONObject(request.downloadHandler.text);
                callback(j);

            }
            else if (request.responseCode == 401) // an occasional unauthorized error
            {
                Debug.Log("Error 401: Unauthorized.");
                requestErrorOccurred = true;
            }
            else
            {
                Debug.Log("Request failed (status:" + request.responseCode + ")");
                requestErrorOccurred = true;
            }

            if (!requestErrorOccurred)
            {
                yield return null;
                // process results
            }
        }
    }
    //public IEnumerator PostFileRequest(string filePath, string clowderID, System.Action<JSONObject> callback = null)
    //{
    //    List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
    //    byte[] bytes = File.ReadAllBytes(filePath);
    //    //files[0] = files[0].Replace(@".\", "");
    //    formData.Add(new MultipartFormFileSection("file", bytes, FileBrowserHelpers.GetFilename(filePath), MimeTypeLookup.GetMimeType(filePath)));
    //    //StartCoroutine(UploadFile(formData));
    //    //FileBrowserHelpers.Get
    //    string url = CLOWDERURL + "uploadToDataset/" + clowderID + "?extract=true&key=" + API_KEY;
    //    UnityWebRequest www = UnityWebRequest.Post(url, formData);
    //    yield return www.SendWebRequest();

    //    if (www.isNetworkError || www.isHttpError)
    //    {
    //        Debug.Log(www.error);
    //    }
    //    else
    //    {
    //        Debug.Log("Form upload complete!");
    //    }
    //}
    public IEnumerator PostJSONasFileRequest(JSONObject report, string name, string clowderID, System.Action<JSONObject> callback = null)
    {
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        //byte[] bytes = File.ReadAllBytes(filePath);

        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(report.Print(true));


        //files[0] = files[0].Replace(@".\", "");
        formData.Add(new MultipartFormFileSection("file", bytes, name + ".json", MimeTypeLookup.GetMimeType(".json")));
        string url = CLOWDERURL + "uploadToDataset/" + clowderID + "?extract=true&key=" + API_KEY;
        UnityWebRequest www = UnityWebRequest.Post(url, formData);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log("Form upload complete!");
        }
    }

    public IEnumerator PostRequest(string url, JSONObject report, System.Action<JSONObject> callback = null)
    {
        Debug.Log("inside post request");
        // bool requestFinished = false;
        bool requestErrorOccurred = false;

        var request = new UnityWebRequest(url, "POST");
        //bodyJsonString.str

        byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(report.ToString()); //convert from jsonobject format to complete json string

        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("context", "unity_clowder");
        request.SetRequestHeader("agent", "wikar");

        //UnityWebRequest request = UnityWebRequest.Get(uri+ "?key="+ API_KEY);
        //UnityWebRequest request = UnityWebRequest.Get(uri + "&key=" + API_KEY);
        //Debug.Log(uri + "?key=" + API_KEY);
        Debug.Log(request.ToString());
        yield return request.SendWebRequest();
        // requestFinished = true;

        //requestFinished = true;
        if (request.isNetworkError)
        {
            Debug.Log("Something went wrong, and returned error: " + request.error);
            requestErrorOccurred = true;
        }
        else
        {

            if (request.responseCode == 200) //success
            {

                //string output = JsonConvert.SerializeObject(request.downloadHandler.text);
                //Debug.Log("Request Success : " + uri);
                //new GameObject myObject = JsonUtility.FromJson<MyClass>(json);
                // Show results as text
                Debug.Log("success: code 200");
                JSONObject j = new JSONObject(request.downloadHandler.text);
                callback(j);
                //accessData(j);

            }
            else if (request.responseCode == 401) // an occasional unauthorized error
            {
                Debug.Log("Error 401: Unauthorized.");
                //StartCoroutine(GetRequest(GenerateRequestURL(lastRequestURL, lastRequestParameters)));
                requestErrorOccurred = true;
            }
            else
            {
                Debug.Log("Request failed (status:" + request.responseCode + ")");
                Debug.Log(request.error);
                requestErrorOccurred = true;
            }

            if (!requestErrorOccurred)
            {
                yield return null;
                // process results
            }
        }
    }
    public IEnumerator DeleteRequest(string url, System.Action<bool> callback = null)
    {
        bool requestErrorOccurred = false;

        var request = UnityWebRequest.Delete(url);

        //Debug.Log(request.ToString());
        yield return request.SendWebRequest();

        if (request.isNetworkError)
        {
            Debug.Log("Something went wrong, and returned error: " + request.error);
            requestErrorOccurred = true;
        }
        else
        {

            if (request.responseCode == 200) //success
            {
                Debug.Log("success: code 200");
                callback(true);
            }
            else if (request.responseCode == 401) // an occasional unauthorized error
            {
                Debug.Log("Error 401: Unauthorized.");
                requestErrorOccurred = true;
            }
            else
            {
                Debug.Log("Request failed (status:" + request.responseCode + ")");
                Debug.Log(request.error);
                requestErrorOccurred = true;
            }

            if (!requestErrorOccurred)
            {
                yield return null;
            }
        }
    }
#endregion

#region Download (Datasets, Assets, Content)
#if TRILIB_ENABLED
    public R3D_AssetDownloader CurrentLoader;
    public AssetLoaderOptions ClowderLoadOptions;
#endif

    public float AssetWaitTime = 2.5f;

    public void DownloadFromURL(string url, GameObject Wrapper = null)
    {
        //https://cyprus.ncsa.illinois.edu/clowder/datasets/5d24c37e4f0c0635b9b0c58f sample
        List<string> array = new List<string>(url.Split('/', '?')); //get tokens between / and ?
        if (array.Count > 1)//URL TESTING
        {
            for (int i = 0; i < array.Count; i++)
            {
                if (array[i].Equals("files") && array.Count > i)//if url points to file
                {
                    Debug.Log("Downloading File: " + array[i + 1]);
                    Wrapper.name = array[i + 1];
                    DownloadAsset(array[i + 1], Wrapper);
                }
                else if (array[i].Equals("datasets") && array.Count > i)//else if its identified a dataset
                {
                    Debug.Log("Downloading Dataset: " + array[i + 1]);
                    if (Wrapper == null)//if we don't have a totem, create a empty representation of the dataset
                    {
                        GameObject DatasetOffsetWrapper = new GameObject(array[i + 1] + " (wrapper)");
                        GameObject DatasetWrapper = new GameObject(array[i + 1]);
                        DatasetWrapper.transform.parent = DatasetOffsetWrapper.transform;
                        Wrapper = DatasetWrapper;
                    }
                    else
                    {
                        GameObject DatasetOffsetWrapper = new GameObject(array[i + 1] + " (wrapper)");
                        DatasetOffsetWrapper.transform.localPosition = Wrapper.transform.localPosition;
                        DatasetOffsetWrapper.transform.localRotation = Wrapper.transform.localRotation;
                        Wrapper.name = array[i + 1];
                        Wrapper.transform.parent = DatasetOffsetWrapper.transform;
                    }
                    StartCoroutine(DownloadDataset(array[i + 1], Wrapper));
                    LoadedAssets.Add(Wrapper);//add the dataset to the list of assets

                    //get the dataset to load its metadata from clowder
                    AddMetaData(array[i + 1], Wrapper, ClowderType.dataset);
                }
                else if (array[i].Equals("prefab") && array.Count > i)//else if its identified a dataset
                {
                    Debug.Log("Instantiating Local Prefab: " + array[i + 1]);
                    if (Wrapper == null)//if we don't have a totem, create a empty representation of the dataset
                    {
                        GameObject DatasetWrapper = new GameObject(array[i + 1]);
                        Wrapper = DatasetWrapper;
                    }
                    else
                    {
                        Wrapper.name = array[i + 1];
                    }
                    //StartCoroutine(InstantiateLocalResource(array[i + 1], Wrapper));

                    GameObject loadedResource = (Resources.Load(array[i + 1]) as GameObject);
                    if (loadedResource != null)
                    {
                        loadedResource = GameObject.Instantiate(loadedResource);
                        loadedResource.transform.parent = Wrapper.transform;
                        loadedResource.transform.localPosition = Vector3.zero;
                        loadedResource.transform.localRotation = Quaternion.identity;

                    }
                    else
                    {
                        Destroy(Wrapper);
                        Debug.Log(array[i + 1] + " was not found in resource directory");
                    }
                }
            }
        }
        else
        {
            if (url.Equals("test"))
            {
                url = "5e443e474f0c4fa40f60aed7";
            }
            Debug.Log("Downloading Dataset: " + url);
            if (Wrapper == null)//if we don't have a totem, create a empty representation of the dataset
            {
                GameObject DatasetOffsetWrapper = new GameObject(url + " (wrapper)");
                GameObject DatasetWrapper = new GameObject(url);
                DatasetWrapper.transform.parent = DatasetOffsetWrapper.transform;
                Wrapper = DatasetWrapper;
            }
            else
            {
                GameObject DatasetOffsetWrapper = new GameObject(url + " (wrapper)");
                DatasetOffsetWrapper.transform.localPosition = Wrapper.transform.localPosition;
                DatasetOffsetWrapper.transform.localRotation = Wrapper.transform.localRotation;
                Wrapper.name = url;
                Wrapper.transform.parent = DatasetOffsetWrapper.transform;
            }
            StartCoroutine(DownloadDataset(url, Wrapper));
            LoadedAssets.Add(Wrapper);//add the dataset to the list of assets

            //get the dataset to load its metadata from clowder
            AddMetaData(url, Wrapper, ClowderType.dataset);
        }

    }

    //Download image file and place it in world
    public void DownloadAssetBundle(string ClowderID, GameObject Wrapper)
    {
        string url = CLOWDERURL + "files/" + ClowderID + "/metadata" + "?key=" + API_KEY;
        StartCoroutine(GetRequest(url, returnValue =>
        {
            string filename = returnValue.GetField("filename").str;

            //grab file extension from filename
            string[] parts = filename.Split('.');
            string extension = parts[parts.Length - 1]; //get file extension

            print("Extension: " + extension);

            //create new target to hold the asset
            if (Wrapper != null)
            {
                //create new target to hold the asset
                GameObject Wrapper2 = new GameObject(ClowderID);
                Wrapper2.transform.parent = Wrapper.transform;
                Wrapper = Wrapper2;
            }
            else
            {
                Wrapper = new GameObject(ClowderID);
            }

            url = CLOWDERURL + "files/" + ClowderID + "/blob" + "?key=" + API_KEY;
            //print(textureURL);
            StartCoroutine(GetAssetBundle(url, Wrapper, loadedAssetBundle =>
            {

                ////   GameObject loadedTexture = Instantiate(Resources.Load("GrabbableImage") as GameObject, Wrapper.transform);
                //GameObject loadedTexture = GameObject.CreatePrimitive(PrimitiveType.Quad);
                //loadedAssetBundle.transform.parent = Wrapper.transform;
                //loadedTexture.GetComponent<Renderer>().material.shader = Shader.Find("Unlit/FadeTexture");


                //loadedTexture.GetComponent<Renderer>().material.SetTexture("_MainTex", textureReturn);
                ////loadedTexture.GetComponent<Renderer>().material.SetFloat("_Glossiness", 0f);
                //Wrapper.transform.localEulerAngles = new Vector3(0, 180, 0);
                ////Wrapper.transform.localScale = new Vector3(0.25f * ((float)textureReturn.width / (float)textureReturn.height), 0.25f, 0.25f); //we are moving this to a local photo thing

                //Wrapper.AddComponent<R3D_Photo>();
                Wrapper.AddComponent<R3D_Transform>();


                AddMetaData(ClowderID, Wrapper, ClowderType.file);
                //LoadedAssets.Add(Wrapper);
                //loadedTexture.GetComponent<GrabbableImage>().SetTexture(textureReturn);
            }));
        }));
    }


    public IEnumerator DownloadAssetMultiple(JSONObject result, GameObject Wrapper)
    {
        foreach (JSONObject j in result.list)
        {
            print("Downloading Dataset File: " + j["id"]);
            DownloadAsset(j.GetField("id").str, Wrapper);
            yield return new WaitForSeconds(AssetWaitTime);//generic wait time
        }
    }

    //Downloads an assetbundle from URL
    IEnumerator GetAssetBundle(string url, GameObject Wrapper, System.Action<GameObject> callback = null)
    {
        //string url = CLOWDERURL + "files/" + ClowderID + "/blob" + "?key=" + API_KEY;
        //print(url);
        UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(url);//UnityWebRequestMultimedia.GetAudioClip(url, audioType);


        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            // Get an asset from the bundle and instantiate it.
            AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(www);
            //var loadAsset = bundle.LoadAssetAsync<GameObject>("Assets/Players/MainPlayer.prefab");
            var loadAsset = bundle.LoadAllAssetsAsync();
            yield return loadAsset;


            //GameObject loadedBundle = Instantiate(loadAsset)as GameObject;
            GameObject loadedBundle = Instantiate(loadAsset.asset) as GameObject;
            loadedBundle.transform.parent = Wrapper.transform;
            loadedBundle.transform.localRotation = Quaternion.identity;
            loadedBundle.transform.localPosition = Vector3.zero;
            var UnloadAssetbundle = loadedBundle.AddComponent<UnloadAssetBundle>();
            UnloadAssetbundle.target = bundle;
            //Instantiate(loadedBundle);
            callback(loadedBundle);
        }

    }
    IEnumerator AdjustVideoAspectRatio(UnityEngine.Video.VideoPlayer vp)
    {
        while (!vp.isPrepared)
        {
            yield return null;
        }
        float aspectRatioY = (float)vp.height / (float)vp.width;
        print(aspectRatioY);
        Vector3 adjustedScale = vp.transform.localScale;
        adjustedScale.y *= aspectRatioY;
        vp.transform.localScale = adjustedScale;

        vp.Play();
        yield break;

    }
    //Download image file and place it in world
    public void DownloadVideo(string ClowderID, GameObject Wrapper)
    {
        string url = CLOWDERURL + "files/" + ClowderID + "/metadata" + "?key=" + API_KEY;
        StartCoroutine(GetRequest(url, returnValue =>
        {
            string filename = returnValue.GetField("filename").str;

            //grab file extension from filename
            string[] parts = filename.Split('.');
            string extension = parts[parts.Length - 1]; //get file extension

            print("extension: " + extension);

            //create new target to hold the asset
            if (Wrapper != null)
            {
                //create new target to hold the asset
                GameObject Wrapper2 = new GameObject(ClowderID);
                Wrapper2.transform.parent = Wrapper.transform;
                Wrapper = Wrapper2;
            }
            else
            {
                Wrapper = new GameObject(ClowderID);
            }

            string videoURL = CLOWDERURL + "files/" + ClowderID + "/blob" + "?key=" + API_KEY;
            print(videoURL);

            GameObject loadedVideo = GameObject.CreatePrimitive(PrimitiveType.Quad);
            loadedVideo.transform.parent = Wrapper.transform;
            loadedVideo.GetComponent<Renderer>().material.shader = Shader.Find("Unlit/Texture");
            Wrapper.transform.localEulerAngles = new Vector3(0, 180, 0);

            var vp = loadedVideo.AddComponent<UnityEngine.Video.VideoPlayer>();
            vp.url = videoURL;



            //vp.transform.localScale = new Vector3(0.25f * ((float)vp.width / (float)vp.height), 0.25f, 0.25f); //we are moving this to a local photo thing
            vp.isLooping = true;
            vp.renderMode = UnityEngine.Video.VideoRenderMode.MaterialOverride;
            vp.targetMaterialRenderer = GetComponent<Renderer>();
            vp.targetMaterialProperty = "_MainTex";
            StartCoroutine(AdjustVideoAspectRatio(vp));

            //vp.transform.localScale = new
            //print(vp.clip.height);
            //print(vp.clip.width);
            //Texture videoTexture = vp.texture;
            //print(videoTexture.width);
            //print(vp.aspectRatio);

            //float aspectRatioY = (float)vp.clip.height / (float)vp.clip.width;
            //print(aspectRatioY);
            //Vector3 adjustedScale = vp.transform.localScale;
            //adjustedScale.y *= aspectRatioY;
            //vp.transform.localScale = adjustedScale;

            Wrapper.AddComponent<R3D_Transform>();
#if TRILIB_ENABLED
            Wrapper.AddComponent<AssetUnloader>();
#endif
            AddMetaData(ClowderID, Wrapper, ClowderType.file);
        }));
    }


    public void DownloadAsset(string ClowderID, GameObject Wrapper = null)
    {
        string url = CLOWDERURL + "files/" + ClowderID + "/metadata" + "?key=" + API_KEY;

        //print("Downloading!");
        StartCoroutine(GetRequest(url, returnValue =>
        {
            // print( "asset metadata: " + returnValue );
            string filename = returnValue.GetField("filename").str;

            //grab file extension from filename
            string[] parts = filename.Split('.');
            string extension = parts[parts.Length - 1]; //get file extension

            //print("extension: " + extension);

            string contentType = returnValue.GetField("content-type").str;
            string type = contentType.Split('/')[0];
            string format = contentType.Split('/')[1];
            //print( "content type: " + contentType + "   " + contentType.Split('/')[0] );


            if (extension.Equals("assetbundle-android"))
            {
                #if UNITY_ANDROID
                Debug.Log("Downloading Android Asset Bundle");
                DownloadAssetBundle(ClowderID, Wrapper);
                #endif
                return;
            }
            if (extension.Equals("assetbundle-ios"))
            {
                #if UNITY_IOS
                Debug.Log("Downloading iOS Asset Bundle");
                DownloadAssetBundle(ClowderID, Wrapper);
                #endif
                return;
            }

            if (type == "audio")
            {
                DownloadAudio(ClowderID, format, Wrapper);
                return;
            }
            else if (type == "image")
            {
                DownloadImage(ClowderID, Wrapper);
                return;
            }
            else if (type == "video")
            {
                //print("video");
                DownloadVideo(ClowderID, Wrapper);
                return;
            }


            GameObject OriginalWrapper = Wrapper;
            //create new target to hold the asset
            if (Wrapper == null)
            {
                Wrapper = new GameObject(ClowderID);
                // Wrapper.transform.parent = LoadTarget.transform;
                //GameObject obj = new GameObject(ClowderID);
                //obj.transform.parent = Wrapper.transform;
            }
            else
            {
                GameObject Wrapper2 = new GameObject(ClowderID);
                Wrapper2.transform.SetParent(Wrapper.transform);
                Wrapper = Wrapper2;

                //Wrapper.name = ClowderID;
                //Wrapper.transform.parent = LoadTarget.transform;

            }

            // Wrapper.transform.localPosition = new Vector3( 0, 0, 0 );


            //Get transform metadata and apply it
            // StartCoroutine( GetExtractorMetaData(ClowderID, "ncsa.unity", result => {
            //     print ( " Extractor Metadata " + result[0]["content"]["R3D_Transform"] );
            //     R3D_Transform r3dt = Wrapper.AddComponent<R3D_Transform>();
            //     r3dt.ApplyJson(result[0]["content"]["R3D_Transform"]);
            // }));

            //used to use a singleton for download, now we spawn a new download agent with each call here
            //R3D_AssetDownloader.Instance.DownloadAsset(CLOWDERURL + "files/" + ClowderID + "?key=" + API_KEY, extension, null, null, null, Wrapper);

            GameObject root = new GameObject("root");
            root.transform.parent = Wrapper.transform;
            root.transform.position = Wrapper.transform.position;
            root.transform.rotation = Wrapper.transform.rotation;

#if TRILIB_ENABLED
            R3D_AssetDownloader downloader = this.gameObject.AddComponent<R3D_AssetDownloader>();
            CurrentLoader = downloader;

            //set up asset loader options
            ClowderLoadOptions.name = ClowderID;
            ClowderLoadOptions.DontLoadCameras = true;
            ClowderLoadOptions.UseOriginalPositionRotationAndScale = true;
            ClowderLoadOptions.AdvancedConfigs.Add(new AssetAdvancedConfig(AssetAdvancedPropertyMetadata.GetConfigKey(AssetAdvancedPropertyClassNames.SplitLargeMeshesVertexLimit), 1000));
            ClowderLoadOptions.AdvancedConfigs.Add(new AssetAdvancedConfig(AssetAdvancedPropertyMetadata.GetConfigKey(AssetAdvancedPropertyClassNames.SplitLargeMeshesTriangleLimit), 1000));
            ClowderLoadOptions.AdvancedConfigs.Add(new AssetAdvancedConfig(AssetAdvancedPropertyMetadata.GetConfigKey(AssetAdvancedPropertyClassNames.FBXImportReadLights), false));
            ClowderLoadOptions.AdvancedConfigs.Add(new AssetAdvancedConfig(AssetAdvancedPropertyMetadata.GetConfigKey(AssetAdvancedPropertyClassNames.FBXImportReadCameras), false));
            Debug.Log(ClowderLoadOptions.AdvancedConfigs.Count);

            //if (Wrapper != null)
            //{
            //    ClowderLoadOptions.RotationAngles = Wrapper.transform.rotation.eulerAngles;
            //}

            //downloader.basicOptions.AddAssetUnloader = true;

            CurrentLoader.ShowProgress = true;
            CurrentLoader.Timeout = 10000000;
            CurrentLoader.Async = true;
            CurrentLoader.DownloadAsset(CLOWDERURL + "files/" + ClowderID + "?key=" + API_KEY, extension, ObjectLoaded, null, ClowderLoadOptions, root, null);
#endif
            //Debug.Log("passed");
            //print("object was added");
            Wrapper.AddComponent<R3D_Transform>();
            //Wrapper.AddComponent<MeshCollider>();
            AddMetaData(ClowderID, Wrapper, ClowderType.file);
        }));
    }
    bool FinishedLoading = false;

    public void ObjectLoaded(GameObject obj)
    {
        // Debug.Log( progress );
        print("OBJECT IN HERE");
        FinishedLoading = true;

    }

    //Download image file and place it in world
    public void DownloadImage(string ClowderID, GameObject Wrapper)
    {
        string url = CLOWDERURL + "files/" + ClowderID + "/metadata" + "?key=" + API_KEY;
        StartCoroutine(GetRequest(url, returnValue =>
        {
            string filename = returnValue.GetField("filename").str;

            //grab file extension from filename
            string[] parts = filename.Split('.');
            string extension = parts[parts.Length - 1]; //get file extension

            print("extension: " + extension);

            //create new target to hold the asset
            if (Wrapper != null)
            {
                //create new target to hold the asset
                GameObject Wrapper2 = new GameObject(ClowderID);
                Wrapper2.transform.parent = Wrapper.transform;
                Wrapper = Wrapper2;
            }
            else
            {
                Wrapper = new GameObject(ClowderID);
            }

            string textureURL = CLOWDERURL + "files/" + ClowderID + "/blob" + "?key=" + API_KEY;
            print(textureURL);
            StartCoroutine(GetTexture(textureURL, textureReturn =>
            {

                //   GameObject loadedTexture = Instantiate(Resources.Load("GrabbableImage") as GameObject, Wrapper.transform);
                GameObject loadedTexture = GameObject.CreatePrimitive(PrimitiveType.Quad);
                loadedTexture.transform.parent = Wrapper.transform;
                loadedTexture.GetComponent<Renderer>().material.shader = Shader.Find("Unlit/FadeTexture");


                loadedTexture.GetComponent<Renderer>().material.SetTexture("_MainTex", textureReturn);
                //loadedTexture.GetComponent<Renderer>().material.SetFloat("_Glossiness", 0f);
                Wrapper.transform.localEulerAngles = new Vector3(0, 180, 0);
                //Wrapper.transform.localScale = new Vector3(0.25f * ((float)textureReturn.width / (float)textureReturn.height), 0.25f, 0.25f); //we are moving this to a local photo thing

                //Wrapper.AddComponent<R3D_Photo>();
                Wrapper.AddComponent<R3D_Transform>();
#if TRILIB_ENABLED
                Wrapper.AddComponent<AssetUnloader>();
#endif

                AddMetaData(ClowderID, Wrapper, ClowderType.file);
                //LoadedAssets.Add(Wrapper);
                //loadedTexture.GetComponent<GrabbableImage>().SetTexture(textureReturn);
            }));
        }));
    }

    //Download image file and place it in world
    public void DownloadAudio(string ClowderID, string format = "", GameObject Wrapper = null)
    {
        string url = CLOWDERURL + "files/" + ClowderID + "/metadata" + "?key=" + API_KEY;
        StartCoroutine(GetRequest(url, returnValue =>
        {
            string filename = returnValue.GetField("filename").str;

            //grab file extension from filename
            string[] parts = filename.Split('.');
            string extension = parts[parts.Length - 1]; //get file extension

            //print("extension: " + extension);

            if (Wrapper != null)
            {
                //create new target to hold the asset
                GameObject Wrapper2 = new GameObject(ClowderID);
                Wrapper2.transform.parent = Wrapper.transform;
                Wrapper = Wrapper2;
            }
            else
            {
                Wrapper = new GameObject(ClowderID);
            }

            string audioURL = CLOWDERURL + "files/" + ClowderID + "/blob" + "?key=" + API_KEY;
            print(audioURL);
            StartCoroutine(GetAudio(audioURL, AudioType.MPEG, audioReturn =>
            {
                GameObject sound = new GameObject();
                sound.transform.parent = Wrapper.transform;
                AudioSource source = sound.AddComponent<AudioSource>();
                source.spatialBlend = 1f;
                source.minDistance = 2f;
                source.clip = audioReturn;
                source.loop = true;
                source.Play();
                Wrapper.AddComponent<R3D_Transform>();
#if TRILIB_ENABLED
                Wrapper.AddComponent<AssetUnloader>();
#endif
                //LoadedAssets.Add(Wrapper);
            }));
        }));
    }


    //Downloads preview from route and places it in an object
    public void DownloadPreivew(string route)
    {
        //create new target to hold the asset
        GameObject Wrapper = new GameObject(route);

        string textureURL = "https://cyprus.ncsa.illinois.edu" + route + "?key=" + API_KEY;
        print(textureURL);
        StartCoroutine(GetTexture(textureURL, textureReturn =>
        {
            // GameObject loadedTexture = Instantiate(Resources.Load("GrabbableImage") as GameObject, Wrapper.transform);
            //loadedTexture.GetComponent<GrabbableImage>().SetTexture(textureReturn);
        }));
    }

    //Downloads texture from URL
    IEnumerator GetVideo(string url, System.Action<Texture> callback = null)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);

        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Texture myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            callback(myTexture);
        }
    }

    //Downloads texture from URL
    IEnumerator GetTexture(string url, System.Action<Texture> callback = null)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);

        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Texture myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            callback(myTexture);
        }
    }

    //Downloads texture from URL
    IEnumerator GetAudio(string url, AudioType audioType, System.Action<AudioClip> callback = null)
    {
        UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(url, audioType);

        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            AudioClip myClip = DownloadHandlerAudioClip.GetContent(www);
            callback(myClip);
        }
    }

    //Downloads preview from route and places it in an object

    //Gets all files in dataset and downloads them
    //triggers callback after each texture is received
    public IEnumerator DownloadDataset(string datasetID, GameObject Wrapper, System.Action<Texture> callback = null)
    {
        StartCoroutine(GetRequest(CLOWDERURL + "datasets/" + datasetID + "/files?key=" + API_KEY, result =>
        {
            StartCoroutine(DownloadAssetMultiple(result, Wrapper));
        }));
        yield return null;
    }
#endregion

#region Create / Edit / Delete / Update

    // Creates new empty dataset from name
    // Call as coroutine and get datasetID from callback
    public IEnumerator CreateDataset(string name, string description = "", string spaceID = "5d83d15d4f0c98fd2d58caa0", System.Action<string> datasetID = null)
    {
        JSONObject report = new JSONObject();
        report.AddField("name", name);
        report.AddField("description", description);
        report.AddField("access", "PUBLIC");

        JSONObject space = new JSONObject();
        space.type = JSONObject.Type.OBJECT;
        space.Add(spaceID);
        //spaceField.AddField( "space", space);

        report.AddField("space", space);
        report.AddField("space", space);
        Debug.Log("creating dataset:+ " + report.ToString());
        StartCoroutine(PostRequest(CLOWDERURL + "datasets/createempty?key=" + API_KEY, report, result =>
        {
            print(result);
            datasetID(result["id"].str);
        }));
        yield return null;
    }

    public void AddMetaData(string ClowderID, GameObject Wrapper, ClowderType contentType)//ClowderType meaning datasets, collections, files. made for api calls
    {
        string url = null;
        switch (contentType)
        {
            case ClowderType.file:
                {
                    url = CLOWDERURL + "files/" + ClowderID + "/metadata" + "?key=" + API_KEY;//finds and sets the basic file metadata
                    break;
                }
            case ClowderType.dataset:
                {
                    url = CLOWDERURL + "datasets/" + ClowderID + "?key=" + API_KEY;//finds and sets the basic file metadata
                    break;
                }
        }
        //string url = CLOWDERURL + ClowderType+"/" + ClowderID + ApiCall + "?key=" + API_KEY;//finds and sets the basic file metadata

        //Debug.Log(url);
        JsonRef target = Wrapper.AddComponent<JsonRef>();
        target.clowderType = contentType;
        //print(url);
        StartCoroutine(GetRequest(url, returnValue =>
        {
            target.setJson(returnValue);

            Debug.Log("Request Success : " + url);
            //accessData(returnValue);//print out metadata
        }));
        //target.accessData(target.);
    }
    
    //Deletes all metadata with the same extractor_id and then posts new metadata
    public void UpdateMetaData(string ClowderID, ClowderType contentType, JSONObject report, string CustomextractorID = null)
    {
        string extractorID = "";
        if (CustomextractorID != null)
        {
            extractorID = CustomextractorID;
        }
        else
        {
            //hacky way to get extractor_id from the end of the url
            string idurl = report["agent"]["extractor_id"].str;
            int position = idurl.LastIndexOf("/");
            

            if (position > -1) extractorID = idurl.Substring(position + 1);
            else
            {
                print("extractorID not found");
                return;
            }
        }
        //delete metadata and post new data after its finished
        string deleteURL = "";
        switch (contentType)
        {
            case ClowderType.file:
                {
                    deleteURL = CLOWDERURL + "files/" + ClowderID + "/metadata.jsonld" + "?extractor=" + extractorID + "&key=" + API_KEY;
                    break;
                }
            case ClowderType.dataset:
                {
                    deleteURL = CLOWDERURL + "datasets/" + ClowderID + "/metadata.jsonld" + "?extractor=" + extractorID + "&key=" + API_KEY;
                    break;
                }
        }



        //print (deleteURL);


        StartCoroutine(DeleteRequest(deleteURL, result => {
            PostMetaData(ClowderID, contentType, report);
        }));
    }


    public void DeleteMetaData(string ClowderID, string ExtractorID, ClowderType contentType = ClowderType.file)
    {
        string url;
        if (contentType == ClowderType.file)
        {
            url = CLOWDERURL + "files/" + ClowderID + "/metadata.jsonld" + "?extractor=" + ExtractorID + "&key=" + API_KEY;
        }
        else
        {
            url = CLOWDERURL + "datasets/" + ClowderID + "/metadata.jsonld" + "?extractor=" + ExtractorID + "&key=" + API_KEY;
        }
        

        //print(url);
        StartCoroutine(DeleteRequest(url, result => { }));
    }

    public void PostMetaData(string ClowderID, ClowderType contentType, JSONObject report)
    {
        string url = "";
        switch (contentType)
        {
            case ClowderType.file:
                {
                    url = CLOWDERURL + "files/" + ClowderID + "/metadata.jsonld" + "?key=" + API_KEY;
                    break;
                }
            case ClowderType.dataset:
                {
                    url = CLOWDERURL + "datasets/" + ClowderID + "/metadata.jsonld" + "?key=" + API_KEY;
                    break;
                }
        }

        print(url);
        StartCoroutine(PostRequest(url, report, returnValue =>
        {
            //Debug.Log("end of coroutine: " +returnValue.ToString());
            //print(returnValue.GetField("filename").str + " ID: " + returnValue.GetField("id").str + " description: " + returnValue.GetField("filedescription").str);
            //accessData(returnValue);//print out metadata
        }));
    }
#endregion

#region Search / Report / Fetch (metadata) / Parse
    public string ParseUrlForClowderId(string input)
    {
        string id = input;
        List<string> array = new List<string>(input.Split('/', '?')); //get tokens between / and ?
        for (int i = 0; i < array.Count; i++)
        {
            if (array[i].Equals("files") && array.Count > i)//if url points to file
            {
                id = array[i + 1];
            }
            else if (array[i].Equals("datasets") && array.Count > i)//else if its identified a dataset
            {
                id = array[i + 1];
            }
            else if (array[i].Equals("prefab") && array.Count > i)//else if its identified a dataset
            {
                return null;
            }
        }
        //Debug.Log(id);
        return id;
    }
    public void Search(string type, string query)
    {
        WWWForm form = new WWWForm();
        form.AddField("X-API-Key", API_KEY);

        string url = CLOWDERURL + "search?" + type + "=" + query;
        //WWW request = new WWW(CLOWDERURL, null, headers);
        //Debug.Log(url);
        //StartCoroutine(GetRequest(url, false));

        //download JSON file with references to relevant file ids
        StartCoroutine(GetRequest(url, returnValue =>
        {
            //get references to files
            JSONObject files = returnValue.GetField("results");
            print(files);
            for (int i = 0; i < files.list.Count; i++)
            {
                // printMetaData(files.list[i].str);
                print(files[i]);

            }
        }));
    }
    public void SearchMetadata(string type, string query)
    {


        string url = "";

        if (type == "file")
        {
            url = CLOWDERURL + "search?resource_type=file&unity.R3D_Transform.Position.x&key=" + API_KEY;
        }
        else if (type == "dataset")
        {
            url = CLOWDERURL + "search?resource_type=dataset&unity.LngLat.Lat&key=" + API_KEY;
        }


        JSONObject report = new JSONObject();
        // // report.AddField("GPS_Location", "38.117701, 13.337773");
        // JSONObject r3d = new JSONObject();
        // JSONObject position = new JSONObject();
        // JSONObject x = new JSONObject();
        // x.AddField("X", "-0.1754619");
        // // position.AddField("Position", x );

        // r3d.AddField("Position", x);

        // report.AddField("content", new JSONObject());
        // report["content"].AddField("R3D_Transform", r3d);

        // JSONObject created = new JSONObject();


        // report = JSONObject.Create("{}");
        // created.AddField( "created_at", "Wed Feb 12 15:40:55 CST 2020");
        // created.Add("");

        // report = created;
        //print(report);
        // printMetaData("5e4470e64f0c4fa40f60bd65");
        print(url);
        StartCoroutine(GetRequest(url, returnValue =>
        {
            print(returnValue);
            //Debug.Log("end of coroutine: " +returnValue.ToString());
            //print(returnValue.GetField("filename").str + " ID: " + returnValue.GetField("id").str + " description: " + returnValue.GetField("filedescription").str);
            //accessData(returnValue);//print out metadata
        }));
    }
    public void reportAssets()
    {

        //string report = "";
        foreach (GameObject o in LoadedAssets)
        {
            Debug.Log(o);
            string report = "";
            report += o.name + " - ";
            //report += o.GetComponent<JsonRef>().BlobJson.GetField("filename").str;
            //report += "\\n";
            Debug.Log(report); ;
        }
        return;
    }
    public void PostAllMetadata()
    {
        foreach (GameObject g in LoadedAssets)
        {
            Debug.Log(g.name);
            foreach (JsonRef jr in g.GetComponentsInChildren<JsonRef>())
            {
                //jr.SerializeFields();
                jr.postMetaData(jr.SerializeFields());
            }
        }
    }
    public void printMetaData(string ClowderID)
    {
        string url = CLOWDERURL + "files/" + ClowderID + "/metadata.jsonld" + "?key=" + API_KEY;

        print(url);
        StartCoroutine(GetRequest(url, returnValue =>
        {
            print(returnValue);
        }));
    }
    private void PostSearchResult(JSONObject j)
    {
        JSONObject files = j.GetField("files");

        Debug.Log(j.keys);
    }
    //Coroutine to get metadata with specific extractor_id
    public IEnumerator GetExtractorMetaData(string ClowderID, string ExtractorID, ClowderType contentType, System.Action<JSONObject> callback = null)
    {
        string url = null;
        //Debug.Log(contentType);
        switch (contentType)
        {
            case ClowderType.file:
                {
                    url = CLOWDERURL + "files/" + ClowderID + "/metadata.jsonld" + "?extractor=" + ExtractorID + "&key=" + API_KEY;
                    break;
                }
            case ClowderType.dataset:
                {
                    url = CLOWDERURL + "datasets/" + ClowderID + "/metadata.jsonld" + "?extractor=" + ExtractorID + "&key=" + API_KEY;
                    break;
                }
        }
        //print(url);
        StartCoroutine(GetRequest(url, result => {
            callback(result);
        }));
        yield return null;
    }

    //Gets all files in dataset and then previews for each file
    //triggers callback after each texture is received
    public IEnumerator GetDatasetName(string datasetID, System.Action<string> callback = null)
    {
        //Debug.Log("HERE2");
        StartCoroutine(GetRequest(CLOWDERURL + "datasets/" + datasetID + "/tags?key=" + API_KEY, result =>
        {
            //Debug.Log("HERE3");
            callback(result["name"].str);
        }));
        yield return null;
    }

    //Gets all files in dataset and then previews for each file
    //triggers callback after each texture is received
    public IEnumerator GetDatasetPreviews(string datasetID, System.Action<Texture> callback = null)
    {
        StartCoroutine(GetRequest(CLOWDERURL + "datasets/" + datasetID + "/files?key=" + API_KEY, result =>
        {
            foreach (JSONObject j in result.list)
            {
                print("file id: " + j["id"]);

                StartCoroutine(GetRequest(CLOWDERURL + "files/" + j["id"].str + "/getPreviews" + "?key=" + API_KEY, previewResult =>
                {
                    print(previewResult);
                    if (previewResult[0]["previews"][0]["p_main"].str == "thumbnail-previewer.js")
                    {
                        string textureURL = "https://cyprus.ncsa.illinois.edu" + previewResult[0]["previews"][0]["pv_route"].str + "?key=" + API_KEY;
                        StartCoroutine(GetTexture(textureURL, textureReturn =>
                        {
                            callback(textureReturn);
                        }));
                    }
                }));
            }

        }));
        yield return null;

    }

    //Coroutine to get list of datasets within a space or collection
    public IEnumerator GetDatasetsFromTarget(string ClowderID, ClowderType contentType, System.Action<JSONObject> callback = null)
    {
        string url = null;
        //Debug.Log(contentType);
        switch (contentType)
        {
            case ClowderType.space:
                {
                    //https://cyprus.ncsa.illinois.edu/clowder/api/spaces/5de3ed494f0c7adda3a8ae45/datasets
                    url = CLOWDERURL + "spaces/" + ClowderID + "/datasets" + "?key=" + API_KEY;
                    break;
                }
            case ClowderType.collection:
                {
                    //https://cyprus.ncsa.illinois.edu/clowder/api/collections/5e7e28de4f0cebe5ffa12121/datasets&key=bbad5e1d-3f09-4c42-b460-590edd10a02a
                    //https://cyprus.ncsa.illinois.edu/clowder/api/collections/5e7e28de4f0cebe5ffa12121/datasets
                    url = CLOWDERURL + "collections/" + ClowderID + "/datasets" + "?key=" + API_KEY;
                    Debug.Log(url);
                    break;
                }
        }
        //print(url);
        StartCoroutine(GetRequest(url, result => {
            callback(result);
        }));
        yield return null;
    }

    //Coroutine to get a list of all entites of type on server
    public IEnumerator GetAllOfType(ClowderType contentType, System.Action<JSONObject> callback = null)
    {
        string url = null;
        //Debug.Log(contentType);
        switch (contentType)
        {
            case ClowderType.space:
                {

                    url = CLOWDERURL + "spaces" + "?key=" + API_KEY;
                    Debug.Log(url);
                    break;
                }
            case ClowderType.collection:
                {
                    //https://cyprus.ncsa.illinois.edu/clowder/api/collections/allCollections?limit=99
                    url = CLOWDERURL + "collections/allCollections?limit=99" + "&key=" + API_KEY;
                    break;
                }
        }
        //print(url);
        StartCoroutine(GetRequest(url, result => {
            callback(result);
        }));
        yield return null;
    }

#endregion
}

public static class MimeTypeLookup
{
    private static readonly Dictionary<string, string> _mappings = new Dictionary<string, string>(2000, StringComparer.InvariantCultureIgnoreCase)
        {
            {".ez", "application/andrew-inset"},
            {".aw", "application/applixware"},
            {".atom", "application/atom+xml"},
            {".atomcat", "application/atomcat+xml"},
            {".atomsvc", "application/atomsvc+xml"},
            {".ccxml", "application/ccxml+xml"},
            {".cdmia", "application/cdmi-capability"},
            {".cdmic", "application/cdmi-container"},
            {".cdmid", "application/cdmi-domain"},
            {".cdmio", "application/cdmi-object"},
            {".cdmiq", "application/cdmi-queue"},
            {".cu", "application/cu-seeme"},
            {".davmount", "application/davmount+xml"},
            {".dbk", "application/docbook+xml"},
            {".dssc", "application/dssc+der"},
            {".xdssc", "application/dssc+xml"},
            {".ecma", "application/ecmascript"},
            {".emma", "application/emma+xml"},
            {".epub", "application/epub+zip"},
            {".exi", "application/exi"},
            {".pfr", "application/font-tdpfr"},
            {".gml", "application/gml+xml"},
            {".gpx", "application/gpx+xml"},
            {".gxf", "application/gxf"},
            {".stk", "application/hyperstudio"},
            {".ink", "application/inkml+xml"},
            {".inkml", "application/inkml+xml"},
            {".ipfix", "application/ipfix"},
            {".jar", "application/java-archive"},
            {".ser", "application/java-serialized-object"},
            {".class", "application/java-vm"},
            {".js", "application/javascript"},
            {".json", "application/json"},
            {".jsonml", "application/jsonml+json"},
            {".lostxml", "application/lost+xml"},
            {".hqx", "application/mac-binhex40"},
            {".cpt", "application/mac-compactpro"},
            {".mads", "application/mads+xml"},
            {".mrc", "application/marc"},
            {".mrcx", "application/marcxml+xml"},
            {".ma", "application/mathematica"},
            {".nb", "application/mathematica"},
            {".mb", "application/mathematica"},
            {".mathml", "application/mathml+xml"},
            {".mbox", "application/mbox"},
            {".mscml", "application/mediaservercontrol+xml"},
            {".metalink", "application/metalink+xml"},
            {".meta4", "application/metalink4+xml"},
            {".mets", "application/mets+xml"},
            {".mods", "application/mods+xml"},
            {".m21", "application/mp21"},
            {".mp21", "application/mp21"},
            {".mp4s", "application/mp4"},
            {".doc", "application/msword"},
            {".dot", "application/msword"},
            {".mxf", "application/mxf"},
            {".bin", "application/octet-stream"},
            {".dms", "application/octet-stream"},
            {".lrf", "application/octet-stream"},
            {".mar", "application/octet-stream"},
            {".so", "application/octet-stream"},
            {".dist", "application/octet-stream"},
            {".distz", "application/octet-stream"},
            {".pkg", "application/octet-stream"},
            {".bpk", "application/octet-stream"},
            {".dump", "application/octet-stream"},
            {".elc", "application/octet-stream"},
            {".deploy", "application/octet-stream"},
            {".oda", "application/oda"},
            {".opf", "application/oebps-package+xml"},
            {".ogx", "application/ogg"},
            {".omdoc", "application/omdoc+xml"},
            {".onetoc", "application/onenote"},
            {".onetoc2", "application/onenote"},
            {".onetmp", "application/onenote"},
            {".onepkg", "application/onenote"},
            {".oxps", "application/oxps"},
            {".xer", "application/patch-ops-error+xml"},
            {".pdf", "application/pdf"},
            {".pgp", "application/pgp-encrypted"},
            {".asc", "application/pgp-signature"},
            {".sig", "application/pgp-signature"},
            {".prf", "application/pics-rules"},
            {".p10", "application/pkcs10"},
            {".p7m", "application/pkcs7-mime"},
            {".p7c", "application/pkcs7-mime"},
            {".p7s", "application/pkcs7-signature"},
            {".p8", "application/pkcs8"},
            {".ac", "application/pkix-attr-cert"},
            {".cer", "application/pkix-cert"},
            {".crl", "application/pkix-crl"},
            {".pkipath", "application/pkix-pkipath"},
            {".pki", "application/pkixcmp"},
            {".pls", "application/pls+xml"},
            {".ai", "application/postscript"},
            {".eps", "application/postscript"},
            {".ps", "application/postscript"},
            {".cww", "application/prs.cww"},
            {".pskcxml", "application/pskc+xml"},
            {".rdf", "application/rdf+xml"},
            {".rif", "application/reginfo+xml"},
            {".rnc", "application/relax-ng-compact-syntax"},
            {".rl", "application/resource-lists+xml"},
            {".rld", "application/resource-lists-diff+xml"},
            {".rs", "application/rls-services+xml"},
            {".gbr", "application/rpki-ghostbusters"},
            {".mft", "application/rpki-manifest"},
            {".roa", "application/rpki-roa"},
            {".rsd", "application/rsd+xml"},
            {".rss", "application/rss+xml"},
            {".rtf", "application/rtf"},
            {".sbml", "application/sbml+xml"},
            {".scq", "application/scvp-cv-request"},
            {".scs", "application/scvp-cv-response"},
            {".spq", "application/scvp-vp-request"},
            {".spp", "application/scvp-vp-response"},
            {".sdp", "application/sdp"},
            {".setpay", "application/set-payment-initiation"},
            {".setreg", "application/set-registration-initiation"},
            {".shf", "application/shf+xml"},
            {".smi", "application/smil+xml"},
            {".smil", "application/smil+xml"},
            {".rq", "application/sparql-query"},
            {".srx", "application/sparql-results+xml"},
            {".gram", "application/srgs"},
            {".grxml", "application/srgs+xml"},
            {".sru", "application/sru+xml"},
            {".ssdl", "application/ssdl+xml"},
            {".ssml", "application/ssml+xml"},
            {".tei", "application/tei+xml"},
            {".teicorpus", "application/tei+xml"},
            {".tfi", "application/thraud+xml"},
            {".tsd", "application/timestamped-data"},
            {".plb", "application/vnd.3gpp.pic-bw-large"},
            {".psb", "application/vnd.3gpp.pic-bw-small"},
            {".pvb", "application/vnd.3gpp.pic-bw-var"},
            {".tcap", "application/vnd.3gpp2.tcap"},
            {".pwn", "application/vnd.3m.post-it-notes"},
            {".aso", "application/vnd.accpac.simply.aso"},
            {".imp", "application/vnd.accpac.simply.imp"},
            {".acu", "application/vnd.acucobol"},
            {".atc", "application/vnd.acucorp"},
            {".acutc", "application/vnd.acucorp"},
            {".air", "application/vnd.adobe.air-application-installer-package+zip"},
            {".fcdt", "application/vnd.adobe.formscentral.fcdt"},
            {".fxp", "application/vnd.adobe.fxp"},
            {".fxpl", "application/vnd.adobe.fxp"},
            {".xdp", "application/vnd.adobe.xdp+xml"},
            {".xfdf", "application/vnd.adobe.xfdf"},
            {".ahead", "application/vnd.ahead.space"},
            {".azf", "application/vnd.airzip.filesecure.azf"},
            {".azs", "application/vnd.airzip.filesecure.azs"},
            {".azw", "application/vnd.amazon.ebook"},
            {".acc", "application/vnd.americandynamics.acc"},
            {".ami", "application/vnd.amiga.ami"},
            {".apk", "application/vnd.android.package-archive"},
            {".cii", "application/vnd.anser-web-certificate-issue-initiation"},
            {".fti", "application/vnd.anser-web-funds-transfer-initiation"},
            {".atx", "application/vnd.antix.game-component"},
            {".mpkg", "application/vnd.apple.installer+xml"},
            {".m3u8", "application/vnd.apple.mpegurl"},
            {".swi", "application/vnd.aristanetworks.swi"},
            {".iota", "application/vnd.astraea-software.iota"},
            {".aep", "application/vnd.audiograph"},
            {".mpm", "application/vnd.blueice.multipass"},
            {".bmi", "application/vnd.bmi"},
            {".rep", "application/vnd.businessobjects"},
            {".cdxml", "application/vnd.chemdraw+xml"},
            {".mmd", "application/vnd.chipnuts.karaoke-mmd"},
            {".cdy", "application/vnd.cinderella"},
            {".cla", "application/vnd.claymore"},
            {".rp9", "application/vnd.cloanto.rp9"},
            {".c4g", "application/vnd.clonk.c4group"},
            {".c4d", "application/vnd.clonk.c4group"},
            {".c4f", "application/vnd.clonk.c4group"},
            {".c4p", "application/vnd.clonk.c4group"},
            {".c4u", "application/vnd.clonk.c4group"},
            {".c11amc", "application/vnd.cluetrust.cartomobile-config"},
            {".c11amz", "application/vnd.cluetrust.cartomobile-config-pkg"},
            {".csp", "application/vnd.commonspace"},
            {".cdbcmsg", "application/vnd.contact.cmsg"},
            {".cmc", "application/vnd.cosmocaller"},
            {".clkx", "application/vnd.crick.clicker"},
            {".clkk", "application/vnd.crick.clicker.keyboard"},
            {".clkp", "application/vnd.crick.clicker.palette"},
            {".clkt", "application/vnd.crick.clicker.template"},
            {".clkw", "application/vnd.crick.clicker.wordbank"},
            {".wbs", "application/vnd.criticaltools.wbs+xml"},
            {".pml", "application/vnd.ctc-posml"},
            {".ppd", "application/vnd.cups-ppd"},
            {".car", "application/vnd.curl.car"},
            {".pcurl", "application/vnd.curl.pcurl"},
            {".dart", "application/vnd.dart"},
            {".rdz", "application/vnd.data-vision.rdz"},
            {".uvf", "application/vnd.dece.data"},
            {".uvvf", "application/vnd.dece.data"},
            {".uvd", "application/vnd.dece.data"},
            {".uvvd", "application/vnd.dece.data"},
            {".uvt", "application/vnd.dece.ttml+xml"},
            {".uvvt", "application/vnd.dece.ttml+xml"},
            {".uvx", "application/vnd.dece.unspecified"},
            {".uvvx", "application/vnd.dece.unspecified"},
            {".uvz", "application/vnd.dece.zip"},
            {".uvvz", "application/vnd.dece.zip"},
            {".fe_launch", "application/vnd.denovo.fcselayout-link"},
            {".dna", "application/vnd.dna"},
            {".mlp", "application/vnd.dolby.mlp"},
            {".dpg", "application/vnd.dpgraph"},
            {".dfac", "application/vnd.dreamfactory"},
            {".kpxx", "application/vnd.ds-keypoint"},
            {".ait", "application/vnd.dvb.ait"},
            {".svc", "application/vnd.dvb.service"},
            {".geo", "application/vnd.dynageo"},
            {".mag", "application/vnd.ecowin.chart"},
            {".nml", "application/vnd.enliven"},
            {".esf", "application/vnd.epson.esf"},
            {".msf", "application/vnd.epson.msf"},
            {".qam", "application/vnd.epson.quickanime"},
            {".slt", "application/vnd.epson.salt"},
            {".ssf", "application/vnd.epson.ssf"},
            {".es3", "application/vnd.eszigno3+xml"},
            {".et3", "application/vnd.eszigno3+xml"},
            {".ez2", "application/vnd.ezpix-album"},
            {".ez3", "application/vnd.ezpix-package"},
            {".fdf", "application/vnd.fdf"},
            {".mseed", "application/vnd.fdsn.mseed"},
            {".seed", "application/vnd.fdsn.seed"},
            {".dataless", "application/vnd.fdsn.seed"},
            {".gph", "application/vnd.flographit"},
            {".ftc", "application/vnd.fluxtime.clip"},
            {".fm", "application/vnd.framemaker"},
            {".frame", "application/vnd.framemaker"},
            {".maker", "application/vnd.framemaker"},
            {".book", "application/vnd.framemaker"},
            {".fnc", "application/vnd.frogans.fnc"},
            {".ltf", "application/vnd.frogans.ltf"},
            {".fsc", "application/vnd.fsc.weblaunch"},
            {".oas", "application/vnd.fujitsu.oasys"},
            {".oa2", "application/vnd.fujitsu.oasys2"},
            {".oa3", "application/vnd.fujitsu.oasys3"},
            {".fg5", "application/vnd.fujitsu.oasysgp"},
            {".bh2", "application/vnd.fujitsu.oasysprs"},
            {".ddd", "application/vnd.fujixerox.ddd"},
            {".xdw", "application/vnd.fujixerox.docuworks"},
            {".xbd", "application/vnd.fujixerox.docuworks.binder"},
            {".fzs", "application/vnd.fuzzysheet"},
            {".txd", "application/vnd.genomatix.tuxedo"},
            {".ggb", "application/vnd.geogebra.file"},
            {".ggt", "application/vnd.geogebra.tool"},
            {".gex", "application/vnd.geometry-explorer"},
            {".gre", "application/vnd.geometry-explorer"},
            {".gxt", "application/vnd.geonext"},
            {".g2w", "application/vnd.geoplan"},
            {".g3w", "application/vnd.geospace"},
            {".gmx", "application/vnd.gmx"},
            {".kml", "application/vnd.google-earth.kml+xml"},
            {".kmz", "application/vnd.google-earth.kmz"},
            {".gqf", "application/vnd.grafeq"},
            {".gqs", "application/vnd.grafeq"},
            {".gac", "application/vnd.groove-account"},
            {".ghf", "application/vnd.groove-help"},
            {".gim", "application/vnd.groove-identity-message"},
            {".grv", "application/vnd.groove-injector"},
            {".gtm", "application/vnd.groove-tool-message"},
            {".tpl", "application/vnd.groove-tool-template"},
            {".vcg", "application/vnd.groove-vcard"},
            {".hal", "application/vnd.hal+xml"},
            {".zmm", "application/vnd.handheld-entertainment+xml"},
            {".hbci", "application/vnd.hbci"},
            {".les", "application/vnd.hhe.lesson-player"},
            {".hpgl", "application/vnd.hp-hpgl"},
            {".hpid", "application/vnd.hp-hpid"},
            {".hps", "application/vnd.hp-hps"},
            {".jlt", "application/vnd.hp-jlyt"},
            {".pcl", "application/vnd.hp-pcl"},
            {".pclxl", "application/vnd.hp-pclxl"},
            {".sfd-hdstx", "application/vnd.hydrostatix.sof-data"},
            {".mpy", "application/vnd.ibm.minipay"},
            {".afp", "application/vnd.ibm.modcap"},
            {".listafp", "application/vnd.ibm.modcap"},
            {".list3820", "application/vnd.ibm.modcap"},
            {".irm", "application/vnd.ibm.rights-management"},
            {".sc", "application/vnd.ibm.secure-container"},
            {".icc", "application/vnd.iccprofile"},
            {".icm", "application/vnd.iccprofile"},
            {".igl", "application/vnd.igloader"},
            {".ivp", "application/vnd.immervision-ivp"},
            {".ivu", "application/vnd.immervision-ivu"},
            {".igm", "application/vnd.insors.igm"},
            {".xpw", "application/vnd.intercon.formnet"},
            {".xpx", "application/vnd.intercon.formnet"},
            {".i2g", "application/vnd.intergeo"},
            {".qbo", "application/vnd.intu.qbo"},
            {".qfx", "application/vnd.intu.qfx"},
            {".rcprofile", "application/vnd.ipunplugged.rcprofile"},
            {".irp", "application/vnd.irepository.package+xml"},
            {".xpr", "application/vnd.is-xpr"},
            {".fcs", "application/vnd.isac.fcs"},
            {".jam", "application/vnd.jam"},
            {".rms", "application/vnd.jcp.javame.midlet-rms"},
            {".jisp", "application/vnd.jisp"},
            {".joda", "application/vnd.joost.joda-archive"},
            {".ktz", "application/vnd.kahootz"},
            {".ktr", "application/vnd.kahootz"},
            {".karbon", "application/vnd.kde.karbon"},
            {".chrt", "application/vnd.kde.kchart"},
            {".kfo", "application/vnd.kde.kformula"},
            {".flw", "application/vnd.kde.kivio"},
            {".kon", "application/vnd.kde.kontour"},
            {".kpr", "application/vnd.kde.kpresenter"},
            {".kpt", "application/vnd.kde.kpresenter"},
            {".ksp", "application/vnd.kde.kspread"},
            {".kwd", "application/vnd.kde.kword"},
            {".kwt", "application/vnd.kde.kword"},
            {".htke", "application/vnd.kenameaapp"},
            {".kia", "application/vnd.kidspiration"},
            {".kne", "application/vnd.kinar"},
            {".knp", "application/vnd.kinar"},
            {".skp", "application/vnd.koan"},
            {".skd", "application/vnd.koan"},
            {".skt", "application/vnd.koan"},
            {".skm", "application/vnd.koan"},
            {".sse", "application/vnd.kodak-descriptor"},
            {".lasxml", "application/vnd.las.las+xml"},
            {".lbd", "application/vnd.llamagraphics.life-balance.desktop"},
            {".lbe", "application/vnd.llamagraphics.life-balance.exchange+xml"},
            {".123", "application/vnd.lotus-1-2-3"},
            {".apr", "application/vnd.lotus-approach"},
            {".pre", "application/vnd.lotus-freelance"},
            {".nsf", "application/vnd.lotus-notes"},
            {".org", "application/vnd.lotus-organizer"},
            {".scm", "application/vnd.lotus-screencam"},
            {".lwp", "application/vnd.lotus-wordpro"},
            {".portpkg", "application/vnd.macports.portpkg"},
            {".mcd", "application/vnd.mcd"},
            {".mc1", "application/vnd.medcalcdata"},
            {".cdkey", "application/vnd.mediastation.cdkey"},
            {".mwf", "application/vnd.mfer"},
            {".mfm", "application/vnd.mfmp"},
            {".flo", "application/vnd.micrografx.flo"},
            {".igx", "application/vnd.micrografx.igx"},
            {".mif", "application/vnd.mif"},
            {".daf", "application/vnd.mobius.daf"},
            {".dis", "application/vnd.mobius.dis"},
            {".mbk", "application/vnd.mobius.mbk"},
            {".mqy", "application/vnd.mobius.mqy"},
            {".msl", "application/vnd.mobius.msl"},
            {".plc", "application/vnd.mobius.plc"},
            {".txf", "application/vnd.mobius.txf"},
            {".mpn", "application/vnd.mophun.application"},
            {".mpc", "application/vnd.mophun.certificate"},
            {".xul", "application/vnd.mozilla.xul+xml"},
            {".cil", "application/vnd.ms-artgalry"},
            {".cab", "application/vnd.ms-cab-compressed"},
            {".xls", "application/vnd.ms-excel"},
            {".xlm", "application/vnd.ms-excel"},
            {".xla", "application/vnd.ms-excel"},
            {".xlc", "application/vnd.ms-excel"},
            {".xlt", "application/vnd.ms-excel"},
            {".xlw", "application/vnd.ms-excel"},
            {".xlam", "application/vnd.ms-excel.addin.macroenabled.12"},
            {".xlsb", "application/vnd.ms-excel.sheet.binary.macroenabled.12"},
            {".xlsm", "application/vnd.ms-excel.sheet.macroenabled.12"},
            {".xltm", "application/vnd.ms-excel.template.macroenabled.12"},
            {".eot", "application/vnd.ms-fontobject"},
            {".chm", "application/vnd.ms-htmlhelp"},
            {".ims", "application/vnd.ms-ims"},
            {".lrm", "application/vnd.ms-lrm"},
            {".thmx", "application/vnd.ms-officetheme"},
            {".cat", "application/vnd.ms-pki.seccat"},
            {".stl", "application/vnd.ms-pki.stl"},
            {".ppt", "application/vnd.ms-powerpoint"},
            {".pps", "application/vnd.ms-powerpoint"},
            {".pot", "application/vnd.ms-powerpoint"},
            {".ppam", "application/vnd.ms-powerpoint.addin.macroenabled.12"},
            {".pptm", "application/vnd.ms-powerpoint.presentation.macroenabled.12"},
            {".sldm", "application/vnd.ms-powerpoint.slide.macroenabled.12"},
            {".ppsm", "application/vnd.ms-powerpoint.slideshow.macroenabled.12"},
            {".potm", "application/vnd.ms-powerpoint.template.macroenabled.12"},
            {".mpp", "application/vnd.ms-project"},
            {".mpt", "application/vnd.ms-project"},
            {".docm", "application/vnd.ms-word.document.macroenabled.12"},
            {".dotm", "application/vnd.ms-word.template.macroenabled.12"},
            {".wps", "application/vnd.ms-works"},
            {".wks", "application/vnd.ms-works"},
            {".wcm", "application/vnd.ms-works"},
            {".wdb", "application/vnd.ms-works"},
            {".wpl", "application/vnd.ms-wpl"},
            {".xps", "application/vnd.ms-xpsdocument"},
            {".mseq", "application/vnd.mseq"},
            {".mus", "application/vnd.musician"},
            {".msty", "application/vnd.muvee.style"},
            {".taglet", "application/vnd.mynfc"},
            {".nlu", "application/vnd.neurolanguage.nlu"},
            {".ntf", "application/vnd.nitf"},
            {".nitf", "application/vnd.nitf"},
            {".nnd", "application/vnd.noblenet-directory"},
            {".nns", "application/vnd.noblenet-sealer"},
            {".nnw", "application/vnd.noblenet-web"},
            {".ngdat", "application/vnd.nokia.n-gage.data"},
            {".n-gage", "application/vnd.nokia.n-gage.symbian.install"},
            {".rpst", "application/vnd.nokia.radio-preset"},
            {".rpss", "application/vnd.nokia.radio-presets"},
            {".edm", "application/vnd.novadigm.edm"},
            {".edx", "application/vnd.novadigm.edx"},
            {".ext", "application/vnd.novadigm.ext"},
            {".odc", "application/vnd.oasis.opendocument.chart"},
            {".otc", "application/vnd.oasis.opendocument.chart-template"},
            {".odb", "application/vnd.oasis.opendocument.database"},
            {".odf", "application/vnd.oasis.opendocument.formula"},
            {".odft", "application/vnd.oasis.opendocument.formula-template"},
            {".odg", "application/vnd.oasis.opendocument.graphics"},
            {".otg", "application/vnd.oasis.opendocument.graphics-template"},
            {".odi", "application/vnd.oasis.opendocument.image"},
            {".oti", "application/vnd.oasis.opendocument.image-template"},
            {".odp", "application/vnd.oasis.opendocument.presentation"},
            {".otp", "application/vnd.oasis.opendocument.presentation-template"},
            {".ods", "application/vnd.oasis.opendocument.spreadsheet"},
            {".ots", "application/vnd.oasis.opendocument.spreadsheet-template"},
            {".odt", "application/vnd.oasis.opendocument.text"},
            {".odm", "application/vnd.oasis.opendocument.text-master"},
            {".ott", "application/vnd.oasis.opendocument.text-template"},
            {".oth", "application/vnd.oasis.opendocument.text-web"},
            {".xo", "application/vnd.olpc-sugar"},
            {".dd2", "application/vnd.oma.dd2+xml"},
            {".oxt", "application/vnd.openofficeorg.extension"},
            {".pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation"},
            {".sldx", "application/vnd.openxmlformats-officedocument.presentationml.slide"},
            {".ppsx", "application/vnd.openxmlformats-officedocument.presentationml.slideshow"},
            {".potx", "application/vnd.openxmlformats-officedocument.presentationml.template"},
            {".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
            {".xltx", "application/vnd.openxmlformats-officedocument.spreadsheetml.template"},
            {".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document"},
            {".dotx", "application/vnd.openxmlformats-officedocument.wordprocessingml.template"},
            {".mgp", "application/vnd.osgeo.mapguide.package"},
            {".dp", "application/vnd.osgi.dp"},
            {".esa", "application/vnd.osgi.subsystem"},
            {".pdb", "application/vnd.palm"},
            {".pqa", "application/vnd.palm"},
            {".oprc", "application/vnd.palm"},
            {".paw", "application/vnd.pawaafile"},
            {".str", "application/vnd.pg.format"},
            {".ei6", "application/vnd.pg.osasli"},
            {".efif", "application/vnd.picsel"},
            {".wg", "application/vnd.pmi.widget"},
            {".plf", "application/vnd.pocketlearn"},
            {".pbd", "application/vnd.powerbuilder6"},
            {".box", "application/vnd.previewsystems.box"},
            {".mgz", "application/vnd.proteus.magazine"},
            {".qps", "application/vnd.publishare-delta-tree"},
            {".ptid", "application/vnd.pvi.ptid1"},
            {".qxd", "application/vnd.quark.quarkxpress"},
            {".qxt", "application/vnd.quark.quarkxpress"},
            {".qwd", "application/vnd.quark.quarkxpress"},
            {".qwt", "application/vnd.quark.quarkxpress"},
            {".qxl", "application/vnd.quark.quarkxpress"},
            {".qxb", "application/vnd.quark.quarkxpress"},
            {".bed", "application/vnd.realvnc.bed"},
            {".mxl", "application/vnd.recordare.musicxml"},
            {".musicxml", "application/vnd.recordare.musicxml+xml"},
            {".cryptonote", "application/vnd.rig.cryptonote"},
            {".cod", "application/vnd.rim.cod"},
            {".rm", "application/vnd.rn-realmedia"},
            {".rmvb", "application/vnd.rn-realmedia-vbr"},
            {".link66", "application/vnd.route66.link66+xml"},
            {".st", "application/vnd.sailingtracker.track"},
            {".see", "application/vnd.seemail"},
            {".sema", "application/vnd.sema"},
            {".semd", "application/vnd.semd"},
            {".semf", "application/vnd.semf"},
            {".ifm", "application/vnd.shana.informed.formdata"},
            {".itp", "application/vnd.shana.informed.formtemplate"},
            {".iif", "application/vnd.shana.informed.interchange"},
            {".ipk", "application/vnd.shana.informed.package"},
            {".twd", "application/vnd.simtech-mindmapper"},
            {".twds", "application/vnd.simtech-mindmapper"},
            {".mmf", "application/vnd.smaf"},
            {".teacher", "application/vnd.smart.teacher"},
            {".sdkm", "application/vnd.solent.sdkm+xml"},
            {".sdkd", "application/vnd.solent.sdkm+xml"},
            {".dxp", "application/vnd.spotfire.dxp"},
            {".sfs", "application/vnd.spotfire.sfs"},
            {".sdc", "application/vnd.stardivision.calc"},
            {".sda", "application/vnd.stardivision.draw"},
            {".sdd", "application/vnd.stardivision.impress"},
            {".smf", "application/vnd.stardivision.math"},
            {".sdw", "application/vnd.stardivision.writer"},
            {".vor", "application/vnd.stardivision.writer"},
            {".sgl", "application/vnd.stardivision.writer-global"},
            {".smzip", "application/vnd.stepmania.package"},
            {".sm", "application/vnd.stepmania.stepchart"},
            {".sxc", "application/vnd.sun.xml.calc"},
            {".stc", "application/vnd.sun.xml.calc.template"},
            {".sxd", "application/vnd.sun.xml.draw"},
            {".std", "application/vnd.sun.xml.draw.template"},
            {".sxi", "application/vnd.sun.xml.impress"},
            {".sti", "application/vnd.sun.xml.impress.template"},
            {".sxm", "application/vnd.sun.xml.math"},
            {".sxw", "application/vnd.sun.xml.writer"},
            {".sxg", "application/vnd.sun.xml.writer.global"},
            {".stw", "application/vnd.sun.xml.writer.template"},
            {".sus", "application/vnd.sus-calendar"},
            {".susp", "application/vnd.sus-calendar"},
            {".svd", "application/vnd.svd"},
            {".sis", "application/vnd.symbian.install"},
            {".sisx", "application/vnd.symbian.install"},
            {".xsm", "application/vnd.syncml+xml"},
            {".bdm", "application/vnd.syncml.dm+wbxml"},
            {".xdm", "application/vnd.syncml.dm+xml"},
            {".tao", "application/vnd.tao.intent-module-archive"},
            {".pcap", "application/vnd.tcpdump.pcap"},
            {".cap", "application/vnd.tcpdump.pcap"},
            {".dmp", "application/vnd.tcpdump.pcap"},
            {".tmo", "application/vnd.tmobile-livetv"},
            {".tpt", "application/vnd.trid.tpt"},
            {".mxs", "application/vnd.triscape.mxs"},
            {".tra", "application/vnd.trueapp"},
            {".ufd", "application/vnd.ufdl"},
            {".ufdl", "application/vnd.ufdl"},
            {".utz", "application/vnd.uiq.theme"},
            {".umj", "application/vnd.umajin"},
            {".unityweb", "application/vnd.unity"},
            {".uoml", "application/vnd.uoml+xml"},
            {".vcx", "application/vnd.vcx"},
            {".vsd", "application/vnd.visio"},
            {".vst", "application/vnd.visio"},
            {".vss", "application/vnd.visio"},
            {".vsw", "application/vnd.visio"},
            {".vis", "application/vnd.visionary"},
            {".vsf", "application/vnd.vsf"},
            {".wbxml", "application/vnd.wap.wbxml"},
            {".wmlc", "application/vnd.wap.wmlc"},
            {".wmlsc", "application/vnd.wap.wmlscriptc"},
            {".wtb", "application/vnd.webturbo"},
            {".nbp", "application/vnd.wolfram.player"},
            {".wpd", "application/vnd.wordperfect"},
            {".wqd", "application/vnd.wqd"},
            {".stf", "application/vnd.wt.stf"},
            {".xar", "application/vnd.xara"},
            {".xfdl", "application/vnd.xfdl"},
            {".hvd", "application/vnd.yamaha.hv-dic"},
            {".hvs", "application/vnd.yamaha.hv-script"},
            {".hvp", "application/vnd.yamaha.hv-voice"},
            {".osf", "application/vnd.yamaha.openscoreformat"},
            {".osfpvg", "application/vnd.yamaha.openscoreformat.osfpvg+xml"},
            {".saf", "application/vnd.yamaha.smaf-audio"},
            {".spf", "application/vnd.yamaha.smaf-phrase"},
            {".cmp", "application/vnd.yellowriver-custom-menu"},
            {".zir", "application/vnd.zul"},
            {".zirz", "application/vnd.zul"},
            {".zaz", "application/vnd.zzazz.deck+xml"},
            {".vxml", "application/voicexml+xml"},
            {".wgt", "application/widget"},
            {".hlp", "application/winhlp"},
            {".wsdl", "application/wsdl+xml"},
            {".wspolicy", "application/wspolicy+xml"},
            {".7z", "application/x-7z-compressed"},
            {".abw", "application/x-abiword"},
            {".ace", "application/x-ace-compressed"},
            {".dmg", "application/x-apple-diskimage"},
            {".aab", "application/x-authorware-bin"},
            {".x32", "application/x-authorware-bin"},
            {".u32", "application/x-authorware-bin"},
            {".vox", "application/x-authorware-bin"},
            {".aam", "application/x-authorware-map"},
            {".aas", "application/x-authorware-seg"},
            {".bcpio", "application/x-bcpio"},
            {".torrent", "application/x-bittorrent"},
            {".blb", "application/x-blorb"},
            {".blorb", "application/x-blorb"},
            {".bz", "application/x-bzip"},
            {".bz2", "application/x-bzip2"},
            {".boz", "application/x-bzip2"},
            {".cbr", "application/x-cbr"},
            {".cba", "application/x-cbr"},
            {".cbt", "application/x-cbr"},
            {".cbz", "application/x-cbr"},
            {".cb7", "application/x-cbr"},
            {".vcd", "application/x-cdlink"},
            {".cfs", "application/x-cfs-compressed"},
            {".chat", "application/x-chat"},
            {".pgn", "application/x-chess-pgn"},
            {".nsc", "application/x-conference"},
            {".cpio", "application/x-cpio"},
            {".csh", "application/x-csh"},
            {".deb", "application/x-debian-package"},
            {".udeb", "application/x-debian-package"},
            {".dgc", "application/x-dgc-compressed"},
            {".dir", "application/x-director"},
            {".dcr", "application/x-director"},
            {".dxr", "application/x-director"},
            {".cst", "application/x-director"},
            {".cct", "application/x-director"},
            {".cxt", "application/x-director"},
            {".w3d", "application/x-director"},
            {".fgd", "application/x-director"},
            {".swa", "application/x-director"},
            {".wad", "application/x-doom"},
            {".ncx", "application/x-dtbncx+xml"},
            {".dtb", "application/x-dtbook+xml"},
            {".res", "application/x-dtbresource+xml"},
            {".dvi", "application/x-dvi"},
            {".evy", "application/x-envoy"},
            {".eva", "application/x-eva"},
            {".bdf", "application/x-font-bdf"},
            {".gsf", "application/x-font-ghostscript"},
            {".psf", "application/x-font-linux-psf"},
            {".otf", "application/x-font-otf"},
            {".pcf", "application/x-font-pcf"},
            {".snf", "application/x-font-snf"},
            {".ttf", "application/x-font-ttf"},
            {".ttc", "application/x-font-ttf"},
            {".pfa", "application/x-font-type1"},
            {".pfb", "application/x-font-type1"},
            {".pfm", "application/x-font-type1"},
            {".afm", "application/x-font-type1"},
            {".woff", "application/font-woff"},
            {".arc", "application/x-freearc"},
            {".spl", "application/x-futuresplash"},
            {".gca", "application/x-gca-compressed"},
            {".ulx", "application/x-glulx"},
            {".gnumeric", "application/x-gnumeric"},
            {".gramps", "application/x-gramps-xml"},
            {".gtar", "application/x-gtar"},
            {".hdf", "application/x-hdf"},
            {".install", "application/x-install-instructions"},
            {".iso", "application/x-iso9660-image"},
            {".jnlp", "application/x-java-jnlp-file"},
            {".latex", "application/x-latex"},
            {".lzh", "application/x-lzh-compressed"},
            {".lha", "application/x-lzh-compressed"},
            {".mie", "application/x-mie"},
            {".prc", "application/x-mobipocket-ebook"},
            {".mobi", "application/x-mobipocket-ebook"},
            {".application", "application/x-ms-application"},
            {".lnk", "application/x-ms-shortcut"},
            {".wmd", "application/x-ms-wmd"},
            {".wmz", "application/x-ms-wmz"},
            {".xbap", "application/x-ms-xbap"},
            {".mdb", "application/x-msaccess"},
            {".obd", "application/x-msbinder"},
            {".crd", "application/x-mscardfile"},
            {".clp", "application/x-msclip"},
            {".exe", "application/x-msdownload"},
            {".dll", "application/x-msdownload"},
            {".com", "application/x-msdownload"},
            {".bat", "application/x-msdownload"},
            {".msi", "application/x-msdownload"},
            {".mvb", "application/x-msmediaview"},
            {".m13", "application/x-msmediaview"},
            {".m14", "application/x-msmediaview"},
            {".wmf", "application/x-msmetafile"},
            {".emf", "application/x-msmetafile"},
            {".emz", "application/x-msmetafile"},
            {".mny", "application/x-msmoney"},
            {".pub", "application/x-mspublisher"},
            {".scd", "application/x-msschedule"},
            {".trm", "application/x-msterminal"},
            {".wri", "application/x-mswrite"},
            {".nc", "application/x-netcdf"},
            {".cdf", "application/x-netcdf"},
            {".nzb", "application/x-nzb"},
            {".p12", "application/x-pkcs12"},
            {".pfx", "application/x-pkcs12"},
            {".p7b", "application/x-pkcs7-certificates"},
            {".spc", "application/x-pkcs7-certificates"},
            {".p7r", "application/x-pkcs7-certreqresp"},
            {".rar", "application/x-rar-compressed"},
            {".ris", "application/x-research-info-systems"},
            {".sh", "application/x-sh"},
            {".shar", "application/x-shar"},
            {".swf", "application/x-shockwave-flash"},
            {".xap", "application/x-silverlight-app"},
            {".sql", "application/x-sql"},
            {".sit", "application/x-stuffit"},
            {".sitx", "application/x-stuffitx"},
            {".srt", "application/x-subrip"},
            {".sv4cpio", "application/x-sv4cpio"},
            {".sv4crc", "application/x-sv4crc"},
            {".t3", "application/x-t3vm-image"},
            {".gam", "application/x-tads"},
            {".tar", "application/x-tar"},
            {".tcl", "application/x-tcl"},
            {".tex", "application/x-tex"},
            {".tfm", "application/x-tex-tfm"},
            {".texinfo", "application/x-texinfo"},
            {".texi", "application/x-texinfo"},
            {".obj", "application/x-tgif"},
            {".ustar", "application/x-ustar"},
            {".src", "application/x-wais-source"},
            {".der", "application/x-x509-ca-cert"},
            {".crt", "application/x-x509-ca-cert"},
            {".fig", "application/x-xfig"},
            {".xlf", "application/x-xliff+xml"},
            {".xpi", "application/x-xpinstall"},
            {".xz", "application/x-xz"},
            {".z1", "application/x-zmachine"},
            {".z2", "application/x-zmachine"},
            {".z3", "application/x-zmachine"},
            {".z4", "application/x-zmachine"},
            {".z5", "application/x-zmachine"},
            {".z6", "application/x-zmachine"},
            {".z7", "application/x-zmachine"},
            {".z8", "application/x-zmachine"},
            {".xaml", "application/xaml+xml"},
            {".xdf", "application/xcap-diff+xml"},
            {".xenc", "application/xenc+xml"},
            {".xhtml", "application/xhtml+xml"},
            {".xht", "application/xhtml+xml"},
            {".xml", "application/xml"},
            {".xsl", "application/xml"},
            {".dtd", "application/xml-dtd"},
            {".xop", "application/xop+xml"},
            {".xpl", "application/xproc+xml"},
            {".xslt", "application/xslt+xml"},
            {".xspf", "application/xspf+xml"},
            {".mxml", "application/xv+xml"},
            {".xhvml", "application/xv+xml"},
            {".xvml", "application/xv+xml"},
            {".xvm", "application/xv+xml"},
            {".yang", "application/yang"},
            {".yin", "application/yin+xml"},
            {".zip", "application/zip"},
            {".adp", "audio/adpcm"},
            {".au", "audio/basic"},
            {".snd", "audio/basic"},
            {".mid", "audio/midi"},
            {".midi", "audio/midi"},
            {".kar", "audio/midi"},
            {".rmi", "audio/midi"},
            {".mp4a", "audio/mp4"},
            {".mpga", "audio/mpeg"},
            {".mp2", "audio/mpeg"},
            {".mp2a", "audio/mpeg"},
            {".mp3", "audio/mpeg"},
            {".m2a", "audio/mpeg"},
            {".m3a", "audio/mpeg"},
            {".oga", "audio/ogg"},
            {".ogg", "audio/ogg"},
            {".spx", "audio/ogg"},
            {".s3m", "audio/s3m"},
            {".sil", "audio/silk"},
            {".uva", "audio/vnd.dece.audio"},
            {".uvva", "audio/vnd.dece.audio"},
            {".eol", "audio/vnd.digital-winds"},
            {".dra", "audio/vnd.dra"},
            {".dts", "audio/vnd.dts"},
            {".dtshd", "audio/vnd.dts.hd"},
            {".lvp", "audio/vnd.lucent.voice"},
            {".pya", "audio/vnd.ms-playready.media.pya"},
            {".ecelp4800", "audio/vnd.nuera.ecelp4800"},
            {".ecelp7470", "audio/vnd.nuera.ecelp7470"},
            {".ecelp9600", "audio/vnd.nuera.ecelp9600"},
            {".rip", "audio/vnd.rip"},
            {".weba", "audio/webm"},
            {".aac", "audio/x-aac"},
            {".aif", "audio/x-aiff"},
            {".aiff", "audio/x-aiff"},
            {".aifc", "audio/x-aiff"},
            {".caf", "audio/x-caf"},
            {".flac", "audio/x-flac"},
            {".mka", "audio/x-matroska"},
            {".m3u", "audio/x-mpegurl"},
            {".wax", "audio/x-ms-wax"},
            {".wma", "audio/x-ms-wma"},
            {".ram", "audio/x-pn-realaudio"},
            {".ra", "audio/x-pn-realaudio"},
            {".rmp", "audio/x-pn-realaudio-plugin"},
            {".wav", "audio/x-wav"},
            {".xm", "audio/xm"},
            {".cdx", "chemical/x-cdx"},
            {".cif", "chemical/x-cif"},
            {".cmdf", "chemical/x-cmdf"},
            {".cml", "chemical/x-cml"},
            {".csml", "chemical/x-csml"},
            {".xyz", "chemical/x-xyz"},
            {".bmp", "image/bmp"},
            {".cgm", "image/cgm"},
            {".g3", "image/g3fax"},
            {".gif", "image/gif"},
            {".ief", "image/ief"},
            {".jpeg", "image/jpeg"},
            {".jpg", "image/jpeg"},
            {".jpe", "image/jpeg"},
            {".ktx", "image/ktx"},
            {".png", "image/png"},
            {".btif", "image/prs.btif"},
            {".sgi", "image/sgi"},
            {".svg", "image/svg+xml"},
            {".svgz", "image/svg+xml"},
            {".tiff", "image/tiff"},
            {".tif", "image/tiff"},
            {".psd", "image/vnd.adobe.photoshop"},
            {".uvi", "image/vnd.dece.graphic"},
            {".uvvi", "image/vnd.dece.graphic"},
            {".uvg", "image/vnd.dece.graphic"},
            {".uvvg", "image/vnd.dece.graphic"},
            {".sub", "text/vnd.dvb.subtitle"},
            {".djvu", "image/vnd.djvu"},
            {".djv", "image/vnd.djvu"},
            {".dwg", "image/vnd.dwg"},
            {".dxf", "image/vnd.dxf"},
            {".fbs", "image/vnd.fastbidsheet"},
            {".fpx", "image/vnd.fpx"},
            {".fst", "image/vnd.fst"},
            {".mmr", "image/vnd.fujixerox.edmics-mmr"},
            {".rlc", "image/vnd.fujixerox.edmics-rlc"},
            {".mdi", "image/vnd.ms-modi"},
            {".wdp", "image/vnd.ms-photo"},
            {".npx", "image/vnd.net-fpx"},
            {".wbmp", "image/vnd.wap.wbmp"},
            {".xif", "image/vnd.xiff"},
            {".webp", "image/webp"},
            {".3ds", "image/x-3ds"},
            {".ras", "image/x-cmu-raster"},
            {".cmx", "image/x-cmx"},
            {".fh", "image/x-freehand"},
            {".fhc", "image/x-freehand"},
            {".fh4", "image/x-freehand"},
            {".fh5", "image/x-freehand"},
            {".fh7", "image/x-freehand"},
            {".ico", "image/x-icon"},
            {".sid", "image/x-mrsid-image"},
            {".pcx", "image/x-pcx"},
            {".pic", "image/x-pict"},
            {".pct", "image/x-pict"},
            {".pnm", "image/x-portable-anymap"},
            {".pbm", "image/x-portable-bitmap"},
            {".pgm", "image/x-portable-graymap"},
            {".ppm", "image/x-portable-pixmap"},
            {".rgb", "image/x-rgb"},
            {".tga", "image/x-tga"},
            {".xbm", "image/x-xbitmap"},
            {".xpm", "image/x-xpixmap"},
            {".xwd", "image/x-xwindowdump"},
            {".eml", "message/rfc822"},
            {".mime", "message/rfc822"},
            {".igs", "model/iges"},
            {".iges", "model/iges"},
            {".msh", "model/mesh"},
            {".mesh", "model/mesh"},
            {".silo", "model/mesh"},
            {".dae", "model/vnd.collada+xml"},
            {".dwf", "model/vnd.dwf"},
            {".gdl", "model/vnd.gdl"},
            {".gtw", "model/vnd.gtw"},
            {".mts", "model/vnd.mts"},
            {".vtu", "model/vnd.vtu"},
            {".wrl", "model/vrml"},
            {".vrml", "model/vrml"},
            {".x3db", "model/x3d+binary"},
            {".x3dbz", "model/x3d+binary"},
            {".x3dv", "model/x3d+vrml"},
            {".x3dvz", "model/x3d+vrml"},
            {".x3d", "model/x3d+xml"},
            {".x3dz", "model/x3d+xml"},
            {".appcache", "text/cache-manifest"},
            {".ics", "text/calendar"},
            {".ifb", "text/calendar"},
            {".css", "text/css"},
            {".csv", "text/csv"},
            {".html", "text/html"},
            {".htm", "text/html"},
            {".n3", "text/n3"},
            {".txt", "text/plain"},
            {".text", "text/plain"},
            {".conf", "text/plain"},
            {".def", "text/plain"},
            {".list", "text/plain"},
            {".log", "text/plain"},
            {".in", "text/plain"},
            {".dsc", "text/prs.lines.tag"},
            {".rtx", "text/richtext"},
            {".sgml", "text/sgml"},
            {".sgm", "text/sgml"},
            {".tsv", "text/tab-separated-values"},
            {".t", "text/troff"},
            {".tr", "text/troff"},
            {".roff", "text/troff"},
            {".man", "text/troff"},
            {".me", "text/troff"},
            {".ms", "text/troff"},
            {".ttl", "text/turtle"},
            {".uri", "text/uri-list"},
            {".uris", "text/uri-list"},
            {".urls", "text/uri-list"},
            {".vcard", "text/vcard"},
            {".curl", "text/vnd.curl"},
            {".dcurl", "text/vnd.curl.dcurl"},
            {".scurl", "text/vnd.curl.scurl"},
            {".mcurl", "text/vnd.curl.mcurl"},
            {".fly", "text/vnd.fly"},
            {".flx", "text/vnd.fmi.flexstor"},
            {".gv", "text/vnd.graphviz"},
            {".3dml", "text/vnd.in3d.3dml"},
            {".spot", "text/vnd.in3d.spot"},
            {".jad", "text/vnd.sun.j2me.app-descriptor"},
            {".wml", "text/vnd.wap.wml"},
            {".wmls", "text/vnd.wap.wmlscript"},
            {".s", "text/x-asm"},
            {".asm", "text/x-asm"},
            {".c", "text/x-c"},
            {".cc", "text/x-c"},
            {".cxx", "text/x-c"},
            {".cpp", "text/x-c"},
            {".h", "text/x-c"},
            {".hh", "text/x-c"},
            {".dic", "text/x-c"},
            {".f", "text/x-fortran"},
            {".for", "text/x-fortran"},
            {".f77", "text/x-fortran"},
            {".f90", "text/x-fortran"},
            {".java", "text/x-java-source"},
            {".opml", "text/x-opml"},
            {".p", "text/x-pascal"},
            {".pas", "text/x-pascal"},
            {".nfo", "text/x-nfo"},
            {".etx", "text/x-setext"},
            {".sfv", "text/x-sfv"},
            {".uu", "text/x-uuencode"},
            {".vcs", "text/x-vcalendar"},
            {".vcf", "text/x-vcard"},
            {".3gp", "video/3gpp"},
            {".3g2", "video/3gpp2"},
            {".h261", "video/h261"},
            {".h263", "video/h263"},
            {".h264", "video/h264"},
            {".jpgv", "video/jpeg"},
            {".jpm", "video/jpm"},
            {".jpgm", "video/jpm"},
            {".mj2", "video/mj2"},
            {".mjp2", "video/mj2"},
            {".mp4", "video/mp4"},
            {".mp4v", "video/mp4"},
            {".mpg4", "video/mp4"},
            {".mpeg", "video/mpeg"},
            {".mpg", "video/mpeg"},
            {".mpe", "video/mpeg"},
            {".m1v", "video/mpeg"},
            {".m2v", "video/mpeg"},
            {".ogv", "video/ogg"},
            {".qt", "video/quicktime"},
            {".mov", "video/quicktime"},
            {".uvh", "video/vnd.dece.hd"},
            {".uvvh", "video/vnd.dece.hd"},
            {".uvm", "video/vnd.dece.mobile"},
            {".uvvm", "video/vnd.dece.mobile"},
            {".uvp", "video/vnd.dece.pd"},
            {".uvvp", "video/vnd.dece.pd"},
            {".uvs", "video/vnd.dece.sd"},
            {".uvvs", "video/vnd.dece.sd"},
            {".uvv", "video/vnd.dece.video"},
            {".uvvv", "video/vnd.dece.video"},
            {".dvb", "video/vnd.dvb.file"},
            {".fvt", "video/vnd.fvt"},
            {".mxu", "video/vnd.mpegurl"},
            {".m4u", "video/vnd.mpegurl"},
            {".pyv", "video/vnd.ms-playready.media.pyv"},
            {".uvu", "video/vnd.uvvu.mp4"},
            {".uvvu", "video/vnd.uvvu.mp4"},
            {".viv", "video/vnd.vivo"},
            {".webm", "video/webm"},
            {".f4v", "video/x-f4v"},
            {".fli", "video/x-fli"},
            {".flv", "video/x-flv"},
            {".m4v", "video/x-m4v"},
            {".mkv", "video/x-matroska"},
            {".mk3d", "video/x-matroska"},
            {".mks", "video/x-matroska"},
            {".mng", "video/x-mng"},
            {".asf", "video/x-ms-asf"},
            {".asx", "video/x-ms-asf"},
            {".vob", "video/x-ms-vob"},
            {".wm", "video/x-ms-wm"},
            {".wmv", "video/x-ms-wmv"},
            {".wmx", "video/x-ms-wmx"},
            {".wvx", "video/x-ms-wvx"},
            {".avi", "video/x-msvideo"},
            {".movie", "video/x-sgi-movie"},
            {".smv", "video/x-smv"},
            {".ice", "x-conference/x-cooltalk"},
            {".323", "text/h323"},
            {".3gp2", "video/3gpp2"},
            {".3gpp", "video/3gpp"},
            {".aa", "audio/audible"},
            {".aaf", "application/octet-stream"},
            {".aax", "audio/vnd.audible.aax"},
            {".ac3", "audio/ac3"},
            {".aca", "application/octet-stream"},
            {".accda", "application/msaccess.addin"},
            {".accdb", "application/msaccess"},
            {".accdc", "application/msaccess.cab"},
            {".accde", "application/msaccess"},
            {".accdr", "application/msaccess.runtime"},
            {".accdt", "application/msaccess"},
            {".accdw", "application/msaccess.webapplication"},
            {".accft", "application/msaccess.ftemplate"},
            {".acx", "application/internet-property-stream"},
            {".AddIn", "text/xml"},
            {".ade", "application/msaccess"},
            {".adobebridge", "application/x-bridge-url"},
            {".ADT", "audio/vnd.dlna.adts"},
            {".ADTS", "audio/aac"},
            {".amc", "application/x-mpeg"},
            {".art", "image/x-jg"},
            {".asa", "application/xml"},
            {".asax", "application/xml"},
            {".ascx", "application/xml"},
            {".asd", "application/octet-stream"},
            {".ashx", "application/xml"},
            {".asi", "application/octet-stream"},
            {".asmx", "application/xml"},
            {".aspx", "application/xml"},
            {".asr", "video/x-ms-asf"},
            {".axs", "application/olescript"},
            {".bas", "text/plain"},
            {".calx", "application/vnd.ms-office.calx"},
            {".cd", "text/plain"},
            {".cdda", "audio/aiff"},
            {".cnf", "text/plain"},
            {".config", "application/xml"},
            {".contact", "text/x-ms-contact"},
            {".coverage", "application/xml"},
            {".cs", "text/plain"},
            {".csdproj", "text/plain"},
            {".csproj", "text/plain"},
            {".cur", "application/octet-stream"},
            {".dat", "application/octet-stream"},
            {".datasource", "application/xml"},
            {".dbproj", "text/plain"},
            {".dgml", "application/xml"},
            {".dib", "image/bmp"},
            {".dif", "video/x-dv"},
            {".disco", "text/xml"},
            {".dll.config", "text/xml"},
            {".dlm", "text/dlm"},
            {".dsp", "application/octet-stream"},
            {".dsw", "text/plain"},
            {".dtsConfig", "text/xml"},
            {".dv", "video/x-dv"},
            {".dwp", "application/octet-stream"},
            {".etl", "application/etl"},
            {".exe.config", "text/xml"},
            {".fif", "application/fractals"},
            {".filters", "Application/xml"},
            {".fla", "application/octet-stream"},
            {".flr", "x-world/x-vrml"},
            {".fsscript", "application/fsharp-script"},
            {".fsx", "application/fsharp-script"},
            {".generictest", "application/xml"},
            {".group", "text/x-ms-group"},
            {".gsm", "audio/x-gsm"},
            {".gz", "application/x-gzip"},
            {".hdml", "text/x-hdml"},
            {".hhc", "application/x-oleobject"},
            {".hhk", "application/octet-stream"},
            {".hhp", "application/octet-stream"},
            {".hpp", "text/plain"},
            {".hta", "application/hta"},
            {".htc", "text/x-component"},
            {".htt", "text/webviewhtml"},
            {".hxa", "application/xml"},
            {".hxc", "application/xml"},
            {".hxd", "application/octet-stream"},
            {".hxe", "application/xml"},
            {".hxf", "application/xml"},
            {".hxh", "application/octet-stream"},
            {".hxi", "application/octet-stream"},
            {".hxk", "application/xml"},
            {".hxq", "application/octet-stream"},
            {".hxr", "application/octet-stream"},
            {".hxs", "application/octet-stream"},
            {".hxt", "text/html"},
            {".hxv", "application/xml"},
            {".hxw", "application/octet-stream"},
            {".hxx", "text/plain"},
            {".i", "text/plain"},
            {".idl", "text/plain"},
            {".iii", "application/x-iphone"},
            {".inc", "text/plain"},
            {".inf", "application/octet-stream"},
            {".inl", "text/plain"},
            {".ins", "application/x-internet-signup"},
            {".ipa", "application/x-itunes-ipa"},
            {".ipg", "application/x-itunes-ipg"},
            {".ipproj", "text/plain"},
            {".ipsw", "application/x-itunes-ipsw"},
            {".iqy", "text/x-ms-iqy"},
            {".isp", "application/x-internet-signup"},
            {".ite", "application/x-itunes-ite"},
            {".itlp", "application/x-itunes-itlp"},
            {".itms", "application/x-itunes-itms"},
            {".itpc", "application/x-itunes-itpc"},
            {".IVF", "video/x-ivf"},
            {".jck", "application/liquidmotion"},
            {".jcz", "application/liquidmotion"},
            {".jfif", "image/pjpeg"},
            {".jpb", "application/octet-stream"},
            {".jsx", "text/jscript"},
            {".jsxbin", "text/plain"},
            {".library-ms", "application/windows-library+xml"},
            {".lit", "application/x-ms-reader"},
            {".loadtest", "application/xml"},
            {".lpk", "application/octet-stream"},
            {".lsf", "video/x-la-asf"},
            {".lst", "text/plain"},
            {".lsx", "video/x-la-asf"},
            {".m2t", "video/vnd.dlna.mpeg-tts"},
            {".m2ts", "video/vnd.dlna.mpeg-tts"},
            {".m4a", "audio/m4a"},
            {".m4b", "audio/m4b"},
            {".m4p", "audio/m4p"},
            {".m4r", "audio/x-m4r"},
            {".mac", "image/x-macpaint"},
            {".mak", "text/plain"},
            {".manifest", "application/x-ms-manifest"},
            {".map", "text/plain"},
            {".master", "application/xml"},
            {".mda", "application/msaccess"},
            {".mde", "application/msaccess"},
            {".mdp", "application/octet-stream"},
            {".mfp", "application/x-shockwave-flash"},
            {".mht", "message/rfc822"},
            {".mhtml", "message/rfc822"},
            {".mix", "application/octet-stream"},
            {".mk", "text/plain"},
            {".mno", "text/xml"},
            {".mod", "video/mpeg"},
            {".mp2v", "video/mpeg"},
            {".mpa", "video/mpeg"},
            {".mpf", "application/vnd.ms-mediapackage"},
            {".mpv2", "video/mpeg"},
            {".mqv", "video/quicktime"},
            {".mso", "application/octet-stream"},
            {".mtx", "application/xml"},
            {".mvc", "application/x-miva-compiled"},
            {".mxp", "application/x-mmxp"},
            {".nws", "message/rfc822"},
            {".ocx", "application/octet-stream"},
            {".odh", "text/plain"},
            {".odl", "text/plain"},
            {".one", "application/onenote"},
            {".onea", "application/onenote"},
            {".orderedtest", "application/xml"},
            {".osdx", "application/opensearchdescription+xml"},
            {".pcast", "application/x-podcast"},
            {".pcz", "application/octet-stream"},
            {".pict", "image/pict"},
            {".pkgdef", "text/plain"},
            {".pkgundef", "text/plain"},
            {".pko", "application/vnd.ms-pki.pko"},
            {".pma", "application/x-perfmon"},
            {".pmc", "application/x-perfmon"},
            {".pmr", "application/x-perfmon"},
            {".pmw", "application/x-perfmon"},
            {".pnt", "image/x-macpaint"},
            {".pntg", "image/x-macpaint"},
            {".pnz", "image/png"},
            {".ppa", "application/vnd.ms-powerpoint"},
            {".prm", "application/octet-stream"},
            {".prx", "application/octet-stream"},
            {".psc1", "application/PowerShell"},
            {".psess", "application/xml"},
            {".psm", "application/octet-stream"},
            {".psp", "application/octet-stream"},
            {".pwz", "application/vnd.ms-powerpoint"},
            {".qht", "text/x-html-insertion"},
            {".qhtm", "text/x-html-insertion"},
            {".qti", "image/x-quicktime"},
            {".qtif", "image/x-quicktime"},
            {".qtl", "application/x-quicktimeplayer"},
            {".rat", "application/rat-file"},
            {".rc", "text/plain"},
            {".rc2", "text/plain"},
            {".rct", "text/plain"},
            {".rdlc", "application/xml"},
            {".resx", "application/xml"},
            {".rf", "image/vnd.rn-realflash"},
            {".rgs", "text/plain"},
            {".rpm", "audio/x-pn-realaudio-plugin"},
            {".rqy", "text/x-ms-rqy"},
            {".ruleset", "application/xml"},
            {".safariextz", "application/x-safari-safariextz"},
            {".sct", "text/scriptlet"},
            {".sd2", "audio/x-sd2"},
            {".sea", "application/octet-stream"},
            {".searchConnector-ms", "application/windows-search-connector+xml"},
            {".settings", "application/xml"},
            {".sgimb", "application/x-sgimb"},
            {".shtml", "text/html"},
            {".sitemap", "application/xml"},
            {".skin", "application/xml"},
            {".slk", "application/vnd.ms-excel"},
            {".sln", "text/plain"},
            {".slupkg-ms", "application/x-ms-license"},
            {".smd", "audio/x-smd"},
            {".smx", "audio/x-smd"},
            {".smz", "audio/x-smd"},
            {".snippet", "application/xml"},
            {".snp", "application/octet-stream"},
            {".sol", "text/plain"},
            {".sor", "text/plain"},
            {".srf", "text/plain"},
            {".SSISDeploymentManifest", "text/xml"},
            {".ssm", "application/streamingmedia"},
            {".sst", "application/vnd.ms-pki.certstore"},
            {".testrunconfig", "application/xml"},
            {".testsettings", "application/xml"},
            {".tgz", "application/x-compressed"},
            {".thn", "application/octet-stream"},
            {".tlh", "text/plain"},
            {".tli", "text/plain"},
            {".toc", "application/octet-stream"},
            {".trx", "application/xml"},
            {".ts", "video/vnd.dlna.mpeg-tts"},
            {".tts", "video/vnd.dlna.mpeg-tts"},
            {".uls", "text/iuls"},
            {".user", "text/plain"},
            {".vb", "text/plain"},
            {".vbdproj", "text/plain"},
            {".vbk", "video/mpeg"},
            {".vbproj", "text/plain"},
            {".vbs", "text/vbscript"},
            {".vcproj", "Application/xml"},
            {".vcxproj", "Application/xml"},
            {".vddproj", "text/plain"},
            {".vdp", "text/plain"},
            {".vdproj", "text/plain"},
            {".vdx", "application/vnd.ms-visio.viewer"},
            {".vml", "text/xml"},
            {".vscontent", "application/xml"},
            {".vsct", "text/xml"},
            {".vsi", "application/ms-vsi"},
            {".vsix", "application/vsix"},
            {".vsixlangpack", "text/xml"},
            {".vsixmanifest", "text/xml"},
            {".vsmdi", "application/xml"},
            {".vspscc", "text/plain"},
            {".vsscc", "text/plain"},
            {".vssettings", "text/xml"},
            {".vssscc", "text/plain"},
            {".vstemplate", "text/xml"},
            {".vsto", "application/x-ms-vsto"},
            {".vsx", "application/vnd.visio"},
            {".vtx", "application/vnd.visio"},
            {".wave", "audio/wav"},
            {".wbk", "application/msword"},
            {".webarchive", "application/x-safari-webarchive"},
            {".webtest", "application/xml"},
            {".wiq", "application/xml"},
            {".wiz", "application/msword"},
            {".WLMP", "application/wlmoviemaker"},
            {".wlpginstall", "application/x-wlpg-detect"},
            {".wlpginstall3", "application/x-wlpg3-detect"},
            {".wmp", "video/x-ms-wmp"},
            {".wrz", "x-world/x-vrml"},
            {".wsc", "text/scriptlet"},
            {".x", "application/directx"},
            {".xaf", "x-world/x-vrml"},
            {".xdr", "text/plain"},
            {".xld", "application/vnd.ms-excel"},
            {".xlk", "application/vnd.ms-excel"},
            {".xll", "application/vnd.ms-excel"},
            {".xmta", "application/xml"},
            {".xof", "x-world/x-vrml"},
            {".XOML", "text/plain"},
            {".xrm-ms", "text/xml"},
            {".xsc", "application/xml"},
            {".xsd", "text/xml"},
            {".xsf", "text/xml"},
            {".xsn", "application/octet-stream"},
            {".xss", "application/xml"},
            {".xtp", "application/octet-stream"},
            {".z", "application/x-compress"},
        };

    public static string GetMimeType(string fileName)
    {
        if (fileName == null)
            throw new ArgumentNullException("fileName");

        string extension = Path.GetExtension(fileName);

        string mimeType;
        if (!String.IsNullOrEmpty(extension) && _mappings.TryGetValue(extension, out mimeType))
            return mimeType;

        return "application/octet-stream";
    }
}
