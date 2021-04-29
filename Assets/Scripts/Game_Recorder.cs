//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.IO;
//using UnityEngine;

//public class Game_Recorder : MonoBehaviour
//{
//    #region Singleton
//    private static Game_Recorder instance;
//    public static Game_Recorder Instance
//    {
//        get
//        {
//            if (instance == null)
//            {

//                instance = new Game_Recorder();
//            }
//            return instance;
//        }
//    }
//    void Awake()
//    {
//        instance = this;
//    }
//    #endregion
//    public struct SessionIdentity
//    {
//        public string deviceID; //represents a specific device
//        public string notes;//starting options, we will note if changed midstream
//        public string date;//time and date the session started (world)
//        public float unitySessionStartTime;//time since start of session (in app)
//        public int gameLevel;
//        public string difficulty;
//        public string platform;
//        public string version;

//        //resolution, options?
//    }
//    public SessionIdentity currentSession;//high level stuff about player (per session)

//    [SerializeField]
//    public List<FrameEvent> eventLog;
//    public struct FrameEvent
//    {
//        public float timestamp;//we will set this in here. time in session.
//        public object FromEntity; //entity that pushed this event to record
//        public object target; //did this entity relate to another entity? what was it?
//        public string announcement;//human description
//        public object eventParameter; //going to take bools, floats, ints. did a value change?
//    }

//    public FrameEvent createFrameEvent(object FromEntity, string announcement, object eventParameter, object target)
//    {
//        FrameEvent output = new FrameEvent();
//        output.timestamp = getTimeStamp();//time in physics space seconds since started
//        output.FromEntity = FromEntity;
//        output.announcement = announcement;

//        output.eventParameter = eventParameter;
//        output.target = target;

//        if (isRecording && eventLog != null)
//        {
//            if (debuglevel > 2) { Debug.Log(output.announcement + "Love, " + output.FromEntity + ". + data: " + output.eventParameter + " and target: " + output.target + " "); }
//            eventLog.Add(output);
//        }

//        return output;
//    }
//    #region transform polling structs and variables
//    //POLLING RATE will ping transforms otherwise we receieve injections of data
//    public float pollingRate = 0.05f;//every half second


//    [SerializeField]
//    [HideInInspector]//target list
//    public List<Transform> TransformsToLog;


//    [SerializeField]
//    //public List<TransformFrameEvent> TransformLog;
//    public Dictionary<int, NamedTransformList> TransformHistories;//contains all our individual transform histories, seperated. int is transformID
//    public class NamedTransformList
//    {
//        public int instanceId;//unique identifier given by unity to each transform
//        public string name;//nickname
//        public List<TransformFrameEvent> content;
//    }

//    public class TransformFrameEvent
//    {
//        public float timestamp;
//        public string transformName;

//        public Vector3? localPosition = null;
//        public Vector3? globalPosition = null;

//        public Vector3? localScale = null;
//        public Vector3? globalScale = null;

//        public Quaternion? localRotation = null;
//        public Quaternion? globalRotation = null;

//        public override string ToString()
//        {
//            string output = "";
//            output += "T: " + timestamp.ToString("0.00") + " ";
//            //if (transformName != null)
//            //{
//            //    output += "Name: " + transformName + " ";
//            //}
//            if (globalPosition != null)
//            {
//                output += "Position: " + globalPosition.Value.ToString("F4") + " ";
//            }
//            if (globalRotation != null)
//            {
//                output += "Rotation: " + globalRotation.Value.ToString("F4") + " ";
//            }
//            return output;
//        }
//    }
//    #endregion

//    public int debuglevel = 2;
//    public bool isRecording = false;
//    public bool uploadRecording = false;

