using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class tooltip : MonoBehaviour
{
    public TextMeshProUGUI UserField;
    public TextMeshProUGUI LocationField;
    public TextMeshProUGUI contentField;

    public LayoutElement layoutElement;
    public RectTransform rectTransform;
    public int characterWrapLimit;

    public RectTransform callout;
    public Transform leftPosition;
    public Transform rightPosition;
    public hide_size hider;
    public float extraDistance = 1f;
    private float appliedExtraOffset = 0f;
    public LineRenderer lineBoy;

    public void setText(string a = "", string b = "", string c = "")
    {
        UserField.text = a;
        LocationField.text = b;
        contentField.text = c;

        int headerLength = Mathf.Max(UserField.text.Length, LocationField.text.Length, contentField.text.Length);
        layoutElement.enabled = headerLength > characterWrapLimit;
    }
    public void Show(string a = "", string b = "", string c = "")
    {
        setText(a, b, c);
        if (hider.setVisible(true))
        {
            SoundBoard.Instance.playSound("click_expand");
            DOTween.To(() => appliedExtraOffset, x => appliedExtraOffset = x, extraDistance, 0.25f);
            Camera_Manager.Instance.breaks += 1;
            //timer = 0;
        }
    }
    public void Hide()
    {
        if (hider.setVisible(false))
        {
            SoundBoard.Instance.playSound("click_contract");
            DOTween.To(() => appliedExtraOffset, x => appliedExtraOffset = x, 0, 0.25f);
            Camera_Manager.Instance.breaks -= 1;
            //timer = 0;
        }
        
    }
    #region Singleton / Start / Awake
    private static tooltip instance;
    public static tooltip Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject bridge = new GameObject("tooltip");
                instance = bridge.AddComponent<tooltip>();
            }
            return instance;
        }
    }
    
    void Awake()
    {
        instance = this;
    }
    #endregion
    public float CursorLeadAdjustmentFactor = 1.05f;
    float timer = 0;
    public void Update()
    {
        if (hider.getCurrentState())
        {
            Vector2 position = Input.mousePosition;
            transform.position = position;
            float pivotX = position.x / Screen.width;
            float pivotY = position.y / Screen.height;
            rectTransform.pivot = new Vector2(pivotX, pivotY);

            if (pivotX > 0.5)
            {
                callout.transform.localPosition = Vector3.MoveTowards(callout.transform.localPosition, rightPosition.transform.localPosition, Time.deltaTime * 1000);
            }
            else
            {
                callout.transform.localPosition = Vector3.MoveTowards(callout.transform.localPosition, leftPosition.transform.localPosition, Time.deltaTime * 1000);
            }
            position.x += pivotX > 0.5 ? -(float)appliedExtraOffset * (float)Screen.height : (float)appliedExtraOffset * (float)Screen.height;
            //position.y += pivotY > 0.5 ? -extraDistance : extraDistance;
            transform.position = position;

            Vector3 calloutGUI = Camera.main.ScreenToWorldPoint(new Vector3(callout.transform.position.x, callout.transform.position.y, 50f));
            Vector3 calloutHand = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 50f));
            calloutHand += cursorManager.Instance.currentSize* CursorLeadAdjustmentFactor * (calloutGUI - calloutHand).normalized;

            if (!lineBoy.enabled)
            {
                lineBoy.enabled = true;
            }
            lineBoy.enabled = true;
            lineBoy.SetPositions(new Vector3[] {calloutGUI, calloutHand });

            if (!Camera_Manager.Instance.didInspect)
            {
                //Debug.Log(timer);
                timer += Time.deltaTime;
                if (timer > 2f)
                {
                    Camera_Manager.Instance.didInspect = true;
                }
            }
        }
        else
        {
            if (lineBoy.enabled)
            {
                lineBoy.enabled = false;
            }

        }
        
    }
}
