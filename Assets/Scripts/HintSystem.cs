using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;

[System.Serializable]
public class textSegment
{
    public int textTargetIndex = 0;
    public float lineDelay = 0;
    public string description;
    public List<string> text;
    public Color textColor = Color.white;
    public AudioClip voice;
    public float textSpeed = 0.001f;
    public bool clickToAdvance = true;
    public bool mustManuallyClear = false;
}

public class HintSystem : MonoBehaviour
{
    //WaitForFixedUpdate fixedUpdater;
    //WaitForSeconds timedUpdater
    private static HintSystem instance;
    public static HintSystem Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject bridge = new GameObject("PlayerController_Jump");
                instance = bridge.AddComponent<HintSystem>();
            }
            return instance;
        }
    }
    void Awake()
    {
        instance = this;
        //Regex rx = new Regex(@"<\?PDS No=\""(.+?)\""");
    }

    public TextMeshProUGUI[] targets;
    //public TextMeshProUGUI target;
    public AudioSource speechSource;

    [SerializeField]
    public List<textSegment> texts;


    public int locked = 0;

    public void playText(int textIndex, float lineDelay, float postBreak)
    {
        currentText = textIndex;
        StartCoroutine(showPoem(texts[textIndex], lineDelay, postBreak));
    }

    public IEnumerator showHint()
    {
        yield return null;
    }

    public bool doClearText;
    public void clearText()
    {
        currentText = -1;
        doClearText = true;
    }
    public void playTextGeneric(int textIndex)
    {
        currentText = textIndex;
        StartCoroutine(showPoem(texts[textIndex], 5f, 0f));
    }


    public void playSpaceTip()
    {
        playText(1, 3f, 0f);
    }
    public void playEndingRamble()
    {
        if (PlayerPrefs.GetInt("beat") != 1)
        {
            playText(4, 3f, 0f);
        }
        else
        {
            Invoke("playEndingDelayed", 10f);
            //"playText(7, 3f, 0f)";
        }
        
    }
    private void playEndingDelayed()
    {
        playText(7, 3f, 0f);
    }
    public void startSHIFTTip()
    {
        StartCoroutine(startSHIFT());
    }
    public IEnumerator startSHIFT()
    {
        playText(2, 0, 2.5f);
        while (!Input.GetKey(KeyCode.LeftShift))
        {
            yield return null;
        }
        clearText();
    }



    public void startWASDTip()
    {
        StartCoroutine(startWASD());
    }
    public IEnumerator startWASD()
    {
        playText(0, 2.5f, 2.5f);
        while ((Mathf.Abs(Input.GetAxis("Horizontal")) + Mathf.Abs(Input.GetAxis("Vertical"))) <= 1f)
        {
            yield return null;
        }
        //clearText();
    }
    public int currentText = -1;
    public float scrollrate = 0.001f;
    public IEnumerator showPoem(textSegment input, float delay = 0, float breakTime = 0)
    {
        locked += 1;

        targets[input.textTargetIndex].color = input.textColor;
        foreach (var line in input.text)
        {
            float currenttime = 0f;
            if (input.textSpeed > 0)
            {
                targets[input.textTargetIndex].text = "";
                string growingText = "";
                for (int i = 0; i < line.Length; i++)
                {
                    //Debug.Log(line[i]);
                    if (line[i].Equals('<'))
                    {
                        //Debug.Log("HERE");
                        while (!line[i].Equals('>') && i < line.Length)
                        {
                            growingText = string.Concat(growingText, line[i]);
                            //Debug.Log(line[i]);
                            i++;
                        }
                    }
                    growingText = string.Concat(growingText, line[i]);


                    //if (line[i].Equals("."))
                    //{

                    //}
                    if (input.voice != null && !line[i].Equals(" ") && !line[i].Equals(".") && !line[i].Equals("\""))
                    {
                        if (Random.Range(0f, 1f) > 0.5f)
                        {
                            speechSource.pitch = Random.Range(0.6f, 1.2f);
                            speechSource.PlayOneShot(input.voice);
                        }
                    }

                    //target.text = "‏‏‎." + growingText+  new System.String(' ', line.Length - i) + ".";
                    targets[input.textTargetIndex].text = growingText;

                    //Wait a certain amount of time, then continue with the for loop
                    float currentTime = 0;
                    while (currentTime < input.textSpeed)
                    {
                        currentTime += Time.deltaTime;
                        yield return null;
                    }
                }
            }
            else
            {
                targets[input.textTargetIndex].text = line;
            }


            if (!input.mustManuallyClear)//we manually clear for tutorial stuff. don't destroy until a piece of code does it
            {
                //wait to continue til key press
                if (input.clickToAdvance)
                {
                    while (!Input.anyKey)
                    {
                        yield return null;
                    }
                }
                else if (delay > 0)//if we don't click to advance, use our delay time
                {
                    currenttime = 0f;
                    if (input.lineDelay > 0)//if our line group has different delays than the generic one we were handed in with the coroutine
                    {
                        while (currenttime < input.lineDelay)
                        {
                            //Debug.Log(currenttime);
                            currenttime += Time.deltaTime;
                            yield return null;
                        }
                    }
                    else
                    {
                        while (currenttime < delay)
                        {
                            currenttime += Time.deltaTime;
                            yield return null;
                        }
                    }
                }

                targets[input.textTargetIndex].text = "";
                currenttime = 0f;
                while (currenttime < (breakTime))
                {
                    currenttime += Time.deltaTime;
                    yield return null;
                }
                //Debug.Log("CLEARING1");
            }
            else //if we must manually clear
            {
                doClearText = false;
                while (!doClearText)
                {
                    yield return null;
                }
                //Debug.Log("CLEARING2");
                targets[input.textTargetIndex].text = "";
                currenttime = 0f;
                while (currenttime < (breakTime))
                {
                    currenttime += Time.deltaTime;
                    yield return null;
                }
            }
        }
        currentText = -1;
        locked -= 1;
    }
}