//    // Start is called before the first frame update
//    void Start()
//    {
//        GameManager.Instance.gameReset.AddListener(DeleteDataVis);
//        //create containers for polled objects
//    }
//    public string[] tagsToTrack;
//    public GameObject[] objectsToTrack;
//    public void InitializeTransformLog()
//    {
//        TransformsToLog = new List<Transform>();
//        foreach (string tag in tagsToTrack)
//        {
//            GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);//player object
//            foreach (GameObject g in objects)
//            {
//                TransformsToLog.Add(g.transform);
//            }
//        }
//        foreach (GameObject o in objectsToTrack)
//        {
//            TransformsToLog.Add(o.transform);
//        }

//        //TransformsToLog.Add(Camera.main.transform);//objects tagged GROUND aka our stage

//        if (debuglevel > 2)
//        {
//            foreach (var t in TransformsToLog)
//            {
//                Debug.Log(t.name);
//            }
//        }


//        //The dictionary is accessed by sending it a transformID. 
//        //it returns slot, which contains that transform's information, as well as a list of its recorded history "content"
//        TransformHistories = new Dictionary<int, NamedTransformList>();
//        foreach (var t in TransformsToLog)
//        {
//            NamedTransformList slot = new NamedTransformList();
//            slot.instanceId = t.GetInstanceID();
//            slot.name = t.gameObject.name;
//            slot.content = new List<TransformFrameEvent>();
//            TransformHistories.Add(slot.instanceId, slot);

//            //THE FIRST OBJECT in our transform history list will store the 'running value' to test duplicates against. REMOVE when done with session
//            TransformHistories[t.GetInstanceID()].content.Add(new TransformFrameEvent());
//        }

//    }
//    public void InitializeSessionIdentitity()
//    {
//        currentSession = new SessionIdentity();
//        currentSession.deviceID = SystemInfo.deviceUniqueIdentifier;
//        currentSession.date = System.DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
//        currentSession.unitySessionStartTime = Time.time; //"physics" time in game, doesn't count pauses. timedelta is 
//        currentSession.platform = Application.platform.ToString();
//        currentSession.version = Application.version;
//        //currentSession.gameLevel = GameManager.Instance.gamelevel;//if we do more than one level this will be useful
//        //currentSession.difficulty = GameManager.Instance.playerDifficulty.ToString();
//        GameManager.Instance.PostSlidersAndEnums(); //get settings at start of round
//    }
//    public void InitializeSessionRecording()
//    {
//        if (isRecording)
//        {
//            EndRecording();
//        }
//        if (GameManager.Instance.trackConsent)
//        {
//            InitializeEventLog();
//            InitializeTransformLog();
//            InitializeSessionIdentitity();
//            InvokeRepeating("fetchPostTransforms", 0, pollingRate);
//            isRecording = true;
//        }
//    }

//    public void InitializeEventLog()
//    {
//        eventLog = new List<FrameEvent>();
//    }

//    public float getTimeStamp()
//    {
//        return Time.fixedTime - currentSession.unitySessionStartTime;
//    }

//    //record last frame for each. could change each transform into its own stream. a stream has a name. 
//    TransformFrameEvent lastFrame; //will hold frame data before adding to database
//    TransformFrameEvent currentFrame; //will hold frame data before adding to database
//    public void fetchPostTransforms()
//    {
//        if (isRecording)
//        {
//            foreach (Transform t in TransformsToLog)
//            {
//                //get transform id
//                int currentid = t.GetInstanceID();

//                //get last frame from id
//                lastFrame = new TransformFrameEvent();
//                lastFrame = TransformHistories[currentid].content[0];//gets the transform event with our CURRENT values

//                //create new frame
//                currentFrame = new TransformFrameEvent();


//                currentFrame.timestamp = getTimeStamp();//always get timestamp


//                //updateDeltaValue(t.name, lastFrame.transformName, currentFrame.transformName);
//                if (t.name != lastFrame.transformName)
//                {
//                    currentFrame.transformName = t.name;
//                    lastFrame.transformName = t.name;
//                }
//                //updateDeltaValue(t.localPosition, localPosition.localPosition, currentFrame.localPosition);
//                if (t.localPosition != lastFrame.globalPosition)
//                {
//                    currentFrame.localPosition = t.localPosition;
//                    lastFrame.localPosition = t.localPosition;
//                }

