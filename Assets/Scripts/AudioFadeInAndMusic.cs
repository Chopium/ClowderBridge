using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioFadeInAndMusic : MonoBehaviour
{
    #region Singleton
    private static AudioFadeInAndMusic instance;
    public static AudioFadeInAndMusic Instance
    {
        get
        {
            if (instance == null)
            {

                instance = new AudioFadeInAndMusic();
            }
            return instance;
        }
    }
    void Awake()
    {
        instance = this;
    }
    #endregion
    public AudioSource neutralMusic;
    public AudioSource tensionMusic;

    public AudioMixer MasterMixer;

    // Start is called before the first frame update

    public void setStartMode()
    {
        setAudioLevelImmediate(MasterMixer, "volume_master", -80f);
        setAudioLevelImmediate(MasterMixer, "volume_music", -80f);
        setAudioLevelImmediate(MasterMixer, "volume_ball", -80f);
        FadeInMaster();
    }
    void Start()
    {
        setStartMode();
    }
    public void beginPlay()
    {
        setAudioLevelSlow(MasterMixer, "volume_ball", 0f, 1f);
        FadeInMusic();
    }

    public void FadeInMusic()
    {
        neutralMusic.Play();
        tensionMusic.Play();
        setAudioLevelSlow(MasterMixer, "volume_music", -12f, 1f);

        tensionMusic.volume = 0;
        StartCoroutine(TensionDecay(tensionMusic, neutralMusic, 0));


        StartCoroutine(AddTension(tensionMusic, neutralMusic, 1, tensionAdditionRate));

    }
    public void FadeInMaster()
    {
        setAudioLevelSlow(MasterMixer, "volume_master", 0f, 1f);
    }


    public bool modulatingTension = true;
    public float tensionDecayRate = .02f;
    public float tensionAdditionRate = .1f;
    private IEnumerator TensionDecay(AudioSource tension, AudioSource normal, float quietVolume)
    {
        while(modulatingTension)
        {
            float decayAmount = Time.fixedDeltaTime * tensionDecayRate;
            //Debug.Log(decayAmount);
            if (tension.volume > quietVolume)
            {
                tension.volume -= decayAmount;
            }
            if (normal.volume < 1f)
            {
                normal.volume += decayAmount;
            }


            yield return new WaitForFixedUpdate(); 
        }
        yield return null;
    }
    
    public float addTensionDampener = 4;
    public void addToTension(float input)
    {
        //Debug.Log("recieved tension  " + input);
        tensionLevel += input/addTensionDampener;//expecting adding 1 for each infraction so devide by dampener to say, 6 infractions = volume 1
    }
    float tensionLevel;
    public float startNeutralFade = 1;
    private IEnumerator AddTension(AudioSource tension, AudioSource normal, float fullVolume, float tensionAddRate)//tensionAddRate like 1/10 means every fixed step we take that portion of the total tension amount and submit it to our player
    {
        float tensionIncrement;
        while (modulatingTension)
        {
            tensionIncrement = ((tensionLevel* Time.fixedDeltaTime) * tensionAddRate);
            //Debug.Log("tensionIncrement+= "+tensionIncrement);
            tensionLevel -= tensionIncrement;

            if (tension.volume < fullVolume)
            {
                tension.volume += tensionIncrement;
            }

            if(tensionLevel > startNeutralFade && normal.volume > 0f )
            {
                //Debug.Log("subtracting from normal");
                normal.volume -= tensionIncrement;
                //tensionLevel -= tensionIncrement;//begin scaling cooldown
            }

            yield return new WaitForFixedUpdate();
        }
    }
    public void setAudioLevelSlow(AudioMixer target, string targetAttribute, float goalValue, float duration)
    {
        StartCoroutine(LerpFadeAudio(target, targetAttribute, goalValue, duration));
    }
    public void setAudioLevelImmediate(AudioMixer target, string targetAttribute, float goalValue)
    {
        target.SetFloat(targetAttribute, goalValue);
    }
    public IEnumerator LerpFadeAudio(AudioMixer target, string targetAttribute, float goalValue, float duration)
    {
        float startingValue;
        target.GetFloat(targetAttribute, out startingValue);
           
        float currentTime = 0;
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            var amount = Mathf.Lerp(startingValue, goalValue, currentTime / duration);
            //amount = amount * amount * amount * (amount * (6f * amount - 15f) + 10f);// smooth
            target.SetFloat(targetAttribute, amount);


            yield return null;
        }
        target.SetFloat(targetAttribute, goalValue);

    }
    public IEnumerator LerpFadeAudioSource(AudioSource target, float goalValue, float duration)
    {
        float startingValue = target.volume;

        float currentTime = 0;
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            var amount = Mathf.Lerp(startingValue, goalValue, currentTime / duration);
            amount = amount * amount * amount * (amount * (6f * amount - 15f) + 10f);// smooth
            target.volume = amount;


            yield return null;
        }
        target.volume = goalValue;

    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
