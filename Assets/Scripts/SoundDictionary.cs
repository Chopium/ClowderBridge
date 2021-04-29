using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class StringSoundClipDictionary : SerializableDictionary<string, SoundClip> { }

[CreateAssetMenu(menuName = "Sound Dictionary")]
public class SoundDictionary : ScriptableObject
{
    [SerializeField]
    private StringSoundClipDictionary stringSoundClipStore = StringSoundClipDictionary.New<StringSoundClipDictionary>();
    public Dictionary<string, SoundClip> soundDictionary
    {
        get { return stringSoundClipStore.dictionary; }
    }
}