//                //updateDeltaValue(t.position, lastFrame.globalPosition, currentFrame.globalPosition);
//                if (t.position != lastFrame.globalPosition)
//                {
//                    currentFrame.globalPosition = t.position;
//                    lastFrame.globalPosition = t.position;
//                }
//                //updateDeltaValue(t.localScale, lastFrame.localScale, out lastFrame.localScale, out currentFrame.localScale);
//                if (t.localScale != lastFrame.localScale)
//                {
//                    currentFrame.localScale = t.localScale;
//                    lastFrame.localScale = t.localScale;
//                }
//                //updateDeltaValue(t.lossyScale, lastFrame.globalScale, currentFrame.globalScale);
//                if (t.lossyScale != lastFrame.globalScale)
//                {
//                    currentFrame.globalScale = t.lossyScale;
//                    lastFrame.globalScale = t.lossyScale;
//                }
//                //updateDeltaValue(t.localRotation, lastFrame.localRotation, currentFrame.localRotation);
//                if (t.localRotation != lastFrame.localRotation)
//                {
//                    currentFrame.localRotation = t.localRotation;
//                    lastFrame.localRotation = t.localRotation;
//                }
//                //updateDeltaValue(t.rotation, lastFrame.globalRotation, currentFrame.globalRotation);
//                if (t.rotation != lastFrame.globalRotation)
//                {
//                    currentFrame.globalRotation = t.rotation;
//                    lastFrame.globalRotation = t.rotation;
//                }


//                //add frame 
//                TransformHistories[currentid].content.Add(currentFrame);

//                //TransformHistories.Add(slot.instanceId, slot);
//                //TransformLog.Add(currentFrame);
//            }
//        }
//        //if (debuglevel > 2) { Debug.Log(TransformLog.Count); }
//    }

//    public void DeleteDataVis()
//    {
//        LineRenderer target = this.GetComponentInChildren<LineRenderer>();
//        if (target == null)
//        {
//            Debug.Log("NO LINERENDER SPECIFIED");
//        }
//        else
//        {
//            target.positionCount = 0;
//            if (eventNodes != null && eventNodes.Count > 0)
//            {
//                foreach (var g in eventNodes)
//                {
//                    Destroy(g);

//                }
//                eventNodes.Clear();
//            }
            
//            //target.SetPositions(sessionTrail);
//        }
//    }
//    public Vector3 stringToVec(string s)
//    {
//        string[] temp = s.Substring(1, s.Length - 2).Split(',');
//        return new Vector3(float.Parse(temp[0]), float.Parse(temp[1]), float.Parse(temp[2]));
//    }
//    public bool recordingTooLong = false;
//    public bool processRecording = true;
//    List<GameObject> eventNodes;
//    List<GameObject> deadNodes;
//    public IEnumerator RenderPlayerTrail(List<TransformFrameEvent> trail)
//    {
//        LineRenderer target = this.GetComponentInChildren<LineRenderer>();
//        //target.widthMultiplier = 0.0020f * GameManager.Instance.gameScale;
//        if (target == null)
//        {
//            Debug.Log("NO LINERENDER SPECIFIED");
//        }
//        else
//        {
//            Vector3[] sessionTrail = new Vector3[trail.Count];
//            Vector3 lastKnownValue = Vector3.zero;
//            for (int i = 0; i < trail.Count; i++)
//            {

//                //check nulls
//                if(trail[i].localPosition != null)
//                {
//                    lastKnownValue = trail[i].localPosition.Value;
//                }
//                sessionTrail[i] = new Vector3(lastKnownValue.x, 0, lastKnownValue.z);//flatten
//            }
//            target.positionCount = trail.Count;
//            target.SetPositions(sessionTrail);
//        }

