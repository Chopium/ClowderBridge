using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using TMPro;
public class nightSwap : MonoBehaviour
{
    #region Singleton / Start / Awake
    private static nightSwap instance;
    public static nightSwap Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject nightSwap = new GameObject("nightSwap");
                instance = nightSwap.AddComponent<nightSwap>();
            }
            return instance;
        }
    }

    void Awake()
    {
        instance = this;
    }
    #endregion
    public GameObject nightGroup;
    public List<MeshFilter> targets;
    public List<Mesh> replacements;
    public VisualEffect snow;
    public GameObject lights;
    public TextMeshPro countDownText;
    List<Mesh> originals;
    private bool _nightMode = false;
    public bool nightMode
    {
        get
        {
            return _nightMode;
        }

        set
        {
            if (value != _nightMode)
            {
                Debug.Log("switching");
                switchMode(value);
                _nightMode = value;
            }



        }
    }
    private int _SystemDate = -1;
    public int SystemDate
    {
        get { return _SystemDate; }
        set
        {
            // Ignore if value has not changed
            if (_SystemDate == value) return;
            _SystemDate = value;

        }
    }

    public void forceNoUpdate()
    {
        StopAllCoroutines();//end check time coroutine
    }
    private void Start()
    {
        submissionDeadline = new DateTime(2021, 1, 1, 0, 0, 1);
        StartCoroutine(updateTime());
    }
    private void switchMode(bool input)
    {
        if(input)
        {
            //reference originals
            if (originals == null)
            {
                originals = new List<Mesh>();
                for (int i = 0; i < targets.Count; i++)
                {
                    originals.Add(targets[i].sharedMesh);
                }
            }
            //swap
            for (int i = 0; i < targets.Count; i++)
            {
                targets[i].mesh = replacements[i];
            }
        }
        else
        {
            //swap
            for (int i = 0; i < targets.Count; i++)
            {
                targets[i].mesh = originals[i];
            }
        }
        lights.SetActive(input);
        snow.SetInt("rate", input ? 100 : 300);
        nightGroup.SetActive(input);
        setAudioBySpeed.Instance.music.clip = SoundBoard.Instance.getAudioClip(input ? "music_night" : "music_day");
        if (setAudioBySpeed.Instance.music.isPlaying)
        {
            doPlayMusic = true;
        }
        if (doPlayMusic)
        {
            setAudioBySpeed.Instance.music.Play();
        }
        //Debug.Log(nightSwap.Instance.nightMode ? "music_night" : "music_day");
    }
    private bool doPlayMusic = false;
    //private void Update()
    //{
    //    if(Input.GetKeyDown(KeyCode.C))
    //    {
    //        nightMode = !nightMode;
    //    }
    //}
    int oldSystemDate = -1;
    WaitForSeconds waiter;
    private DateTime submissionDeadline;
    private DateTime calendarDate;
    private int daysLeft;
    public IEnumerator updateTime()
    {
        waiter = new WaitForSeconds(30);
        while (true)
        {
            calendarDate = DateTime.Now;
            //Debug.Log(calendarDate.ToShortDateString());
            //Debug.Log(submissionDeadline.ToShortDateString());
            if (masterSwitchInterface.Instance.currentSettings.forceDayNightMode == 0)
            {
                Debug.Log(calendarDate.Hour);
                if (calendarDate.Hour < 7 || calendarDate.Hour > 17)
                {
                    nightMode = true;
                }
                else
                {
                    nightMode = false;
                }
            }
            daysLeft = submissionDeadline.Subtract(calendarDate).Days;
            if (daysLeft > 0)
            {
                countDownText.text = "- Closing in " + daysLeft + " Day" + (daysLeft > 1 ? "s" : "")+" -";//Subtract(calendarDate)
            }
            else
            {
                countDownText.text = "- Completed "+-daysLeft+ " Day" + (-daysLeft > 1 ? "s" : "")+" Ago. -";//Subtract(calendarDate)
            }
            yield return waiter;
        }
    }
}
