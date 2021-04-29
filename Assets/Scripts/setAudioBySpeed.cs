using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class setAudioBySpeed : MonoBehaviour
{
    #region Singleton / Start / Awake
    private static setAudioBySpeed instance;
    public static setAudioBySpeed Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject setAudioBySpeed = new GameObject("setAudioBySpeed");
                instance = setAudioBySpeed.AddComponent<setAudioBySpeed>();
            }
            return instance;
        }
    }

    void Awake()
    {
        instance = this;
    }
    #endregion


    public AudioSource target;
    Transform source;
    public Vector2 sourceSpeed = new Vector2(0, 10f);
    public Vector2 appliedVolume = new Vector2(0,1f);
    public Vector2 appliedPitch = new Vector2(0, 1f);
    public float volumeMultiplier = 1;
    Vector3 lastPosition;
    float speed;
    float adjustedSpeed = 0f;
    public float fastness = 1f;
    // Start is called before the first frame update
    public AudioSource music;
    private float musicFadeFactor = 0.25f;
    IEnumerator Start()
    {
        music = SoundBoard.Instance.getAudioSource("music_day");
        music.Pause();
        music.loop = true;
        volumeMultiplier = 0f;
        source = this.transform;
        lastPosition = source.transform.position;
        
        yield return new WaitForSeconds(1);
        target.volume = 0;
        //target.DOFade()
        while (Camera_Manager.Instance.breaks > 0)
        {
            yield return null;
        }
    
        music.volume = 0.25f;
        music.Play();
        DOTween.To(() => musicFadeFactor, x => musicFadeFactor = x, 0.3f, 20f).SetEase(Ease.InSine);
        DOTween.To(() => volumeMultiplier, x => volumeMultiplier = x, 1, 4f);
    }

    // Update is called once per frame
    void Update()
    {
        speed =  (source.transform.position - lastPosition ).magnitude/ Time.deltaTime;
        speed = Mathf.InverseLerp(sourceSpeed.x, sourceSpeed.y, speed);
        adjustedSpeed = Mathf.MoveTowards(adjustedSpeed, speed, Time.deltaTime * fastness );
        lastPosition = source.transform.position;
        target.volume = volumeMultiplier * Mathf.Lerp(appliedVolume.x, appliedVolume.y, adjustedSpeed);
        music.volume = Mathf.Min(musicFadeFactor,1 - target.volume);
        target.pitch = Mathf.Lerp(1, Mathf.Lerp(appliedPitch.x, appliedPitch.y, adjustedSpeed), volumeMultiplier);
    }
}
