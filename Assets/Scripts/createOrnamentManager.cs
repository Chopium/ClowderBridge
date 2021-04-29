using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
public class createOrnamentManager : MonoBehaviour
{
    public bool tryEnableButton = false;
    public hide_size target;
    public bool didZoom = false;
    public bool didDrag = false;
    public bool didInspect = false;
    public List<RectTransform> buttons;
    public List<TextMeshProUGUI> texts;
    public TextMeshProUGUI createOrnamentReadout;
    // Start is called before the first frame update
    IEnumerator Start()
    {
        if (PlayerPrefs.GetInt("postedOrnament") > 0)
        {
            createOrnamentReadout.text = "Redo Ornament";
        }
        else
        {
            createOrnamentReadout.text = "Create Ornament";
        }
        Vector2 sizeDelta = buttons[0].sizeDelta;
        var seq = DOTween.Sequence();
        seq.AppendCallback(delegate
        {
            for (int i = 0; i < texts.Count; i++)
            {
                texts[i].transform.localScale = new Vector3(1, 0, 0);
                buttons[i].sizeDelta = new Vector2(sizeDelta.x, 0);
                buttons[i].gameObject.SetActive(false);

            }
            //texts[0].transform.localScale = new Vector3(1, 0, 0);
            //texts[1].transform.localScale = new Vector3(1, 0, 0);
            //texts[2].transform.localScale = new Vector3(1, 0, 0);
            //buttons[0].sizeDelta = new Vector2(sizeDelta.x, 0);
            //buttons[1].sizeDelta = new Vector2(sizeDelta.x, 0);
            //buttons[2].sizeDelta = new Vector2(sizeDelta.x, 0);
            //buttons[0].gameObject.SetActive(false);
            //buttons[1].gameObject.SetActive(false);
            //buttons[2].gameObject.SetActive(false);

        });
        
        while (Camera_Manager.Instance.inHold|| Camera_Manager.Instance.breaks > 0 || Camera_Manager.Instance.motionLock > 0)
        {
            //Debug.Log("hold one");
            yield return null;
        }
        if (PlayerPrefs.GetInt("postedOrnament") == 0)
        {
            seq = DOTween.Sequence();
            seq.AppendCallback(delegate
            {
                for (int i = 0; i < texts.Count; i++)
                {
                    buttons[i].gameObject.SetActive(true);
                }
                SoundBoard.Instance.playSound("hover_generic");
            });
            for (int i = 0; i < texts.Count; i++)
            {
                seq.Join(buttons[i].DOSizeDelta(sizeDelta, 0.25f));
                seq.Join(texts[i].transform.DOScale(new Vector3(1, 1, 1), 0.25f));
            }
            //seq.Join(buttons[0].DOSizeDelta(sizeDelta, 0.25f));
            //seq.Join(texts[0].transform.DOScale(new Vector3(1, 1, 1), 0.25f));
            //seq.Join(buttons[1].DOSizeDelta(sizeDelta, 0.25f));
            //seq.Join(texts[1].transform.DOScale(new Vector3(1, 1, 1), 0.25f));


            while (!didZoom || !didDrag || !didInspect || Camera_Manager.Instance.cameraLock > 0 || !tryEnableButton)
            {
                //Debug.Log("hold two");
                if (Camera_Manager.Instance.inSpin && !didDrag)
                {
                    didDrag = true;
                    //Vector2 sizeDelta = buttons[0].sizeDelta;
                    seq = DOTween.Sequence();
                    seq.Append(buttons[0].DOSizeDelta(new Vector2(sizeDelta.x, 0), 0.25f));
                    seq.Join(texts[0].transform.DOScale(new Vector3(1, 0, 0), 0.25f));
                    seq.AppendCallback(delegate
                    {
                        buttons[0].gameObject.SetActive(false);
                    });

                    SoundBoard.Instance.playSound("click_yes");
                }
                if (Input.mouseScrollDelta.y != 0 && Camera_Manager.Instance.breaks == 0 && !didZoom)
                {
                    didZoom = true;
                    //Vector2 sizeDelta = buttons[1].sizeDelta;
                    seq = DOTween.Sequence();
                    seq.Append(buttons[1].DOSizeDelta(new Vector2(sizeDelta.x, 0), 0.25f));
                    seq.Join(texts[1].transform.DOScale(new Vector3(1, 0, 0), 0.25f));
                    seq.AppendCallback(delegate
                    {
                        buttons[1].gameObject.SetActive(false);
                    });

                    SoundBoard.Instance.playSound("click_yes");
                }
                if (!didInspect && Camera_Manager.Instance.didInspect)
                {
                    didInspect = true;
                    //Vector2 sizeDelta = buttons[1].sizeDelta;
                    seq = DOTween.Sequence();
                    seq.Append(buttons[2].DOSizeDelta(new Vector2(sizeDelta.x, 0), 0.25f));
                    seq.Join(texts[2].transform.DOScale(new Vector3(1, 0, 0), 0.25f));
                    seq.AppendCallback(delegate
                    {
                        buttons[2].gameObject.SetActive(false);
                    });

                    SoundBoard.Instance.playSound("click_yes");
                }
                yield return null;
            }
        }
        
        yield return new WaitForSeconds(7f);
        foreach (var item in buttons)
        {
            Destroy(item.gameObject);
        }
        if (tryEnableButton)
        {
            target.setVisible(true);
        }
    }
}
