using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class openEyes : MonoBehaviour
{
    // Start is called before the first frame update
    public float duration;
    public float upAmount;
    public float delay;
    void Start()
    {
        StartCoroutine(doOpenEyes());
    }

    public IEnumerator doOpenEyes()
    {
        Vector3 startPosition = this.transform.localPosition;

        float currentTime = 0;
        while (currentTime < delay)
        {
            currentTime += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        currentTime = 0;
        while (currentTime < duration)
        {
            currentTime += Time.fixedDeltaTime;

            var amount = Mathf.Lerp(0, 1, currentTime / duration);
            amount = amount * amount * amount * (amount * (6f * amount - 15f) + 10f);
            this.transform.localPosition = startPosition + Vector3.up * Mathf.Sign(this.transform.localPosition.y) * Mathf.Lerp(0, upAmount, amount);
            yield return new WaitForFixedUpdate();
        }
        Destroy(this.gameObject);
    }
}
