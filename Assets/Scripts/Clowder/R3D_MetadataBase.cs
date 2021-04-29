using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class R3D_MetadataBase : MonoBehaviour
{
    abstract public JSONObject MakeJson();
    abstract public void ApplyJson(JSONObject master);
    
}
