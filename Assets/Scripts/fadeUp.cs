using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fadeUp : MonoBehaviour
{
    public Material Target;
    Color startingColor = Color.black;
    Color endingColor = Color.white;
    private void Start()
    {
        Target.SetColor("_alphaMultiply", startingColor);
    }
    public void doFadeUp(float duration)
    {
        StartCoroutine(doFade(duration));
    }

    public IEnumerator doFade(float duration)
    {
        float currentTime = 0;
        while (currentTime < duration)
        {
            currentTime += Time.fixedDeltaTime;

            var amount = Mathf.Lerp(0, 1, currentTime / duration);
            amount = amount * amount * amount * (amount * (6f * amount - 15f) + 10f);

            Target.SetColor("_alphaMultiply", Color.Lerp(startingColor,endingColor,amount));

            //float scale = Mathf.Lerp(start, finalgravity, amount);

            yield return new WaitForFixedUpdate();
        }
    }
}