//        eventNodes = new List<GameObject>();
//        deadNodes = new List<GameObject>();
//        foreach (FrameEvent e in eventLog)
//        {
//            if(e.announcement.Equals("CHEATED:"))
//            {
//                Debug.Log("cheated node");
//                //ModalWindow/ModalWindow_WinGame
//                GameObject node = Instantiate(Resources.Load("DataVisNodes/Node-X", typeof(GameObject))) as GameObject;
//                node.transform.parent = target.transform;
//                Vector3 destination = (Vector3)e.target;
//                node.transform.localPosition = new Vector3(destination.x, 0, destination.z);
//                //node.transform.localScale *= GameManager.Instance.gameScale;
//                eventNodes.Add(node);
                
//            }
//            else if (e.announcement.Equals("HIT HOLE:"))
//            {
//                Debug.Log("died node");
//                //ModalWindow/ModalWindow_WinGame
//                GameObject node = Instantiate(Resources.Load("DataVisNodes/Node-Death", typeof(GameObject))) as GameObject;
//                node.transform.parent = target.transform;
//                Vector3 destination = (Vector3)e.target;
//                node.transform.localPosition = new Vector3(destination.x, 0, destination.z);
//                //node.transform.localScale *= GameManager.Instance.gameScale;
//                eventNodes.Add(node);
//                if (deadNodes.Count > 0)
//                {
//                    foreach (var item in deadNodes)
//                    {
//                        Destroy(item);
//                    }
//                    deadNodes.Clear();
//                }
//                eventNodes.Add(node);
//                deadNodes.Add(node);
//            }
//            else if (e.announcement.Equals("WON:"))
//            {
//                //Debug.Log("won node");
//                //ModalWindow/ModalWindow_WinGame
//                GameObject node = Instantiate(Resources.Load("DataVisNodes/Node-Check", typeof(GameObject))) as GameObject;
//                node.transform.parent = target.transform;
//                Vector3 destination = (Vector3)e.eventParameter;
//                node.transform.localPosition = new Vector3(destination.x, 0, destination.z);
//                //node.transform.localScale *= GameManager.Instance.gameScale;
//                eventNodes.Add(node);
//            }
//        }


//        yield return null;
//    }
//    public void ShowSessionData()
//    {
//        if (GameManager.Instance.trackConsent)
//        {
//            //render trail
//            foreach (var item in TransformHistories)
//            {
//                if (item.Value.name.Equals("Player Ball"))
//                {
//                    //Debug.Log("found player, visualizing");
//                    StartCoroutine(RenderPlayerTrail(item.Value.content));
//                }

//            }
//        }
//    }

//    //maybe make this a coroutine
//    public void EndRecording()
//    {
//        CancelInvoke();//end all polling

//        //Debug.Log("end recording. " + processRecording + GameManager.Instance.trackConsent + isRecording);
//        //maybe we post all our end game data and post
//        if (processRecording && GameManager.Instance.trackConsent && isRecording)
//        {
//            isRecording = false;
//            foreach (var item in TransformHistories)
//            {
//                item.Value.content.RemoveAt(0);
//                if (debuglevel > 2)
//                {
//                    //transform report
//                    string report = "";
//                    report += "ID: " + item.Key + " ";
//                    report += "NAME: " + item.Value.name + "\r\n ";
//                    var movementList = item.Value.content;
//                    foreach (var frame in movementList)
//                    {
//                        report += frame.ToString() + "\r\n ";
//                    }
//                    print(report);
//                }
//            }//remove first array transforms
//            if (uploadRecording && !recordingTooLong)
//            {
//                StartCoroutine(uploadRecordingCoroutine());
//            }
//        }
//        isRecording = false; //just making sure
//    }

//    public IEnumerator uploadRecordingCoroutine()
//    {
//        JSONObject SessionReport = new JSONObject(JSONObject.Type.OBJECT);//final submission

//        JSONObject sessionData = new JSONObject(JSONObject.Type.OBJECT);
//        JSONObject sessionIdentity  = new JSONObject(JsonUtility.ToJson(currentSession));
//        sessionData.AddField("SessionIdentity", new JSONObject(JsonUtility.ToJson(currentSession)));

