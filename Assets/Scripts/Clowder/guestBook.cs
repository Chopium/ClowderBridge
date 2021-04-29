using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
[System.Serializable]
public class GuestSignature : wikarBase
{
    public string name = null;
    public string location = null;
    public string message = null;
    public string dateTime = null;
    public int points = 0;
    public bool approved = false;
}

public class guestBook : wikarManager<GuestSignature>
{
    public GuestSignature localSubmission;
    public TMP_InputField nameInput;
    public TMP_InputField locationInput;
    public TMP_InputField messageInput;
    public int collectionLength = 0;
    public TMP_Text readout;
    private float submittedFloat;
    private float totalFloat;
    //public bool isFetching = false;//lockout from fetching more than once at a time
    //public bool generateRandomID = false;
    public int didPost = 0;

    #region Singleton / Start / Awake
    private static guestBook instance;
    public static guestBook Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject guestBook = new GameObject("guestBook");
                instance = guestBook.AddComponent<guestBook>();
            }
            return instance;
        }
    }

    void Awake()
    {
        instance = this;
    }
    #endregion
    private void Update()
    {
        if (Collection != null)
        {
            collectionLength = Collection.Count;
        }
        else
        {
            Debug.Log("collection is null");
        }
        
    }


    private void Start()
    {
        localSubmission = new GuestSignature();
        Collection = new List<GuestSignature>();
        didPost = PlayerPrefs.GetInt("postedSig"); 
        //fetchMessages();
    }

    string fullLog;
    public void updateMessageLog()
    {
        if (masterSwitchInterface.Instance.user == masterSwitchInterface.userType.admin)//don't care to do this on client machines
        {
            fullLog = string.Empty;
            foreach (GuestSignature item in Collection)
            {
                if (item.approved || user == userType.admin)
                {
                    if (user == userType.admin)//show the approval link
                    {
                        fullLog += "<link=approve/" + item.unityID + "><b>Approved:</b> [" + item.approved + "]</link>";//approve item
                        fullLog += "  <link=delete/" + item.unityID + "><b>[DELETE]</b></link>";//delete item
                        fullLog += "\r\n";
                    }
                    fullLog += "<b>Date:</b> " + item.dateTime;
                    fullLog += "\r\n";
                    fullLog += "<b>From:</b> " + item.name;
                    fullLog += "\r\n";
                    fullLog += "<b>Location:</b> " + item.location;
                    fullLog += "\r\n";
                    fullLog += "<link=test><b>Points:</b>" + item.points + "</link>";
                    fullLog += "\r\n\r\n";
                    fullLog += "<margin=2em>" + item.message + "<margin=0>";
                    fullLog += "\r\n\r\n";
                }
            }
            readout.text = fullLog;
        }
    }
    public void recieveCommand(string command)
    {
        List<string> array = new List<string>(command.Split('/', '?')); //get tokens between / and ?
        if (array.Count > 1)//URL TESTING
        {
            for (int i = 0; i < array.Count; i++)
            {
                if (array[i].Equals("approve") && array.Count > i)//if url points to file
                {
                    int index = getSignatureIndexByID(array[i + 1]);
                    Collection[index].approved = !Collection[index].approved;
                    ClowderBridge.Instance.UpdateMetaData(trackedDataset, ClowderBridge.ClowderType.dataset, createSignatureJson(Collection[index])); //"ncsa.unity." + array[i + 1]
                    updateMessageLog();
                }
                if (array[i].Equals("delete") && array.Count > i)//if url points to file
                {
                    GuestSignature target = getSignatureByID(array[i + 1]);
                    DeleteObject(target, updateMessageLog);
                    //ClowderBridge.Instance.DeleteMetaData(trackedDataset, "ncsa.unity." + array[i + 1], ClowderBridge.ClowderType.dataset);
                    
                    //Collection.Remove(target);
                    //updateMessageLog();
                }
            }
        }
    }
    public int getSignatureIndexByID(string id)
    {
        Debug.Log(id);
        for (int i = 0; i < Collection.Count; i++)
        {
            Debug.Log(Collection[i].unityID);
            if (Collection[i].unityID.Equals(id))
            {
                Debug.Log(Collection[i].name);
                return i;
            }
        }
        throw new Exception("didn't find id");
    }

    public GuestSignature getSignatureByID(string id)
    {
        foreach (GuestSignature item in Collection)
        {
            if (item.unityID.Equals(id))
            {
                return item;
            }
        }
        Debug.Log("didn't find id");
        return null;
        
    }

    public JSONObject createSignatureJson(GuestSignature input)
    {
        return createObjectJson(input);
        //JSONObject submission = new JSONObject();
        //submission.AddField("GuestSignature", new JSONObject(JsonUtility.ToJson(input)));
        //submission = ClowderHelper.CreateMetadataPost(submission, input.name, "https://clowder.ncsa.illinois.edu/api/extractors/ncsa.unity." + input.unityID);//hand it ID  localSubmission.unityID
        //return submission;
    }

    public void submitSignature()
    {

        if (localSubmission.name == string.Empty)// || (didPost > 0 && user == userType.client))
        {
            //don't post without at least a name
            Debug.Log("FAILED TEST");
        }
        else
        {
            //float.TryParse(input.text, out submittedFloat);//try cast
            localSubmission.dateTime = System.DateTime.Now.ToString("dd/MM/yyyy HH:mm");

            //if (generateRandomID && user == userType.admin)
            //{
            //    localSubmission.unityID = UnityEngine.Random.Range(0, 99999).ToString();
            //}
            //else
            //{
            //    localSubmission.unityID = SystemInfo.deviceUniqueIdentifier;
            //}
            localSubmission.unityID = masterSwitchInterface.Instance.getUnityID();
            localSubmission.message = messageInput.text;
            localSubmission.name = nameInput.text;
            localSubmission.message = messageInput.text;
            localSubmission.location = locationInput.text;
            localSubmission.approved = false; //user == userType.admin ? true : false

            //PostMetaData(trackedDataset, createSignatureJson(localSubmission));
            UpdateMetadata((localSubmission));//ued to be createSignatureJson() inside

            var previousSig = getSignatureByID(localSubmission.unityID);
            if (previousSig != null)
            {
                //removing previous signature from local database
                Collection.Remove(previousSig);
            }
            //Debug.Log(report);
            localSubmission.approved = true;//let the user see their own post
            Collection.Add(localSubmission);
            updateMessageLog();
            //nameInput.text = string.Empty;
            //locationInput.text = string.Empty;
            //messageInput.text = string.Empty;
            PlayerPrefs.SetInt("postedSig", 1);
            this.transform.GetChild(0).gameObject.SetActive(false);
        }
    }
    //public void PostMetaData(string ClowderID, JSONObject report)
    //{
    //    string url = ClowderBridge.Instance.CLOWDERURL + "datasets/" + ClowderID + "/metadata.jsonld" + "?key=" + ClowderBridge.Instance.API_KEY;
    //    StartCoroutine(ClowderBridge.Instance.PostRequest(url, report, returnValue =>
    //    {
    //    }));
    //}

    public void fetchMessages()
    {
        fetchObjects(delegate
        {
            updateMessageLog();
            var localSig = getSignatureByID(localSubmission.unityID);
            if (localSig != null)
            {
                nameInput.text = localSig.name;
                locationInput.text = localSig.location;
                messageInput.text = localSig.message;
            }
            //foreach (var item in Ornament_Master.Instance.GameObjectCollection)
            //{

            //}
            //foreach (var item in Collection)
            //{

            //}
        });
        //if (!ing)
        //{
        //    isFetching = true;
        //    StartCoroutine(ClowderBridge.Instance.GetExtractorMetaData(trackedDataset, "ncsa.unity", ClowderBridge.ClowderType.dataset, result =>
        //    {
        //        Collection.Clear();
        //        List<JSONObject> unpacked = result.list;
        //        string lastURL = string.Empty;
        //        for (int i = 0; i < unpacked.Count; i++)
        //        {
        //            if (unpacked[i]["content"].GetField("GuestSignature"))
        //            {
        //                Collection.Add(JsonUtility.FromJson<GuestSignature>(unpacked[i]["content"].GetField("GuestSignature").ToString()));
        //            }
        //        }
        //        updateMessageLog();
        //        //Debug.Log("FETCH COMPLETE");
        //        isFetching = false;
        //    }));
        //}
    }
}