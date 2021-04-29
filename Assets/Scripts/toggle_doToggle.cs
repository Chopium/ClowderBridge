using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(Toggle))]
public class toggle_doToggle : MonoBehaviour
{

    Toggle target; 
    // Start is called before the first frame update
    public void toggle()
    {
        if(target == null)
        {
            target = this.GetComponent<Toggle>();
        }
        target.isOn = !target.isOn;
    }
}