//        SessionReport.AddField("SessionIdentity", sessionData);

//        string dataString = ""; ;
//        if (debuglevel > 2)
//        {
//            dataString += sessionData.Print(true);//make aSYNC
//        }
//        //FRAME ONE
//        yield return null;

//        JSONObject eventHistory = new JSONObject(JSONObject.Type.OBJECT);
//        JSONObject eventArray = new JSONObject(JSONObject.Type.ARRAY);
//        //eventData.Add(loggedEvent);
//        //EVENTS
//        foreach (FrameEvent c in eventLog)
//        {
//            JSONObject frameEvent = new JSONObject(JSONObject.Type.OBJECT);
//            frameEvent.AddField("timestamp", c.timestamp);
//            frameEvent.AddField("announcement", c.announcement);
//            frameEvent.AddField("FromEntity", c.FromEntity?.ToString());

//            if (true)//we want more decimal places if the object sent here is a vector or quaternion
//            {
//                if (c.eventParameter?.GetType() == typeof(Vector3))
//                {
//                    Vector3 eventParameterVector = (Vector3)c.eventParameter;
//                    frameEvent.AddField("eventParameter", eventParameterVector.ToString("F4"));
//                }
//                else if (c.eventParameter?.GetType() == typeof(Quaternion))
//                {
//                    Quaternion eventParameterVector = (Quaternion)c.eventParameter;
//                    frameEvent.AddField("eventParameter", eventParameterVector.ToString("F4"));
//                }
//                else
//                    frameEvent.AddField("eventParameter", c.eventParameter?.ToString());
//            }

//            if (true)
//            {
//                if (c.target?.GetType() == typeof(Vector3))
//                {
//                    Vector3 eventParameterVector = (Vector3)c.eventParameter;
//                    frameEvent.AddField("target", eventParameterVector.ToString("F4"));
//                }
//                else if (c.target?.GetType() == typeof(Quaternion))
//                {
//                    Quaternion eventParameterVector = (Quaternion)c.eventParameter;
//                    frameEvent.AddField("target", eventParameterVector.ToString("F4"));
//                }
//                else
//                    frameEvent.AddField("target", c.target?.ToString());
//            }

//            eventArray.Add(frameEvent);
            
//        }
//        //eventHistory.Add(eventArray);
//        //eventData.AddField("EventData", loggedEvent);
//        SessionReport.AddField("EventDataArray", eventArray);
//        if (debuglevel > 2)
//        {
//            dataString += eventHistory.Print(true);
//        }
//        yield return null;

//        JSONObject TransformHistory = new JSONObject(JSONObject.Type.OBJECT);
//        JSONObject TransformArray = new JSONObject(JSONObject.Type.ARRAY);
//        //TransformHistory.Add(TransformArray);
//        //TRANSFORMS
//        foreach (var item in TransformHistories)
//        {

//            JSONObject Transform = new JSONObject(JSONObject.Type.OBJECT);
//            Transform.AddField("instanceId", item.Value.instanceId);
//            Transform.AddField("name", item.Value.name);

