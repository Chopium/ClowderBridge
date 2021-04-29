using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InitializeButtonOnsubmit : MonoBehaviour
{
    private TMP_InputField field;

    // Start is called before the first frame update
    void Start()
    {
        field = this.GetComponent<TMP_InputField>();
        field.onSubmit.AddListener(delegate {
            Console.Instance.Activate();
            field.text = "";
            #if UNITY_EDITOR//will keep focus on input field if we're using a mouse and keyboard
            field.ActivateInputField();
            #endif
        });
    }
}
