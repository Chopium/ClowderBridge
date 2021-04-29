using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotateRandom : MonoBehaviour
{
    public List<GameObject> targets;
    // Start is called before the first frame update
    void Start()
    {
        float amount = Random.Range(0f, 360f);

        foreach (var item in targets)
        {
            item.transform.Rotate(Vector3.up * amount, Space.World);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