//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using TMPro;

//[System.Serializable]
//public class textSegment
//{
//    public string description;
//    public List<string> text;
//    public Color textColor = Color.white;
//    public AudioClip voice;
//    public float textSpeed = 0.001f;
//    public bool clickToAdvance = true;
//    public bool mustManuallyClear = false;
//}

//public class HintSystem : MonoBehaviour
//{
//    public TextMeshProUGUI target;
//    public AudioSource speechSource;

//    [SerializeField]
//    public List<textSegment> texts;

//    // Start is called before the first frame update
//    void Start()
//    {
//        //playText(3, 2f, 0f);
//        //StartCoroutine(SpillHints(15f, 30f));
//        //GameManager.Instance.sessionEnd.AddListener(dripHint);
//    }

//    public int locked = 0;

//    public void playText(int textIndex, float lineDelay, float postBreak)
//    {
//        StartCoroutine(showPoem(texts[textIndex], lineDelay, postBreak));
//    }

//    public IEnumerator showHint()
//    {
//        yield return null;
//    }

//    public bool doClearText;
//    public void clearText()
//    {
//        doClearText = true;
//    }
//    public void playTextGeneric(int textIndex)
//    {
//        StartCoroutine(showPoem(texts[textIndex], 2f, 0f));
//    }


//    public void playSpaceTip()
//    {
//        playText(1, 3f, 0f);
//    }
//    public void playEndingRamble()
//    {
//        playText(4, 3f, 0f);
//    }

//    public void startSHIFTTip()
//    {
//        StartCoroutine(startSHIFT());
//    }
//    public IEnumerator startSHIFT()
//    {
//        playText(2, 0, 2.5f);
//        while (!Input.GetKey(KeyCode.LeftShift))
//        {
//            yield return new WaitForFixedUpdate();
//        }
//        clearText();
//    }



//    public void startWASDTip()
//    {
//        StartCoroutine(startWASD());
//    }
//    public IEnumerator startWASD()
//    {
//        playText(0, 2.5f, 2.5f);
//        while ((Mathf.Abs(Input.GetAxis("Horizontal")) + Mathf.Abs(Input.GetAxis("Vertical"))) <= 1f)
//        {
//            yield return new WaitForFixedUpdate();
//        }
//        //clearText();
//    }
//    float currentTime = 0;
//    public float scrollrate = 0.001f;
//    public IEnumerator showPoem(textSegment input, float delay = 0, float breakTime = 0)
//    {
//        locked += 1;
//        foreach (var line in input.text)
//        {
//            float currenttime = 0f;
//            if (input.textSpeed > 0)
//            {

//                target.text = "";
//                string growingText = "";
//                for (int i = 0; i < line.Length; i++)
//                {
//                    growingText = string.Concat(growingText, line[i]);

//                    if (input.voice != null && !line[i].Equals(" ") && !line[i].Equals(".") && !line[i].Equals("\""))
//                    {
//                        if (Random.Range(0f,1f) > 0.5f)
//                        {
//                            speechSource.pitch = Random.Range(0.6f, 1.2f);
//                            speechSource.PlayOneShot(input.voice);
//                        }

//                    }

//                    //target.text = "‏‏‎." + growingText+  new System.String(' ', line.Length - i) + ".";
//                    target.text = growingText;
//                    currentTime = 0;
//                    while (currentTime < input.textSpeed)
//                    {
//                        yield return null;
//                    }
//                }
//            }
//            else
//            {
//                target.text = line;
//            }


//            if (!input.mustManuallyClear)//we manually clear for tutorial stuff. don't destroy until a piece of code does it
//            {
//                //wait to continue til key press
//                if (input.clickToAdvance)
//                {
//                    while (!Input.anyKey)
//                    {
//                        yield return null;
//                    }
//                }
//                else if (delay > 0)//if we don't click to advance, use our delay time
//                {
//                    currenttime = 0f;
//                    while (currenttime < delay)
//                    {
//                        currenttime += Time.fixedDeltaTime;
//                        yield return new WaitForFixedUpdate();
//                    }
//                }

//                target.text = "";
//                currenttime = 0f;
//                while (currenttime < (breakTime))
//                {
//                    currenttime += Time.fixedDeltaTime;
//                    yield return new WaitForFixedUpdate();
//                }
//            }
//            else //if we must manually clear
//            {
//                doClearText = false;
//                while (!doClearText)
//                {
//                    yield return new WaitForFixedUpdate();
//                }
//                target.text = "";
//                currenttime = 0f;
//                while (currenttime < (breakTime))
//                {
//                    currenttime += Time.fixedDeltaTime;
//                    yield return new WaitForFixedUpdate();
//                }
//            }
//        }
//        locked -= 1;
//    }
//}
