using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JsonRef : MonoBehaviour
{
    public TextAsset StaticJson;

    [HideInInspector]
    public JSONObject JsonOutput;

    public ClowderBridge.ClowderType clowderType = ClowderBridge.ClowderType.unassigned;

    [HideInInspector]
    public JSONObject BlobJson;

    [HideInInspector]
    //public List<JSONObject> MetaJson;
    public JSONObject MetaJson;


    [SerializeField]
    private string clowderID = null;

    private void Awake()
    {

        //if (StaticJson != null)
        //{
        //    setJson(StaticJson.ToString);
        //}
    }
    wikar_comment_container[] comments;
    public JSONObject SerializeFields()
    {
        JSONObject report = new JSONObject();

        R3D_Transform target = this.GetOrAddComponent<R3D_Transform>();//this one we'll always want
        //R3D_Animation animation = this.GetComponent<R3D_Animation>();//we wanna get here then test if false, otherwise we put every component on everything
        //R3D_Photo photo = this.GetComponent<R3D_Photo>();
       // R3D_FadeModel modelFade = this.GetComponent<R3D_FadeModel>();
       // R3D_ExitTotemOverride exitTotem = this.GetComponent<R3D_ExitTotemOverride>();

        comments = this.GetComponentsInChildren<wikar_comment_container>();

        if (target != null)
        {
            report.AddField(target.GetType().ToString(), target.MakeJson());
        }
        //if (animation != null)
        //{
        //    report.AddField(animation.GetType().ToString(), animation.MakeJson());
        //}
        //if (photo != null)
        //{
        //    report.AddField(photo.GetType().ToString(), photo.MakeJson());
        //}
        //if (modelFade != null)
        //{
        //    report.AddField(modelFade.GetType().ToString(), modelFade.MakeJson());
        //}
        //if (exitTotem != null)
        //{
        //    //report.AddField(exitTotem.GetType().ToString(), exitTotem.MakeJson());
        //}
        if (comments != null)
        {
            JSONObject commentsReport = new JSONObject(JSONObject.Type.ARRAY);

            foreach (wikar_comment_container c in comments)
            {
                commentsReport.Add(c.MakeJson());
            }
            report.AddField("wikar_comment_container", commentsReport);
        }

        report = ClowderHelper.CreateMetadataPost(report);

        //Debug.Log(accessData(report));
        Debug.Log(report.ToString(true));
        JsonOutput = report;

        //Debug.Log(jsonOutput);
        return report;
    }

    public void fetchTechnicalMetaData()
    {
        StartCoroutine(ClowderBridge.Instance.GetExtractorMetaData(clowderID, "ncsa.unity", clowderType, result =>
        {
            MetaJson = new JSONObject();
            foreach (JSONObject j in result.list)
            {
                MetaJson.Add(j["content"]);
                //Debug.Log(j["content"]);
                if (j["content"].GetField("R3D_Transform"))
                {
                    this.GetOrAddComponent<R3D_Transform>().ApplyJson(j["content"].GetField("R3D_Transform"));
                }             
                if (j["content"].GetField("wikar_comment_container"))
                {
                    var commentsList = new List<wikar_comment_container>();
                    JSONObject comments = j["content"].GetField("wikar_comment_container");
                    foreach (JSONObject c in comments.list)
                    {
                        var comment = Instantiate(Resources.Load("wikar_CommentNode")) as GameObject;
                        comment.transform.parent = this.transform;
                        comment.GetComponent<wikar_comment_container>().ApplyJson(c);
                       
                    }
                }
                //print("GET RESULT: " + j[0]["content"]);
            }

        }));

    }

    public void setJsonFromAsset()
    {
        BlobJson = new JSONObject(StaticJson.text);
        clowderID = BlobJson.GetField("id").str;
    }

    public void setJson(JSONObject obj)
    {
        if (StaticJson != null)
        {
            //BlobJson.Clear();
            BlobJson = new JSONObject(StaticJson.text);
            clowderID = BlobJson.GetField("id").str;
            this.gameObject.name = BlobJson.GetField("filename").str;
        }
        else if (obj != null) //if we were handed json in this command
        {
            //BlobJson.Clear();
            BlobJson = obj;
            clowderID = BlobJson.GetField("id").str;
            if (BlobJson.GetField("filename") != null)
            {
                this.gameObject.name = BlobJson.GetField("filename").str;
            }
            if (BlobJson.GetField("name") != null)
            {
                this.gameObject.name = BlobJson.GetField("name").str + " (dataset)";
            }

            fetchTechnicalMetaData();
        }

        //accessData(BlobJson);
    }



    public string accessData(JSONObject obj)
    {
        string report = "";
        switch (obj.type)
        {
            case JSONObject.Type.OBJECT:
                for (int i = 0; i < obj.list.Count; i++)
                {
                    string key = (string)obj.keys[i];
                    JSONObject j = (JSONObject)obj.list[i];
                    //Debug.Log(key);
                    report += "\n" + key + ": ";
                    report += accessData(j);
                    //accessData(j);
                }
                break;
            case JSONObject.Type.ARRAY:
                foreach (JSONObject j in obj.list)
                {
                    report += accessData(j);
                }
                break;
            case JSONObject.Type.STRING:
                //Debug.Log(obj.str);
                report += obj.str.ToString();
                break;
            case JSONObject.Type.NUMBER:
                //Debug.Log(obj.n);
                report += obj.n.ToString();
                break;
            case JSONObject.Type.BOOL:
                //Debug.Log(obj.b);
                report += obj.b.ToString();
                break;
            case JSONObject.Type.NULL:
                //Debug.Log("NULL");
                report += "NULL";
                break;

        }
        //Debug.Log(report);

        return report;
    }

    public void postMetaData(JSONObject obj)
    {
        if (clowderID == null)
        {
            Debug.LogError("jsonref tried to post json without knowing a file id / target");
        }
        Debug.Log("posting @: " + clowderID + " : " + obj.ToString(false));
        ClowderBridge.Instance.UpdateMetaData(clowderID, clowderType, obj);
    }
}