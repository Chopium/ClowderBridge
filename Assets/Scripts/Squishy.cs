using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;

public class Squishy : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler
{
    public Transform target;

    public  Vector3 startScale;

    public float shakeStrength = 1f;
    public float squishMultiplier = 1f;
    public float duration = 0.2f;
    public int vibrato = 20;
    public float elasticity = 0.5f;

    public string clickSound = "click_generic";
    public string hoverSound = "hover_generic";
    public float randomness = 90f;
    public bool locked = false;
    private void OnMouseDown()
    {
        if ( !locked)
        {
            Squish();
            //Debug.Log("CLICKED");
            SoundBoard.Instance.playSound(clickSound);
        }
        
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        OnMouseDown();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        OnMouseEnter();
    }
    private void OnMouseEnter()
    {
        //Debug.Log("MOUSE OVER");
        if (!Input.GetMouseButton(0) && !inSquish && !locked)
        {
            
            Squish();
            SoundBoard.Instance.playSound(hoverSound);
        }
        
    }
    
    private void Start()
    {
        if (target == null)
        {
            target = this.transform;
        }
        startScale = target.transform.localScale;
    }

  
    private bool inSquish = false;
    /// <summary> 
    /// Squishes the object with the parameters defined in the inspector.
    /// </summary>
    public void Squish()
    {
        //DOShakeScale(float duration, float/Vector3 strength, int vibrato, float randomness, bool fadeOut)
        //DOPunchScale(Vector3 punch, float duration, int vibrato, float elasticity)
        if (!inSquish)
        {
            startScale = target.transform.localScale;
            inSquish = true;
            target.DOShakeScale(duration, shakeStrength, vibrato, randomness, true).OnComplete(() => { inSquish = false; });
        }
    }
}