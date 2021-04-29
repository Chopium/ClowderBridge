using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sunAnimator : MonoBehaviour
{
    Quaternion goalRotation;
    public Renderer sun01;
    public Renderer sun02;

    private Color sun01color;
    private Color sun02color;
    // Start is called before the first frame update
    void Start()
    {
        goalRotation = this.transform.rotation;
        sun02color = sun02.material.GetColor("_MainColor");


    }

    // Update is called once per frame
    void Update()
    {
        this.transform.rotation = goalRotation;
        //Debug.Log(Vector3.Dot(Camera.main.transform.forward, this.transform.forward));
        sun02.material.SetColor("_MainColor", Color.Lerp(Color.clear, sun02color, .2f + Vector3.Dot(Camera.main.transform.forward, this.transform.forward)));



    
    }

    



}
