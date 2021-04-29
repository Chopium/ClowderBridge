using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FloatTracker : MonoBehaviour
{
    public TMP_InputField input;
    public TMP_Text readout;
    private float submittedFloat;
    private float totalFloat;
    public string trackedDataset = "5f7267f04f0cbe0fe2297e1e";
    public bool isFetching = false;//lockout from fetching more than once at a time

    public float autoUpdateRate = 30;
    // Start is called before the first frame update
    void Start()
    {
        if (autoUpdateRate > 0)
        {
            InvokeRepeating("fetchFloat", 0, autoUpdateRate);
        }
    }


    public void submitFloat()
    {
        float.TryParse(input.text, out submittedFloat);//try cast

        //we can fill this json with any field based data and the report should be valid
        JSONObject submission = new JSONObject();
        submission.AddField("trackedFloat",submittedFloat);

        
        JSONObject report = ClowderHelper.CreateMetadataPost(submission);
        ClowderBridge.Instance.PostMetaData(trackedDataset, ClowderBridge.ClowderType.dataset, report);

        //we COULD start a fetch request on submit, 
        //but i think we can just add the players submission locally 
        //and then let our incremental fetch update take care of it.
        totalFloat += submittedFloat;
        readout.text = totalFloat.ToString("F2");
        input.text = "";
    }
    public void fetchFloat()
    {
        if (!isFetching)
        {
            isFetching = true;
            StartCoroutine(ClowderBridge.Instance.GetExtractorMetaData(trackedDataset, "ncsa.unity", ClowderBridge.ClowderType.dataset, result =>
            {
                float tally = 0;
                JSONObject MetaJson = new JSONObject();
                foreach (JSONObject j in result.list)
                {
                    if (j["content"].GetField("trackedFloat"))
                    {
                        tally += (j["content"].GetField("trackedFloat")).f;
                    }
                }
                Debug.Log(tally);
                totalFloat = tally;
                readout.text = totalFloat.ToString("F2");
                isFetching = false;
            }));
        }
    }
}
