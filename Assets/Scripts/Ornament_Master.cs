using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.IO;
using System.Linq;

[System.Serializable]
public class wikarBase
{
    public string unityID = null;//user unity id
    public string clowderID = null;//metadata entry
    public Vector3 localPosition = Vector3.zero;
    public Quaternion localRotation = Quaternion.identity;
}

[System.Serializable]
public class Ornament2020 : wikarBase
{
    
    public int shapeID = 0; //ultimate shape of ornament
    //public int[] collected = {0,0,0,0,0};//color ID + amount collected
    public Color[] colors = { Color.red, Color.blue , Color.green };
    //public float size = 1f;
    public int bonusAttribute = 0;
}

public class wikarManager<T> : MonoBehaviour where T : wikarBase
{
    public bool saveLocalCopy;
    private bool isFetching = false;
    string typeParameterType = typeof(T).ToString();
    public enum userType
    {
        client,
        admin
    };
    public userType user = userType.admin;
    public string trackedDataset = "5f83a7464f0c54bf4e0c0530";
    [SerializeField]
    public IList<T> Collection;

    //#endregion
    public List<GameObject> GameObjectCollection;
    public int getIndexByID(string id)
    {
        for (int i = 0; i < Collection.Count; i++)
        {
            if (Collection[i].unityID.Equals(id))
            {
                Debug.Log(Collection[i].unityID);
                return i;
            }
        }
        Debug.Log("didn't find id");
        return -1;
    }
   
    public T getObjectByID(string id)
    {
        foreach (T item in Collection)
        {
            if (item.unityID.Equals(id))
            {
                return item;
            }
        }
        //Debug.Log("didn't find id");
        return null;
    }
    public JSONObject createObjectJson(T input)
    {
        JSONObject submission = new JSONObject();
        submission.AddField(input.ToString(), new JSONObject(JsonUtility.ToJson(input)));
        submission = ClowderHelper.CreateMetadataPost(submission, input.unityID, "https://clowder.ncsa.illinois.edu/api/extractors/ncsa.unity." + typeParameterType +"."+ input.unityID);//hand it ID  localSubmission.unityID
        return submission;
    }

    public void PostMetaData(string ClowderID, JSONObject report)
    {
        string url = ClowderBridge.Instance.CLOWDERURL + "datasets/" + ClowderID + "/metadata.jsonld" + "?key=" + ClowderBridge.Instance.API_KEY;
        ClowderBridge.Instance.StartCoroutine(ClowderBridge.Instance.PostRequest(url, report, returnValue =>
        { }));
    }
    public void DeleteObject(T input, System.Action callback = null)//this will take care of list of class but not gameobject in list
    {
        ClowderBridge.Instance.DeleteMetaData(trackedDataset, "ncsa.unity." + typeParameterType + "." + input.unityID, ClowderBridge.ClowderType.dataset);
        //T target = getIndexByID(input.unityID);
        Collection.Remove(input);
        if (callback != null)
        {
            callback.Invoke();
        }
    }
    public void UpdateMetadata(T localSubmission)
    {
        ClowderBridge.Instance.UpdateMetaData(trackedDataset, ClowderBridge.ClowderType.dataset, createObjectJson(localSubmission)); //"ncsa.unity." + array[i + 1]
    }
    public void fetchObjects(System.Action callback = null)
    {
        Debug.Log("fetching: "+ typeof(T).ToString());
        if (!isFetching)
        {
            isFetching = true;
            ClowderBridge.Instance.StartCoroutine(ClowderBridge.Instance.GetExtractorMetaData(trackedDataset, "ncsa.unity."+ typeParameterType, ClowderBridge.ClowderType.dataset, result =>
            {
                Collection.Clear();
                //Debug.Log(result);
                if (saveLocalCopy)
                {
                    saveToFile(result);
                }
                loadFromJSON(result);
                isFetching = false;
                callback.Invoke();
            }));
        }
    }
    public void saveToFile(JSONObject input)
    {
        //Debug.Log("SAVING FILE");
        //Debug.Log(folderpath + "/" + trackedDataset + ".json");
        System.IO.File.WriteAllText(folderpath + "/"+ trackedDataset + ".json", input.Print());
    }
    private string GetWorkingDirectory(string game_path)
    {
        int dir_char_idx = game_path.LastIndexOf("\\");
        if (dir_char_idx == -1)
            return game_path;
        return game_path.Substring(0, dir_char_idx + 1);
    }
    private string ConvertToBackslashPath(string dataPath)
    {
        return dataPath.Replace("/", "\\\\"); //I do this because the windows command likes path to look like this: "C:\\CoolDesktop\\cool.html"
    }
    private string _folderpath = null;
    private string folderpath
    {
        set
        {
            _folderpath = value;
        }
        get
        {
            if (_folderpath == null)
            {
                string newPath = ConvertToBackslashPath(Application.streamingAssetsPath);
                if (Application.isEditor) newPath = GetWorkingDirectory(newPath) + "StreamingAssets";
                _folderpath = newPath;
            }
            return _folderpath;
        }
    }


