using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
[ExecuteInEditMode]
public class GetName : MonoBehaviour
{
    public bool autofetch = true;
    string source;
    TextMeshProUGUI target;
    #if UNITY_EDITOR
    // Update is called once per frame

    void Update()
    {
        if (autofetch)
        {
            source = this.transform.parent.gameObject.name;
            if (target == null)
            {
                target = this.GetComponent<TextMeshProUGUI>();
            }
                target.SetText(source);
        }
        
    }
    #endif
   public void setName(string s)
    {
        if (target == null)
        {
            target = this.GetComponent<TextMeshProUGUI>();
        }
            target.SetText(s);

    }
    public void Start()
    {
            if (target == null)
            {
                target = this.GetComponent<TextMeshProUGUI>();
            }
            source = this.transform.parent.gameObject.name;
            target.SetText(source);
    }
}
