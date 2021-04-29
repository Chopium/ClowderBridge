using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wikar_comment_container : R3D_MetadataBase
{
    public string commentID = ""; //hash representing comment
    public string context = "";
    public string title = "";
    public string linkedFileID = "";
    public Vector2 GPS = Vector2.zero;
    public double lat = 0f;
    public double lng = 0f;

    [System.Serializable]
    public struct comment
    {
        public string text;
        public string datetime;
        public string author;
    }
    public List<comment> commentsList;


    override public JSONObject MakeJson()
    {
        JSONObject report = new JSONObject();

        JSONObject Position = JSONTemplates.FromVector3(this.transform.localPosition);
        JSONObject Rotation = JSONTemplates.FromQuaternion(this.transform.localRotation);
        JSONObject Scale = JSONTemplates.FromVector3(this.transform.localScale);

        JSONObject Content = new JSONObject();
        JSONObject comments = new JSONObject(JSONObject.Type.ARRAY);
        Content.AddField("comments", comments);
        Content.AddField("commentID", commentID);
        Content.AddField("title", title);
        Content.AddField("linkedFileID", linkedFileID);
        foreach (comment c in commentsList)
        {
            JSONObject comment = new JSONObject();
            comment.AddField("text", c.text);
            comment.AddField("datetime", c.datetime);
            comment.AddField("author", c.author);
            comments.Add(comment);
        }
        //Content.AddField("comments",comments);
        //Content.AddField("author", author);
        Content.AddField("context", context);
        //JSONObject GPS = new JSONObject(JSONObject.Type.NUMBER);
        Content.AddField("lat", lat.ToString());
        Content.AddField("lng", lng.ToString());

        report.AddField("Position", Position);
        report.AddField("Rotation", Rotation);
        report.AddField("Scale", Scale);
        report.AddField("Content", Content);
        //this.GetComponent<JsonRef>().accessData(report);
        return report;
    }
    private void Awake()
    {
        commentsList = new List<comment>();
        //this.transform.localPosition = Vector3.zero;
        //this.transform.localRotation = Quaternion.identity;
    }

    override public void ApplyJson(JSONObject master)
    {
        //JSONObject Position = master.GetField("Position");
        //Position.type = JSONObject.Type.OBJECT;
        //Debug.Log(JSONTemplates.ToVector3(master["Position"]));
        //this.transform.localPosition = new Vector3(Position.GetField("X").f, Position.GetField("Y").f, Position.GetField("Z").f);
        if (JSONTemplates.ToVector3(master["Position"]) != null) this.transform.localPosition = JSONTemplates.ToVector3(master["Position"]);
        //Debug.Log(Position.GetField("X")+" "+ Position["y"] + Position["z"]);
        //Debug.Log(JSONTemplates.ToVector3(Position));

        //JSONObject Rotation = master.GetField("Rotation");
        if (JSONTemplates.ToVector3(master["Rotation"]) != null) this.transform.localRotation = JSONTemplates.ToQuaternion(master["Rotation"]);
        //JSONObject Scale = master.GetField("Scale");
        //this.transform.localScale = JSONTemplates.ToVector3(master["Scale"]);
        if (!JSONTemplates.ToVector3(master["Scale"]).Equals(Vector3.zero))
        {
            this.transform.localScale = JSONTemplates.ToVector3(master["Scale"]);
        }

        JSONObject Content = master.GetField("Content");
        //comment = Content.GetField("comment").str;
        //author = Content.GetField("author").str;
        context = Content.GetField("context")?.str;
        title = Content.GetField("title")?.str;
        commentID = Content.GetField("commentID")?.str;
        linkedFileID = Content.GetField("linkedFileID")?.str;
        //lat = Content.GetField("lat").f;
        double.TryParse(Content.GetField("lat")?.str, out lat);
        double.TryParse(Content.GetField("lng")?.str, out lng);
        //float.TryParse(Content.GetField("lat").f, out this.lat);
        //float.TryParse(Content.GetField("lng").f, out this.lng);
        commentsList = new List<comment>();
        JSONObject comments = Content.GetField("comments");
        foreach (JSONObject c in comments.list)
        {
            comment loadedComment;
            loadedComment.text = c.GetField("text")?.str;
            loadedComment.datetime = c.GetField("datetime")?.str;
            loadedComment.author = c.GetField("author")?.str;
            commentsList.Add(loadedComment);
            //JSONObject commentsArray = comments.GetField("comment");k
            //Debug.Log(commentsArray);
        }
    }

    static public Transform JSONtoTransform(JSONObject json)
    {
        GameObject g = new GameObject();
        JSONObject Position = json.GetField("Position");
        g.transform.localPosition = new Vector3(Position.GetField("X").f, Position.GetField("Y").f, Position.GetField("Z").f);

        JSONObject Rotation = json.GetField("Rotation");
        g.transform.localRotation = new Quaternion(Rotation.GetField("X").f, Rotation.GetField("Y").f, Rotation.GetField("Z").f, Rotation.GetField("W").f);

        //JSONObject Scale = json.GetField("Scale");
        //g.transform.localScale = new Vector3(Scale.GetField("X").f, Scale.GetField("Y").f, Scale.GetField("Z").f);
        if (!JSONTemplates.ToVector3(json["Scale"]).Equals(Vector3.zero))
        {
            g.transform.localScale = JSONTemplates.ToVector3(json["Scale"]);
        }

        return g.transform;
    }
}


