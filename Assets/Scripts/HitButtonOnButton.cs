using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HitButtonOnButton : MonoBehaviour
{
    public Button target;

    public string submitButton;
    // Update is called once per frame
    void Update()
    {
        if(target != null)
        {
            if (Input.GetButtonDown(submitButton))
            {
                target.onClick.Invoke();
            }
        }
    }
}
