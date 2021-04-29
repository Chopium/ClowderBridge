using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlowIndicatorAnimator : MonoBehaviour
{
    public Color HighColor= Color.white;
    public Color LowColor = Color.black;
    public float interval = .25f;
    Material Target;
    // Start is called before the first frame update
    void Start()
    {
        Target = GetComponent<Renderer>().material;
    }
    private void OnEnable()
    {
        if (Target == null)
        {
            Target = GetComponent<Renderer>().material;
        }
        StartCoroutine(runInterval());
    }
    private void OnDisable()
    {
        StopAllCoroutines();
    }
    float timeStep = 0;
    // Update is called once per frame
    public IEnumerator runInterval()
    {
        while (this.isActiveAndEnabled)
        {
            timeStep = (Mathf.Sin(Time.time * 3.14f * 1 / interval) + 1) / 2;
            Target.color = Color.Lerp(HighColor, LowColor, timeStep);
            yield return new WaitForFixedUpdate();
        }
        
    }
}
