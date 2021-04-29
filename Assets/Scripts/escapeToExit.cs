using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;
public class escapeToExit : MonoBehaviour
{
    public Image fadeOut;
    public float holdCount = 1.5f;
    float currentCount = 0;
    public bool exiting = false;
    Tween FadeOutTween;
    public hide_size introText;
    // Start is called before the first frame update
    void Start()
    {
        Sequence introSequence = DOTween.Sequence();
        #if UNITY_STANDALONE
        if (masterSwitchInterface.Instance.user.Equals(masterSwitchInterface.userType.client))
        {
            introSequence.AppendCallback(delegate
            {
                introText.setVisible(true);
            });
            
            introSequence.AppendInterval(3f);

            introSequence.AppendCallback(delegate
            {
                introText.setVisible(false);
            });
        }
#else
        introSequence.AppendInterval(1f);
#endif
        introSequence.AppendInterval(0.5f);

        introSequence.Append(fadeOut.DOFade(0, Camera_Manager.Instance.introDuration/5).SetEase(Ease.InSine));
        
        //fadeOut.DOFade(0, Camera_Manager.Instance.introDuration);
        //UnityEngine.Cursor.visible = false;
    }
    private bool inhold = false;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Escape) && !exiting)
        {
            if (!inhold)
            {
                inhold = true;
                FadeOutTween.Kill();
                FadeOutTween = fadeOut.DOFade(1, holdCount - currentCount);
            }

            currentCount += Time.deltaTime;

            //fadeOutMaterial.color = Color.Lerp(Color.clear, Color.black, currentCount / holdCount);
            if (currentCount > holdCount)
            {
                //openEyes.Instance.eyesOpen = false;
                Debug.Log("EXITING");
                exiting = true;
                Invoke("doExit", 0.5f);
            }
        }
        else if(!exiting)
        {
            inhold = false;
            if (currentCount > 0)
            {
                FadeOutTween.Kill();
                FadeOutTween = fadeOut.DOFade(0, currentCount);
                currentCount -= Time.fixedDeltaTime;
            }
        }
    }
    public void doExit()
    {

#if UNITY_STANDALONE
        if (Application.isEditor)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else
            Application.Quit();
#else
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
#endif
    }
}
