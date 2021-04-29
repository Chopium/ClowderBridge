using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModalWindowLauncher : MonoBehaviour
{
    #region Singleton / Start / Awake
    private static ModalWindowLauncher instance;
    public static ModalWindowLauncher Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new ModalWindowLauncher();
            }
            return instance;
        }
    }

    void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        modalWindows = new List<ModalWindow_Logic>();
    }

    #endregion

    //will store active modal windows
    public List<ModalWindow_Logic> modalWindows;

    //x will always only close itself
    
    //closeSelf
    public void CloseSelfModal(ModalWindow_Logic m)
    {
        modalWindows.Remove(m);
        Destroy(m.gameObject);
    }
    //closeAll
    public void CloseAllModals()
    {
        foreach ( ModalWindow_Logic m in modalWindows)
        {
            Destroy(m.gameObject);
        }
        modalWindows.Clear();
    }
    
    public IEnumerator LaunchModalWindow(string topic = "Make Selection:", string modalPrefabLocation = "ModalWindow/ModalWindow", JSONObject contents = null, System.Action<ModalWindow_Logic> callback = null)
    {
        //Debug.Log("LAUNCHED");
        GameObject window;

        window = Instantiate(Resources.Load<GameObject>(modalPrefabLocation) as GameObject, this.gameObject.GetComponentInChildren<Canvas>().transform);
        ModalWindow_Logic modalLogic = window.GetComponent<ModalWindow_Logic>();
        modalWindows.Add(modalLogic);
        //topic is string at top of modal window
        modalLogic.topic.SetText(topic);
        //contents is a linked list of names for buttons and then links
        //delgate action is the method to hand the link in content to execute
        callback(modalLogic);
        yield return null;
    }
    

    public void LaunchResolutionBrowse(Button senderButton = null)
    {
        Resolution[] resolutions = Screen.resolutions;
        //Debug.Log("HERE");
        
            if (senderButton != null) { senderButton.interactable = false; }
            //reformat result, create json
            //Debug.Log("HERE");
            JSONObject buttonList = new JSONObject(JSONObject.Type.ARRAY);

            foreach (Resolution c in resolutions)
            {

                JSONObject item = new JSONObject();
                item.AddField("name", c.ToString());
                item.AddField("width", c.width);
                item.AddField("height", c.height);
                buttonList.Add(item);
            }
            
            StartCoroutine(LaunchModalWindow("Select Resolution", "ModalWindow/ModalWindow", buttonList, modalWindow => {
                //result2 is highest level script inside the launched modal window
                //iterate through JSON and instantiate buttons, create listeners for next modal window
                foreach (JSONObject c in buttonList.list)
                {
                    GameObject button = Instantiate(Resources.Load<GameObject>("ModalWindow/Button") as GameObject, modalWindow.gameObject.GetComponentInChildren<FlexibleGrid>().transform);
                    button.name = c.GetField("name").str;
                    modalWindow.GetComponentInChildren<FlexibleGrid>().columns = 3;
                    //button.GetComponentInChildren<GetName>().setName(button.name);
                    //tell our instantiated button what it does when clicked
                    var buttonTarget = button.GetComponent<Button>();
                    buttonTarget.onClick.AddListener(delegate {
                        Debug.Log("SETTING RESOLUTOIN: " + c.GetField("width").i + " x " + c.GetField("height").i);

                        Screen.SetResolution((int)c.GetField("width").i, (int)c.GetField("height").i, Screen.fullScreen);
                        CloseAllModals();
                    });
                }
            }));
            if (senderButton != null) { senderButton.interactable = true; }
    }
    public void LaunchQualityLevelBrowse(Button senderButton = null)
    {
        string[] qualityLevels = QualitySettings.names;


        if (senderButton != null) { senderButton.interactable = false; }
        //reformat result, create json
        //Debug.Log("HERE");
        JSONObject buttonList = new JSONObject(JSONObject.Type.ARRAY);
        for (int i = 0; i < qualityLevels.Length; i++)
        {
            JSONObject item = new JSONObject();
            item.AddField("name", qualityLevels[i].ToString());
            item.AddField("index", i);
            buttonList.Add(item);
        }

        StartCoroutine(LaunchModalWindow("Select Quality Level", "ModalWindow/ModalWindow", buttonList, modalWindow => {
            //result2 is highest level script inside the launched modal window
            //iterate through JSON and instantiate buttons, create listeners for next modal window
            foreach (JSONObject c in buttonList.list)
            {
                GameObject button = Instantiate(Resources.Load<GameObject>("ModalWindow/Button") as GameObject, modalWindow.gameObject.GetComponentInChildren<FlexibleGrid>().transform);
                button.name = c.GetField("name").str;
                modalWindow.GetComponentInChildren<FlexibleGrid>().columns = 1;
                //button.GetComponentInChildren<GetName>().setName(button.name);
                //tell our instantiated button what it does when clicked
                var buttonTarget = button.GetComponent<Button>();
                buttonTarget.onClick.AddListener(delegate {

                    if (QualitySettings.GetQualityLevel() != c.GetField("index").i)
                    {
                        QualitySettings.SetQualityLevel((int)c.GetField("index").i);

                    }
                    
                    CloseAllModals();
                });
            }
        }));
        if (senderButton != null) { senderButton.interactable = true; }
    }
    //public IEnumerator LaunchEndgameModal(Button senderButton = null, ResetTrigger ResetSender = null, System.Action<ModalWindow_Logic> callback = null)
    //{
    //    string[] qualityLevels = QualitySettings.names;


    //    if (senderButton != null) { senderButton.interactable = false; }
    //    //reformat result, create json
    //    //Debug.Log("HERE");
    //    JSONObject buttonList = new JSONObject(JSONObject.Type.ARRAY);
    //    for (int i = 0; i < qualityLevels.Length; i++)
    //    {
    //        JSONObject item = new JSONObject();
    //        item.AddField("name", qualityLevels[i].ToString());
    //        item.AddField("index", i);
    //        buttonList.Add(item);
    //    }
    //    yield return null;
    //    StartCoroutine(LaunchModalWindow("Session Results", "ModalWindow/ModalWindow_WinGame", buttonList, modalWindow => {
    //        if(ResetSender != null)
    //        {
    //            PopulatePlayerStats statmachine = modalWindow.gameObject.GetComponent<PopulatePlayerStats>();
    //            statmachine.Exit.onClick.AddListener(delegate {

    //                Game_Recorder.Instance.DeleteDataVis();
    //                StartCoroutine(ResetSender.RecoverPart2());
    //                GameManager.Instance.locked -= 1;
    //                //if (GameManager.Instance.didGetGoodEnding())
    //                //{
    //                //    GameManager.Instance.doWin();
    //                //}
    //                //else
    //                //{
    //                //    StartCoroutine(ResetSender.RecoverPart2());
    //                //}

    //                CloseAllModals();
    //            });

    //        }
            
    //        //result2 is highest level script inside the launched modal window
    //        //iterate through JSON and instantiate buttons, create listeners for next modal window
    //    //foreach (JSONObject c in buttonList.list)
    //    //    {
    //    //        GameObject button = Instantiate(Resources.Load<GameObject>("ModalWindow/Button") as GameObject, modalWindow.gameObject.GetComponentInChildren<FlexibleGrid>().transform);
    //    //        button.name = c.GetField("name").str;
    //    //        modalWindow.GetComponentInChildren<FlexibleGrid>().columns = 1;
    //    //        //button.GetComponentInChildren<GetName>().setName(button.name);
    //    //        //tell our instantiated button what it does when clicked
    //    //        var buttonTarget = button.GetComponent<Button>();
    //    //        buttonTarget.onClick.AddListener(delegate {

    //    //            if (QualitySettings.GetQualityLevel() != c.GetField("index").i)
    //    //            {
    //    //                QualitySettings.SetQualityLevel((int)c.GetField("index").i);

    //    //            }
    //    //            callback(modalWindow);
    //    //            CloseAllModals();
    //    //        });
    //    //    }
    //    }));
    //    if (senderButton != null) { senderButton.interactable = true; }
    //}
}
