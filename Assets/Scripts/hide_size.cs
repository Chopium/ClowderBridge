using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class hide_size : MonoBehaviour
{
    Vector3 originalScale;
    private bool currentState = true;
    public bool hideOnStart = false;
    // Start is called before the first frame update
    void Start()
    {
        originalScale = this.transform.localScale;
        if (hideOnStart)
        {
            this.transform.localScale = Vector3.zero;
            currentState = false;
        }
    }
    public bool getCurrentState()
    {
        return currentState;
    }
    public void simpleSetVisible(bool input)
    {
        setVisible(input);
    }
    public bool setVisible(bool input)//returns true if we changed Something
    {
        if (input != currentState)
        {
            if (!input)
            {
                this.transform.DOScale(0, 0.25f);
                SoundBoard.Instance.playSound("click_contract");
            }
            else
            {
                this.transform.DOScale(originalScale, 0.25f);
                SoundBoard.Instance.playSound("click_expand");
            }
            currentState = input;
            return true;
        }
        else
        {
            return false;
        }
        
        
    }
}
