using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class setBaseOrnamentToInspect : MonoBehaviour
{
    public Ornament_2020_Interface target;
    // Start is called before the first frame update
    void Awake()
    {
        target.currentMode = Ornament_2020_Interface.mode.Inspect;
    }
}
