using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class man_animation_manager : MonoBehaviour
{
    public float speed;
    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.GetInt("beat") > 0 && Random.Range(0f, 10f) >= 5)
        {
            Debug.Log("MAN GET");
        }
        else
        {
            Debug.Log("DESTROY MAN");
            Destroy(this.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.localPosition -= new Vector3(Time.deltaTime * speed, 0, 0);
    }
}
