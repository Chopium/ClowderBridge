using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using DG.Tweening;
[System.Serializable]
public class cursorStyle
{
    public string name;
    public Sprite image;
    public float idealSize = 0.2f;
    public Color defaultcolor = Color.white;
    public float stretchiness = .01f;
}
public class cursorManager : MonoBehaviour
{
    [SerializeField]
    public List<cursorStyle> cursors;
    public Color currentcolor;
    public float currentSize;
    public int currentCursor = -1;
    public RectTransform cursor;
    public Image targetImage; 
    private  float stretchiness = .01f;
    public float transitionTime = 0.1f;
    #region Singleton / Start / Awake
    private static cursorManager instance;
    public static cursorManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject bridge = new GameObject("cursorManager");
                instance = bridge.AddComponent<cursorManager>();
            }
            return instance;
        }
    }

    public void setCursor(int index = 0)
    {
        if (currentCursor != index)
        {
            targetImage.sprite = cursors[index].image;
            DOTween.To(() => currentSize, x => currentSize = x, cursors[index].idealSize, transitionTime);
            //currentSize = cursors[index].idealSize;
            DOTween.To(() => stretchiness, x => stretchiness = x, cursors[index].stretchiness, transitionTime);
            //stretchiness = cursors[index].stretchiness;
            DOTween.To(() => targetImage.color, x => targetImage.color = x, cursors[index].defaultcolor, transitionTime);
            //targetImage.color = cursors[index].defaultcolor;

            if (stretchiness == 0)
            {
                if (cursor.rotation != Quaternion.identity)
                {
                    cursor.rotation = Quaternion.identity;
                }
            }
            currentCursor = index;
        }
        
    }
    public IEnumerator Start()
    {
        setCursor(5);
        yield return new WaitForSeconds(1f);
        Cursor.visible = false;
        setCursor(0);
        lastPosition = Input.mousePosition;
    }
    public Vector3 lastPosition;
    public void Update()
    {
        Vector3 deltaPosition = lastPosition - Input.mousePosition;
        if (deltaPosition.x != 0 && stretchiness != 0)
        {
            float angle = Mathf.Rad2Deg * Mathf.Atan(deltaPosition.y / deltaPosition.x);
            cursor.rotation = Quaternion.Euler(0, 0, angle);
            
        }
        else
        {
            if (cursor.rotation != Quaternion.identity)
            {
                cursor.rotation = Quaternion.identity;
            }
        }
        cursor.localScale = new Vector3(currentSize + deltaPosition.magnitude * stretchiness, currentSize- Mathf.Min(currentSize-.1f,deltaPosition.magnitude * stretchiness), currentSize);
        cursor.position = Input.mousePosition;
        cursor.Translate(Vector3.down * (deltaPosition.magnitude * stretchiness /2 ), Space.Self);
        lastPosition = Input.mousePosition;

    }
    void Awake()
    {
        instance = this;
    }
    #endregion

    
}
