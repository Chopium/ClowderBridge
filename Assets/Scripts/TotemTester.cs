using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TotemTester : MonoBehaviour
{
    public LoadTotem lt1;
    public LoadTotem lt2;
    // Start is called before the first frame update
    void Start()
    {
        // lt1.LoadObject();

    }
    void Update() {
        if ( Input.GetKeyDown( KeyCode.Alpha1 ) ) {
            lt1.LoadObject();
        }
        if ( Input.GetKeyDown( KeyCode.Alpha2 ) ) {
            lt2.LoadObject();
        }
    }
}
