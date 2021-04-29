using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class increaseWhirlWithPosition : MonoBehaviour
{
    public Renderer target1;
    public Renderer target2;
    //public Renderer target3;
    //public Renderer target4;
    public Vector2 startEndSpeed;
    public Vector2 startEndHeight;
    float whirlSpeed;
    // Start is called before the first frame update
    void Start()
    {
        //target = this.GetComponent<Renderer>();
    }
    public float playerHeight;
    // Update is called once per frame
    void Update()
    {
        //180
        //Debug.Log(Camera.main.transform.position.y);
        playerHeight = Camera.main.transform.position.y;
        whirlSpeed = Mathf.Lerp(startEndSpeed.x, startEndSpeed.y, Mathf.InverseLerp(startEndHeight.x, startEndHeight.y, playerHeight));
        target1.sharedMaterial.SetFloat("_whirlspeed", whirlSpeed);
        //target3.sharedMaterial.SetFloat("_whirlspeed", whirlSpeed + 0.5f);
        //target4.sharedMaterial.SetFloat("_whirlspeed", whirlSpeed + 0.5f);
        target2.sharedMaterial.SetFloat("_whirlspeed", whirlSpeed+0.001f);
    }
}