    public bool tryLoadLocalJSON()
    {
        String[] foundFiles = Directory.Exists(folderpath) ?
            Directory.GetFiles(folderpath, "*.json", SearchOption.AllDirectories).ToArray()
            : null;

        if (foundFiles != null && foundFiles.Length > 0)//if we have a result of our search
        {
            String[] fileNames = Directory.GetFiles(folderpath, "*.json", SearchOption.AllDirectories)
            .Select(fileName => Path.GetFileNameWithoutExtension(fileName))
            .ToArray();

            //UnityEngine.Debug.Log("Files detected:");
            //foreach (var item in fileNames)
            //{
            //    UnityEngine.Debug.Log(item);//list them in console
            //}
            
            for (int i = 0; i < fileNames.Length; i++)
            {
                if (fileNames[i].Equals(trackedDataset))
                {
                    //Debug.Log(File.ReadAllText(foundFiles[i]));
                    Debug.Log("loading: " + fileNames[i]);
                    if (loadFromJSON(new JSONObject(File.ReadAllText(foundFiles[i]))))
                    {
                        Debug.Log("loading complete");
                        return true;
                    }
                    else
                        Debug.Log("failed to load");
                    return false;

                }
            }
            Debug.Log("no matching files found");
            return false;
        }
        else
        {
            Debug.Log("no local files found :<");
            return false;
        }
    }

    public bool loadFromJSON(JSONObject input)
    {
        List<JSONObject> unpacked = input.list;
        string lastURL = string.Empty;
        if (Collection == null)
        {
            Collection = new List<T>();
        }
        for (int i = 0; i < unpacked.Count; i++)
        {
            if (unpacked[i]["content"].GetField(typeParameterType))
            {
                Collection.Add(JsonUtility.FromJson<T>(unpacked[i]["content"].GetField(typeParameterType).ToString()));
            }
            
        }
        return true;
    }
}
[Serializable]
public struct colorPreset
{
    public Color a;
    public Color b;
    public Color c;
}