//            JSONObject TransformDataListObject = new JSONObject(JSONObject.Type.OBJECT);
//            JSONObject TransformDataList = new JSONObject(JSONObject.Type.ARRAY);
//            foreach (var transformFrameEvent in item.Value.content)
//            {
//                JSONObject timestep = new JSONObject(JSONObject.Type.OBJECT);
//                timestep.AddField("timestamp", transformFrameEvent.timestamp);
//                if (transformFrameEvent.transformName != null)
//                {
//                    timestep.AddField("transformName", transformFrameEvent.transformName);
//                }
//                if (transformFrameEvent.globalPosition != null)
//                {
//                    timestep.AddField("globalPosition", JSONTemplates.FromVector3(transformFrameEvent.globalPosition.Value));
//                }
//                if (transformFrameEvent.localPosition != null)
//                {
//                    timestep.AddField("localPosition", JSONTemplates.FromVector3(transformFrameEvent.localPosition.Value));
//                }
//                if (transformFrameEvent.globalRotation != null)
//                {
//                    timestep.AddField("globalRotation", JSONTemplates.FromQuaternion(transformFrameEvent.globalRotation.Value));
//                }
//                if (transformFrameEvent.localRotation != null)
//                {
//                    timestep.AddField("localRotation", JSONTemplates.FromQuaternion(transformFrameEvent.localRotation.Value));
//                }
//                if (transformFrameEvent.globalScale != null)
//                {
//                    timestep.AddField("globalScale", JSONTemplates.FromVector3(transformFrameEvent.globalScale.Value));
//                }
//                if (transformFrameEvent.localScale != null)
//                {
//                    timestep.AddField("localScale", JSONTemplates.FromVector3(transformFrameEvent.localScale.Value));
//                }
//                //TransformDataListObject("TransformFrame", timestep);
//                //TransformDataList.Add(TransformDataListObject);
//                TransformDataList.Add(timestep);
                
//            }
//            //TransformDataListObject.Add(TransformDataList);
//            Transform.AddField("TransformDataList", TransformDataList);
//            TransformArray.Add(Transform);
            
//        }
//        TransformHistory.Add(TransformArray);
//        SessionReport.AddField("TrackedTransformArray",TransformHistory);

//        if (debuglevel > 2)
//        {
//            dataString += TransformHistory.Print(true);
//            Debug.Log(dataString);
//        }

//        SubmitData(SessionReport);
        
//        yield return null;
//    }

//    private string MinotaurClowderSpaceId = "5e929d0d4f0cebe5ffa9c73f";
//    void SubmitData(JSONObject report)
//    {
//        string url = ClowderBridge.Instance.CLOWDERURL + "spaces/" + MinotaurClowderSpaceId + "/datasets?limit=10000&key=" + ClowderBridge.Instance.API_KEY;
//        //Debug.Log(url);
//        StartCoroutine(ClowderBridge.Instance.GetRequest(url, returnValue =>
//        {
//            bool foundProposal = false;
//            foreach (JSONObject c in returnValue.list)
//            {
//                string JsonDeviceId;
//                JsonDeviceId = c.GetField("description").str;
//                //if we have a dataset with our device ID
//                if (JsonDeviceId == currentSession.deviceID)
//                {
//                    foundProposal = true;
//                    Debug.Log("Found Unity DeviceID Dataset. Submitting...");
//                    //post to c.GetField("id").str


//                    JSONObject ClowderPost = ClowderHelper.CreateMetadataPost(report);
//                    //Debug.Log(ClowderPost.Print(true));
//                    ClowderBridge.Instance.PostMetaData(c.GetField("id").str, ClowderBridge.ClowderType.dataset, ClowderPost);
//                    StartCoroutine(ClowderBridge.Instance.PostJSONasFileRequest(ClowderPost, "SessionLog_" + currentSession.date,c.GetField("id").str));

//                }
//            }//datasets/{id}/metadata.jsonld
//            if (!foundProposal)
//            {
//                Debug.Log("No Unity DeviceID Dataset Detected, creating....");
//                StartCoroutine(ClowderBridge.Instance.CreateDataset(currentSession.deviceID, currentSession.deviceID, MinotaurClowderSpaceId, returnValue2 =>
//                {
//                    JSONObject ClowderPost = ClowderHelper.CreateMetadataPost(report);
//                    ClowderBridge.Instance.PostMetaData(returnValue2, ClowderBridge.ClowderType.dataset, ClowderPost);
//                    StartCoroutine(ClowderBridge.Instance.PostJSONasFileRequest(ClowderPost,   "SessionLog_" + currentSession.date, returnValue2));//currentSession.deviceID
//                }));
//            }

//        }));

//    }

//}