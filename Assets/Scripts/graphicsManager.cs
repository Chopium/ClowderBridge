using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class graphicsManager : MonoBehaviour
{
    #region Singleton / Start / Awake
    private static graphicsManager instance;
    public static graphicsManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new graphicsManager();
            }
            return instance;
        }
    }
    void Awake()
    {
        instance = this;
        setFullScreen(Screen.fullScreen);
        //retro = ScriptableObject.CreateInstance<RetroPostProcessEffect>();
        //retro = PostProcessManager.instance.settingsTypes< RetroPostProcessEffect
    }
    #endregion

    //RetroPostProcessEffect retro = null;
    public void setLowResMode(bool b)
    {
        //#if UNITY_STANDALONE
        //lowResMode = b;
        //retro.enabled.Override(b);
        ////retro = ScriptableObject.CreateInstance<RetroPostProcessEffect>();
        ////GameObject.FindGameObjectWithTag("PostProcess").GetComponent<PostProcessProfile>().settings..enabled.Override(b);
        ////gameObject..GetComponentInChildren<RetroPostProcessEffect>().active = b;
        //#endif
}

public bool lowResMode = true;

    public void setPSXShaders(bool b)
    {
        PSXShaders = b;
    }
    public bool PSXShaders = true;

    public void setFullScreen(bool b)
    {
        isFullScreen = b;
        Screen.fullScreen = b;
    }
    public bool isFullScreen = true;
    

}
