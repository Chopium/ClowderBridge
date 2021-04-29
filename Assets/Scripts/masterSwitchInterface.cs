using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using DG.Tweening;
[System.Serializable]
public class masterSwitch : wikarBase
{
    public bool freeToPost = false;
    public bool loadOrnaments = false;
    public bool loadComments = false;
    public bool downloadCopy = false;
    public float currentScale = 10f;
    //public int currentDate;
    public int forceDayNightMode = 0; // 0 none 1 night 2 day
    public string LaunchNote;
}



public class masterSwitchInterface : wikarManager<masterSwitch>
{

    #region Singleton / Start / Awake
    private static masterSwitchInterface instance;
    public static masterSwitchInterface Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject bridge = new GameObject("masterSwitchInterface");
                instance = bridge.AddComponent<masterSwitchInterface>();
            }
            return instance;
        }
    }
    #endregion

    private string _unitySessionID = null;
    public createOrnamentManager createOrnamentButton;
    public masterSwitch currentSettings;
    // Start is called before the first frame update
    [ExecuteAlways]
    public void Awake ()
    {
        instance = this;
        Collection = new List<masterSwitch>();
        if (!Application.isEditor)
        {
            user = userType.client;
        }
        

    }
    public string getUnityID()
    {
        if (_unitySessionID != null)
        {
            return _unitySessionID;
        }
        else if (user == userType.admin)
        {
            _unitySessionID =  UnityEngine.Random.Range(0, 99999).ToString();
            return _unitySessionID;
        }
        else
        {
            _unitySessionID = SystemInfo.deviceUniqueIdentifier;
            return _unitySessionID;
        }
    }
    


    public IEnumerator Start ()
    {
        fetchSettings();//update our pool of objects with our type and call the method after to update gameobjects
        //wait 10 seconds to load content otherwise search local
        yield return new WaitForSeconds(1);
        if (!loadSuccess)
        {
            currentSettings.freeToPost = false;
            currentSettings.loadOrnaments = false;
            currentSettings.downloadCopy = false;
            currentSettings.loadComments = false;
            currentSettings.currentScale = 10f;//default scale unless saved in preferences

            //try load local JSON
            if (Ornament_Master.Instance.tryLoadLocalJSON())
            {
                Ornament_Master.Instance.groupSizeFactor = PlayerPrefs.GetFloat("groupSize");
                Ornament_Master.Instance.updateOrnamentObjects();
            }
            if (guestBook.Instance.tryLoadLocalJSON())
            {
                guestBook.Instance.updateMessageLog();
                //Ornament_Master.Instance.groupSizeFactor = PlayerPrefs.GetFloat("groupSize");
                //Ornament_Master.Instance.updateOrnamentObjects();
            }

        }
        

    }
    public void updateSettings()
    {
        if (user == wikarManager<masterSwitch>.userType.admin)
        {
            ClowderBridge.Instance.UpdateMetaData( trackedDataset, ClowderBridge.ClowderType.dataset, createObjectJson(currentSettings)); //"ncsa.unity." + array[i + 1]
            
        }
        else
        {
            Debug.LogError("NOT ADMIN");
        }
    }
    public void postSettings()
    {
        if (user == wikarManager<masterSwitch>.userType.admin)
        {
            currentSettings.unityID = SystemInfo.deviceUniqueIdentifier;
            PostMetaData(trackedDataset, createObjectJson(currentSettings));
        }
        else
        {
            Debug.LogError("NOT ADMIN");
        }
    }
    private bool loadSuccess = false;
    public void fetchSettings()
    {
        fetchObjects(delegate
        {
            loadSuccess = true;
            applySettings();
        });
        //update our pool of objects with our type and call the method after to update gameobjects
    }
    public void deleteAllSettings()
    {
        if (user == wikarManager<masterSwitch>.userType.admin)
        {
            ClowderBridge.Instance.DeleteMetaData(trackedDataset, "ncsa.unity", ClowderBridge.ClowderType.dataset);
        }
        else
        {
            Debug.LogError("NOT ADMIN");
        }
    }
    public void applyCurrentSettings()
    {

    }
    public void applySettings()
    {
        if (Collection == null)
        {
            Debug.LogError("UPDATE FAILED OR NO METADATA TO LOAD");

        }
        else
        {
            if (Collection.Count > 1)
            {
                Debug.LogError("MASTER SWITCH HAS MORE THAN ONE SETTING POSTED. APPLYING FIRST FOUND");
            }
            if (Collection[0] != null)
            {
                Debug.Log("Loaded Settings");
                currentSettings = Collection[0];

                //enableDisablePost
                var tween = DOTween.Sequence().AppendInterval(Camera_Manager.Instance.introDuration).AppendCallback(delegate
                {
                    //enaable after intro
                    if (currentSettings.freeToPost)
                    {
                        createOrnamentButton.tryEnableButton = true;
                    }
                    else
                    {
                        createOrnamentButton.tryEnableButton = false;
                    }
                    

                });

                #if !UNITY_WEBGL
                Ornament_Master.Instance.saveLocalCopy = currentSettings.downloadCopy;
                guestBook.Instance.saveLocalCopy = currentSettings.downloadCopy;
                #endif
                //don't download copy in webgl

                PlayerPrefs.SetFloat("groupSize", Ornament_Master.Instance.groupSizeFactor = currentSettings.currentScale);
                Ornament_Master.Instance.groupSizeFactor = currentSettings.currentScale;

                if (currentSettings.loadOrnaments)
                {
                    Ornament_Master.Instance.fetchOrnaments();
                }
                else
                {
                    if (Ornament_Master.Instance.tryLoadLocalJSON())
                    {
                        Ornament_Master.Instance.updateOrnamentObjects();
                    }
                }
                if (currentSettings.loadComments)
                {
                    guestBook.Instance.fetchMessages();
                }
                else
                {
                    if (guestBook.Instance.tryLoadLocalJSON())
                    {
                        guestBook.Instance.updateMessageLog();
                    }
                }
                if (currentSettings.forceDayNightMode != 0)
                {
                    if (currentSettings.forceDayNightMode == 1)
                    {
                        nightSwap.Instance.nightMode = (true);
                    }
                    else if (currentSettings.forceDayNightMode == 2)
                    {
                        nightSwap.Instance.nightMode = (false);
                    }
                }
            }
        }
    }
}