public class Ornament_Master : wikarManager<Ornament2020>
{
    #region Singleton / Start / Awake
    private static Ornament_Master instance;
    public static Ornament_Master Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject bridge = new GameObject("Ornament_Master");
                instance = bridge.AddComponent<Ornament_Master>();
            }
            return instance;
        }
    }

    void Awake()
    {
        instance = this;
    }
    #endregion
    
    public colorPreset[] colorPresets;
    public List<Mesh> shapes;
    public List<float> additonalOffsetByShape;
    public Ornament2020 localSubmission;
    
    public bool generateRandomID = false;
    public bool generateRandomOrnament = true;
    public int didPost = 0;
    public float groupSizeFactor = 10f;
    public GameObject treeTarget;
    public GameObject ornamentPrefab;
    public GameObject submitOrnament(RaycastHit position, Ornament2020 parameters)
    {
        //if ((didPost > 0 && user == userType.client))
        //{
        //    //don't post without at least a name
        //    Debug.Log("Launch Modal window about if they wanna submit");
        //}
        //else
        //{
            Vector3 lookTarget = position.normal;
            lookTarget.y /= 4;
            localSubmission = parameters;
            localSubmission.localPosition = treeTarget.transform.InverseTransformPoint(position.point);
            //Vector3 adjustLookTarget = position.normal;
            //adjustLookTarget.y = 0;//((position.normal + (Vector3.up * position.normal.y)) / 2).normalized;
            //localSubmission.localRotation = Quaternion.LookRotation(treeTarget.transform.InverseTransformDirection(position.normal), Vector3.up) * Quaternion.Euler(-90,0,0);
            localSubmission.localRotation = Quaternion.Inverse(treeTarget.transform.rotation) * Quaternion.LookRotation(-lookTarget, Vector3.up);
            localSubmission.unityID = masterSwitchInterface.Instance.getUnityID();
            
            //if (generateRandomOrnament)//randomize ornament values
            //{
            //    localSubmission.shapeID = UnityEngine.Random.Range(0, shapes.Count - 1);
            //    //localSubmission.size = UnityEngine.Random.Range(1f, 3f);
            //}
            var postedOrnament = createOrnament(localSubmission, localSubmission.localPosition);
            postedOrnament.SetActive(false);

            Collection.Add(localSubmission);
            //PostMetaData(trackedDataset, createObjectJson(localSubmission));
            UpdateMetadata(localSubmission);
            PlayerPrefs.SetInt("postedOrnament", 1);
            return postedOrnament;
        //}
        //return null;
    }
    public void Start()
    {
        Collection = new List<Ornament2020>();
        localSubmission = new Ornament2020();
        
    }
    public void fetchOrnaments()
    {
        fetchObjects(updateOrnamentObjects);//update our pool of objects with our type and call the method after to update gameobjects
    }
    public GameObject createOrnament(Ornament2020 data, Vector3 localPosition)//this HAPPENS IN WORLD POSITION
    {
        //Vector3 faceDirection = treeTarget.transform.localPosition - localPosition;
        //faceDirection = new Vector3(0, faceDirection.y, 90);


        GameObject newOrnament = GameObject.Instantiate(ornamentPrefab, Vector3.zero, Quaternion.identity);
        newOrnament.transform.SetParent(treeTarget.transform);
        newOrnament.transform.localPosition = localPosition;
        //newOrnament.transform.localPosition += Quaternion.LookRotation(faceDirection, Vector3.up).eulerAngles.normalized * (groupSizeFactor / (2 * treeTarget.transform.localScale.x));
        //newOrnament.transform.localRotation = Quaternion.LookRotation(faceDirection, Vector3.up) * Quaternion.Euler(0,0,90f);
        newOrnament.transform.localRotation = data.localRotation * Quaternion.Euler(270, 0, 180);
        newOrnament.transform.position += -newOrnament.transform.up * ((groupSizeFactor / (2))); //* ((groupSizeFactor / (2 * treeTarget.transform.localScale.x));
        
        newOrnament.transform.localScale = Vector3.one * groupSizeFactor;
        newOrnament.GetComponent<MeshFilter>().mesh = shapes[data.shapeID];

        newOrnament.GetComponent<Ornament_2020_Interface>().localIdentity = data;
        newOrnament.GetComponent<Ornament_2020_Interface>().setMesh(shapes[data.shapeID]);
        GameObjectCollection.Add(newOrnament);
        return newOrnament;
    }

    public GameObject modifyOrnament(int index, Ornament2020 data, Vector3 localPositon)
    {
        //Vector3 faceDirection = treeTarget.transform.localPosition - localPositon;
        //faceDirection = new Vector3(0, faceDirection.y, 0);

        var newOrnament = GameObjectCollection[index];
        this.gameObject.SetActive(true);//make sure it's true
        newOrnament.transform.localPosition = localPositon;
        newOrnament.transform.position += -newOrnament.transform.up * ((groupSizeFactor / (2))); //* ((groupSizeFactor / (2 * treeTarget.transform.localScale.x));
        //newOrnament.transform.localRotation = Quaternion.LookRotation(faceDirection, Vector3.up) * Quaternion.Euler(0, 0, 90f);
        newOrnament.transform.localRotation = data.localRotation * Quaternion.Euler(270,0,180);
        newOrnament.transform.localScale = Vector3.one * groupSizeFactor;

        newOrnament.GetComponent<Ornament_2020_Interface>().localIdentity = data;
        newOrnament.GetComponent<Ornament_2020_Interface>().setMesh(shapes[data.shapeID]);
        return newOrnament;
    }

    

    public void updateOrnamentObjects()
    {
        StartCoroutine(updateOrnamentObjectsCoroutine());
    }
    public IEnumerator updateOrnamentObjectsCoroutine()
    {
        if (GameObjectCollection.Count > Collection.Count)
        {
            int difference = GameObjectCollection.Count - Collection.Count;
            int endOfCollection = GameObjectCollection.Count - 1;
            for (int i = endOfCollection; i > endOfCollection - difference; i--)
            {
                //Debug.Log("HERE 1");
                //GameObjectCollection.RemoveAt(i);
                //Debug.Log("HERE 1 REMOVE AT complete");
                GameObject.Destroy(GameObjectCollection[i]);
                //Debug.Log("HERE 1 DESTROY");
                yield return null;
            }
            GameObjectCollection.TrimExcess();
            //Debug.Log("end of HERE 1");
        }
        yield return null;
        if (Collection.Count >= GameObjectCollection.Count)
        {
            int difference = Collection.Count - GameObjectCollection.Count;
            for (int i = 0; i < GameObjectCollection.Count; i++)
            {
                //Debug.Log("HERE 2");
                modifyOrnament(i, Collection[i] as Ornament2020, Collection[i].localPosition);
                yield return null;
            }
            for (int i = 0; i < difference; i++)
            {
                //Debug.Log("HERE 3");
                createOrnament(Collection[i] as Ornament2020, Collection[i].localPosition);
                yield return null;
            }
        }
        yield return null;
    }

    public void deleteAllOrnaments()
    {
        ClowderBridge.Instance.DeleteMetaData(trackedDataset, "ncsa.unity", ClowderBridge.ClowderType.dataset);
    }


}