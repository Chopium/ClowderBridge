using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class R3D_Transform : R3D_MetadataBase
{
    //JsonRef sender;
    override public JSONObject MakeJson()
    {
        JSONObject report = new JSONObject();

        //JSONObject Position = new JSONObject();
        //Vector3 TargetPosition = this.transform.localPosition;
        //Position.AddField("X", TargetPosition.x);
        //Position.AddField("Y", TargetPosition.y);
        //Position.AddField("Z", TargetPosition.z);

        //JSONObject Rotation = new JSONObject();
        //Quaternion TargetRotation = this.transform.localRotation;
        //Rotation.AddField("W", TargetRotation.w);
        //Rotation.AddField("X", TargetRotation.x);
        //Rotation.AddField("Y", TargetRotation.y);
        //Rotation.AddField("Z", TargetRotation.z);

        //JSONObject Scale = new JSONObject();
        //Vector3 TargetScale = this.transform.localScale;
        //Scale.AddField("X", TargetScale.x);
        //Scale.AddField("Y", TargetScale.y);
        //Scale.AddField("Z", TargetScale.z);

        JSONObject Position = JSONTemplates.FromVector3(this.transform.localPosition);
        JSONObject Rotation = JSONTemplates.FromQuaternion(this.transform.localRotation);
        JSONObject Scale = JSONTemplates.FromVector3(this.transform.localScale);

        report.AddField("Position", Position);
        report.AddField("Rotation", Rotation);
        report.AddField("Scale", Scale);
        //this.GetComponent<JsonRef>().accessData(report);
        return report;
    }
    private void Awake()
    {
        this.transform.localPosition = Vector3.zero;
        this.transform.localRotation = Quaternion.identity;
        //this.transform.localScale = Vector3.one;
    }

    override public void ApplyJson(JSONObject master)
    {
        //JSONObject Position = master.GetField("Position");
        //this.transform.localPosition = new Vector3(Position.GetField("X").f, Position.GetField("Y").f, Position.GetField("Z").f);
        //JSONObject Rotation = master.GetField("Rotation");
        //this.transform.localRotation = new Quaternion( Rotation.GetField("X").f, Rotation.GetField("Y").f, Rotation.GetField("Z").f, Rotation.GetField("W").f );
        //JSONObject Scale = master.GetField("Scale");
        //this.transform.localScale = new Vector3(Scale.GetField("X").f, Scale.GetField("Y").f, Scale.GetField("Z").f);

        this.transform.localPosition = JSONTemplates.ToVector3(master["Position"]);
        this.transform.localRotation = JSONTemplates.ToQuaternion(master["Rotation"]);
        if (!JSONTemplates.ToVector3(master["Scale"]).Equals(Vector3.zero))
        {
            this.transform.localScale = JSONTemplates.ToVector3(master["Scale"]);
        }
    }

    static public Transform JSONtoTransform(JSONObject json)
    {
        GameObject g = new GameObject();
        //JSONObject Position = json.GetField("Position");
        //g.transform.localPosition = new Vector3(Position.GetField("X").f, Position.GetField("Y").f, Position.GetField("Z").f);

        //JSONObject Rotation = json.GetField("Rotation");
        //g.transform.localRotation = new Quaternion( Rotation.GetField("X").f, Rotation.GetField("Y").f, Rotation.GetField("Z").f, Rotation.GetField("W").f );

        //JSONObject Scale = json.GetField("Scale");
        //g.transform.localScale = new Vector3(Scale.GetField("X").f, Scale.GetField("Y").f, Scale.GetField("Z").f);
        g.transform.localPosition = JSONTemplates.ToVector3(json["Position"]);
        //Debug.Log(Position.GetField("X")+" "+ Position["y"] + Position["z"]);
        //Debug.Log(JSONTemplates.ToVector3(Position));

        //JSONObject Rotation = master.GetField("Rotation");
        g.transform.localRotation = JSONTemplates.ToQuaternion(json["Rotation"]);
        //JSONObject Scale = master.GetField("Scale");
        g.transform.localScale = JSONTemplates.ToVector3(json["Scale"]);

        return g.transform;
    }

}


//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class R3D_Transform : R3D_MetadataBase
//{

//    override public JSONObject MakeJson()
//    {
//        JSONObject report = new JSONObject();

//        JSONObject Position = new JSONObject();
//        Vector3 TargetPosition = this.transform.localPosition;
//        Position.AddField("X", TargetPosition.x);
//        Position.AddField("Y", TargetPosition.y);
//        Position.AddField("Z", TargetPosition.z);

//        JSONObject Rotation = new JSONObject();
//        Quaternion TargetRotation = this.transform.localRotation;
//        Rotation.AddField("W", TargetRotation.w);
//        Rotation.AddField("X", TargetRotation.x);
//        Rotation.AddField("Y", TargetRotation.y);
//        Rotation.AddField("Z", TargetRotation.z);

//        JSONObject Scale = new JSONObject();
//        Vector3 TargetScale = this.transform.localScale;
//        Scale.AddField("X", TargetScale.x);
//        Scale.AddField("Y", TargetScale.y);
//        Scale.AddField("Z", TargetScale.z);

//        report.AddField("Position", Position);
//        report.AddField("Rotation", Rotation);
//        report.AddField("Scale", Scale);
//        //this.GetComponent<JsonRef>().accessData(report);
//        return report;
//    }
//    private void Awake()
//    {
//        this.transform.localPosition = Vector3.zero;
//        this.transform.localRotation = Quaternion.identity;

//    }

//    override public void ApplyJson(JSONObject master)
//    {
//        JSONObject Position = master.GetField("Position");
//        this.transform.localPosition = new Vector3(Position.GetField("X").f, Position.GetField("Y").f, Position.GetField("Z").f);

//        JSONObject Rotation = master.GetField("Rotation");
//        this.transform.localRotation = new Quaternion( Rotation.GetField("X").f, Rotation.GetField("Y").f, Rotation.GetField("Z").f, Rotation.GetField("W").f );
//        JSONObject Scale = master.GetField("Scale");
//        this.transform.localScale = new Vector3(Scale.GetField("X").f, Scale.GetField("Y").f, Scale.GetField("Z").f);
//    }

//    static public Transform JSONtoTransform( JSONObject json ) {
//        GameObject g = new GameObject();
//        JSONObject Position = json.GetField("Position");
//        g.transform.localPosition = new Vector3(Position.GetField("X").f, Position.GetField("Y").f, Position.GetField("Z").f);

//        JSONObject Rotation = json.GetField("Rotation");
//        g.transform.localRotation = new Quaternion( Rotation.GetField("X").f, Rotation.GetField("Y").f, Rotation.GetField("Z").f, Rotation.GetField("W").f );

//        JSONObject Scale = json.GetField("Scale");
//        g.transform.localScale = new Vector3(Scale.GetField("X").f, Scale.GetField("Y").f, Scale.GetField("Z").f);

//        return g.transform;
//    }

//}
