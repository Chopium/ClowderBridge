using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{

    public IEnumerator doFade(AudioSource target, float goal, float duration)
    {
        float currentTime = 0;
        float initialVolume = target.volume;
        while (currentTime < duration)
        {
            currentTime += Time.fixedDeltaTime;

            var amount = Mathf.Lerp(0, 1, currentTime / duration);
            amount = amount * amount * amount * (amount * (6f * amount - 15f) + 10f);
            target.volume = Mathf.Lerp(initialVolume, goal,amount);
            yield return new WaitForFixedUpdate();
        }
        Debug.Log("COMPLETED TRANSITION: " + target.clip.name + " " + goal + " " + duration);
    }
}
