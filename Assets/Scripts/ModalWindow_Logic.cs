using System.Collections;
using System.Collections.Generic;
//using UnityEngine;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ModalWindow_Logic : MonoBehaviour
{
    public TextMeshProUGUI topic;
    public Button exitButton;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DestroySelf()
    {
        ModalWindowLauncher.Instance.CloseSelfModal(this);
    }
}
