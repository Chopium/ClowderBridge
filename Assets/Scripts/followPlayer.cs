using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class followPlayer : MonoBehaviour
{
    public bool doFollow = false;
    public void setDoFollow(bool input)
    {
        doFollow = input;
    }
    public Transform target;
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        if (doFollow)
        {
            this.transform.position = Vector3.MoveTowards(this.transform.position, target.transform.position, Vector3.Distance(this.transform.position, target.transform.position) * Time.deltaTime);

        }
    }
}
