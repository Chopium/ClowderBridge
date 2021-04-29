using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
[RequireComponent(typeof(TextMeshProUGUI))]
public class textmesh_recieveFloatAsText : MonoBehaviour
{

    TextMeshProUGUI target;
    // Start is called before the first frame update
    public void setFloatAsText(float input)
    {
        if (target == null)
        {
            target = this.GetComponent<TextMeshProUGUI>();
        }
        target.SetText(input.ToString("#0.0"));
    }
}