//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class wikar_comment_container : R3D_MetadataBase
//{
//    public string commentID = ""; //hash representing comment
//    public string context = "";
//    public string title = "";
//    public string linkedFileID = "";
//    public Vector2 GPS = Vector2.zero;
//    public double lat = 0f;
//    public double lng = 0f;

//    [System.Serializable]
//    public struct comment
//    {
//        public string text;
//        public string datetime;
//        public string author;
//    }
//    public List<comment> commentsList;


//    override public JSONObject MakeJson()
//    {
//        JSONObject report = new JSONObject();

//        JSONObject Position = JSONTemplates.FromVector3(this.transform.localPosition);
//        JSONObject Rotation = JSONTemplates.FromQuaternion(this.transform.localRotation);
//        JSONObject Scale = JSONTemplates.FromVector3(this.transform.localScale);

//        JSONObject Content = new JSONObject();
//        JSONObject comments = new JSONObject(JSONObject.Type.ARRAY);
//        Content.AddField("comments", comments);
//        Content.AddField("commentID", commentID);
//        Content.AddField("title", title);
//        Content.AddField("linkedFileID", linkedFileID);
//        foreach (comment c in commentsList)
//        {
//            JSONObject comment = new JSONObject();
//            comment.AddField("text", c.text);
//            comment.AddField("datetime", c.datetime);
//            comment.AddField("author", c.author);
//            comments.Add(comment);
//        }
//        //Content.AddField("comments",comments);
//        //Content.AddField("author", author);
//        Content.AddField("context", context);
//        //JSONObject GPS = new JSONObject(JSONObject.Type.NUMBER);
//        Content.AddField("lat", lat.ToString());
//        Content.AddField("lng", lng.ToString());

//        report.AddField("Position", Position);
//        report.AddField("Rotation", Rotation);
//        report.AddField("Scale", Scale);
//        report.AddField("Content", Content);
//        //this.GetComponent<JsonRef>().accessData(report);
//        return report;
//    }
//    private void Awake()
//    {
//        commentsList = new List<comment>();
//        //this.transform.localPosition = Vector3.zero;
//        //this.transform.localRotation = Quaternion.identity;
//    }

//    override public void ApplyJson(JSONObject master)
//    {
//        //JSONObject Position = master.GetField("Position");
//        //Position.type = JSONObject.Type.OBJECT;
//        //Debug.Log(JSONTemplates.ToVector3(master["Position"]));
//        //this.transform.localPosition = new Vector3(Position.GetField("X").f, Position.GetField("Y").f, Position.GetField("Z").f);
//        if(JSONTemplates.ToVector3(master["Position"]) != null) this.transform.localPosition = JSONTemplates.ToVector3(master["Position"]);
//        //Debug.Log(Position.GetField("X")+" "+ Position["y"] + Position["z"]);
//        //Debug.Log(JSONTemplates.ToVector3(Position));

//        //JSONObject Rotation = master.GetField("Rotation");
//        if (JSONTemplates.ToVector3(master["Rotation"]) != null) this.transform.localRotation = JSONTemplates.ToQuaternion(master["Rotation"]);
//        //JSONObject Scale = master.GetField("Scale");
//        //this.transform.localScale = JSONTemplates.ToVector3(master["Scale"]);
//        if (!JSONTemplates.ToVector3(master["Scale"]).Equals(Vector3.zero))
//        {
//            this.transform.localScale = JSONTemplates.ToVector3(master["Scale"]);
//        }

//        JSONObject Content = master.GetField("Content");
//        //comment = Content.GetField("comment").str;
//        //author = Content.GetField("author").str;
//        context = Content.GetField("context")?.str;
//        title = Content.GetField("title")?.str;
//        commentID = Content.GetField("commentID")?.str;
//        linkedFileID = Content.GetField("linkedFileID")?.str;
//        //lat = Content.GetField("lat").f;
//        double.TryParse(Content.GetField("lat")?.str, out lat);
//        double.TryParse(Content.GetField("lng")?.str, out lng);
//        //float.TryParse(Content.GetField("lat").f, out this.lat);
//        //float.TryParse(Content.GetField("lng").f, out this.lng);
//        commentsList = new List<comment>();
//        JSONObject comments = Content.GetField("comments");
//        foreach (JSONObject c in comments.list)
//        {
//            comment loadedComment;
//            loadedComment.text = c.GetField("text")?.str;
//            loadedComment.datetime = c.GetField("datetime")?.str;
//            loadedComment.author = c.GetField("author")?.str;
//            commentsList.Add(loadedComment);
//            //JSONObject commentsArray = comments.GetField("comment");k
//            //Debug.Log(commentsArray);
//        }
//    }

//    static public Transform JSONtoTransform(JSONObject json)
//    {
//        GameObject g = new GameObject();
//        JSONObject Position = json.GetField("Position");
//        g.transform.localPosition = new Vector3(Position.GetField("X").f, Position.GetField("Y").f, Position.GetField("Z").f);

//        JSONObject Rotation = json.GetField("Rotation");
//        g.transform.localRotation = new Quaternion(Rotation.GetField("X").f, Rotation.GetField("Y").f, Rotation.GetField("Z").f, Rotation.GetField("W").f);

//        if (!JSONTemplates.ToVector3(json["Scale"]).Equals(Vector3.zero))
//        {
//            g.transform.localScale = JSONTemplates.ToVector3(json["Scale"]);
//        }

//        return g.transform;
//    }
//}
