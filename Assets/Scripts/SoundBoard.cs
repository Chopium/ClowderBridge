using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[Serializable]
public class SoundClip
{
    [SerializeField]
    public AudioClip[] sounds;//number of possible sounds
    [SerializeField]
    public Vector2 PitchMinMax = Vector2.one;
    [SerializeField]
    public Vector2 VolumeMinMax = Vector2.one;

    public AudioClip getSound()
    {
        return sounds[UnityEngine.Random.Range(0, sounds.Length)];
    }
    public float getVolume()
    {
        return UnityEngine.Random.Range(VolumeMinMax.x, VolumeMinMax.y);
    }
    public float getPitch()
    {
        //Debug.Log(PitchMinMax.x +" " +PitchMinMax.y);
        return UnityEngine.Random.Range(PitchMinMax.x, PitchMinMax.y);
    }
}

public class SoundBoard : MonoBehaviour
{
    #region Singleton / Start / Awake
    private static SoundBoard instance;
    public static SoundBoard Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject SoundBoard = new GameObject("SoundBoard");
                instance = SoundBoard.AddComponent<SoundBoard>();
            }
            return instance;
        }
    }


    void Awake()
    {
        instance = this;
        player = this.gameObject.AddComponent<AudioSource>();

        Application.logMessageReceived += HandleLog;

    }
    #endregion


    public SoundDictionary soundDictionary;
    AudioSource player;
    // Start is called before the first frame update


    public void playSound(string target)
    {
        if (soundDictionary.soundDictionary.ContainsKey(target))
        {
            player.pitch = soundDictionary.soundDictionary[target].getPitch();
            player.PlayOneShot(soundDictionary.soundDictionary[target].getSound(), soundDictionary.soundDictionary[target].getVolume());
        }
        else
        {
            Debug.Log("SOUNDBOARD DOESNT CONTAIN SOUND "+ target);
        }
    }
    void playSound(SoundClip overRide = null)
    {

    }
    public AudioSource getAudioSource(string target)
    {
        GameObject newSource = new GameObject();
        var sound = newSource.AddComponent<AudioSource>();

        if (soundDictionary.soundDictionary.ContainsKey(target))
        {
            sound.pitch = soundDictionary.soundDictionary[target].getPitch();
            sound.clip = soundDictionary.soundDictionary[target].getSound();
            sound.volume = soundDictionary.soundDictionary[target].getVolume();
            sound.Play();
        }
        else
        {
            Debug.Log("SOUNDBOARD DOESNT CONTAIN SOUND");
        }


        return sound;
    }
    public AudioClip getAudioClip(string target)
    {
        AudioClip sound = null;
        if (soundDictionary.soundDictionary.ContainsKey(target))
        {
            sound= soundDictionary.soundDictionary[target].getSound();
        }
        else
        {
            Debug.Log("SOUNDBOARD DOESNT CONTAIN SOUND");
        }


        return sound;
    }
    void HandleLog(string logString, string stackTrace, LogType type)
    {
        if (type.Equals(LogType.Error))
        {
            SoundBoard.Instance.playSound("error_generic");
        }

    }

}