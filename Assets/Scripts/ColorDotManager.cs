using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorDotManager : MonoBehaviour
{
    public Ornament_2020_Interface source;
    public ColorPicker_Interactable GUI;
    public ColorDot_Interactable[] buttons;
    Sequence GUISequence;
    
    private ColorDot_Interactable currentButton;
    bool animating = false;
    public float expandedScale = 1f;
    public float moveDuration = 0.25f;
    public Vector3 expandedOffset = new Vector3(-0.5f, -0.5f, -0.5f);
    // Start is called before the first frame update
    void InitializeButtonColors()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            //Debug.Log(source.localIdentity.colors[i]);
            buttons[i].setColor(source.localIdentity.colors[i]);
        }
    }
    public void setColorPicker(ColorDot_Interactable target)
    {
        if (currentButton != target)
        {
            GUISequence = DOTween.Sequence();
            if (GUI.gameObject.activeSelf && currentButton !=null)
            {
                DeactivateColorPicker();
            }
            
            ActivateColorPicker(target);

        }
    }

    private void ActivateColorPicker(ColorDot_Interactable target)
    {
        
        //Debug.Log("ACTIVATE");
        GUISequence.AppendCallback(delegate {
            GUI.setColor(target.ColorTarget.color);
            GUI.currentIndex = target.index;
            GUI.visualizer = target.ColorTarget;
            GUI.gameObject.SetActive(true);
            target.gameObject.SetActive(false);

        });
        GUISequence.Append(GUI.transform.DOLocalMove(target.transform.localPosition, 0));
        GUISequence.Append(GUI.transform.DOLocalMove(buttons[1].transform.localPosition + expandedOffset, moveDuration)); //GUI.transform.InverseTransformPoint(
        GUISequence.Join(GUI.transform.DOScale(expandedScale * Vector3.one, moveDuration));
        GUISequence.AppendCallback(delegate {
            currentButton = target;
        });
        

    }
    public void ConfirmColorPicker()
    {
        if (GUI.gameObject.activeSelf)
        {
            GUISequence = DOTween.Sequence();
            DeactivateColorPicker();

        }
    }
    private void DeactivateColorPicker()
    {
        //Debug.Log("DEACTIVATE");
        GUISequence.Append(GUI.transform.DOLocalMove(currentButton.transform.localPosition, moveDuration));//.onComplete(()=>GUI.gameObject.SetActive(false)); //GUI.transform.InverseTransformPoint(
        GUISequence.Join(GUI.transform.DOScale(Vector3.zero, moveDuration));
        GUISequence.AppendCallback(delegate {
            GUI.gameObject.SetActive(false);
            currentButton.gameObject.SetActive(true);
            //Debug.Log("TRUE");
            currentButton = null;
        });
        
    }
    private void Awake()
    {
        //set up buttons to trigger this script
        source.StatsUpdated.AddListener(InitializeButtonColors);
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].index = i;
            buttons[i].mouseDown.AddListener(setColorPicker);
        }
        //currentButton = buttons[0];
        GUI.transform.localScale = Vector3.zero;
        GUI.gameObject.SetActive(false);
        //GUISequence = DOTween.Sequence();
    }
    
}
