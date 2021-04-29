using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class func_reveal : MonoBehaviour
{
    public void doFadeUp(float duration)
    {
        StartCoroutine(doFade(duration));
    }

    public IEnumerator doFade(float duration)
    {
        Vector3 initialPosition = this.transform.localPosition;
        float currentTime = 0;
        while (currentTime < duration/2)
        {
            currentTime += Time.fixedDeltaTime;

            var amount = Mathf.Lerp(0, 1, currentTime / duration);
            amount = amount * amount * amount * (amount * (6f * amount - 15f) + 10f);

            this.transform.localPosition = initialPosition + ( Vector3.down * Mathf.Lerp(0, 100, amount));

            yield return new WaitForFixedUpdate();
        }
        Destroy(this.gameObject);
    }
}
