using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.VFX;
using TMPro;
public class Camera_Manager : MonoBehaviour
{
    #region Singleton / Start / Awake
    private static Camera_Manager instance;
    public static Camera_Manager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject bridge = new GameObject("Camera_Manager");
                instance = bridge.AddComponent<Camera_Manager>();
            }
            return instance;
        }
    }

    void Awake()
    {
        instance = this;
    }
    #endregion



    public float introDepth = 400f;
    public float introDistance = 100f;
    public float introDuration = 7f;

    public Transform cameraOrigin;
    public Transform cameraLookTarget;
    public Transform orbitCenter;

    public Transform ornamentEditor;
    public Transform OrnamentThrowPosition;
    public Vector3 ornamentEditorGoalPosition;
    public Vector3 ornamentEditorHiddenPosition;
    public GameObject logo;
    public Transform halfWayPosition;
    public Collider Tree;
    public VisualEffect snow;

    [HideInInspector]
    public bool didInspect = false;
    Sequence GUISequence;
    [HideInInspector]
    public int motionLock = 0;
    
    public bool readComments = true;
    public bool inHold = true;
    public void leaveHold()
    {
        inHold = false;
    }
    // Start is called before the first frame update
    IEnumerator Start()
    {
        breaks += 1;
        cameraLock += 0;
        motionLock = 0;
        ornamentEditorGoalPosition = ornamentEditor.localPosition;
        ornamentEditor.localPosition = ornamentEditorGoalPosition + ornamentEditorHiddenPosition;
        ornamentEditor.gameObject.SetActive(false);

        //Vector3 originalCameraPosition = cameraOrigin.localPosition;
        //cameraOrigin.localPosition = cameraOrigin.localPosition + logo.transform.localPosition / 2;
        //Camera.main.transform.position = cameraOrigin.position;
        //cameraLookTarget.transform.position = logo.transform.position + Vector3.up * 100f;

        //randomize rotation
        //orbitCenter.Rotate(Vector3.up * Random.Range(0,360), Space.Self);

        var Seq2 = DOTween.Sequence();
        Seq2.AppendInterval(introDuration/2);
        Seq2.Append(logo.GetComponent<Renderer>().material.DOFade(0, introDuration / 2));
        Seq2.Join(logo.GetComponentInChildren<TextMeshPro>().DOFade(0, introDuration / 2));
        Seq2.Join(cameraLookTarget.DOLocalMove(cameraLookTarget.localPosition, introDuration));
        

        //var Seq3 = DOTween.Sequence();
        //Seq3.AppendInterval(introDuration/2);
        //Seq3.AppendInterval(introDuration / 4);
        //Seq3.Join(logo.GetComponent<Renderer>().material.DOFade(0, introDuration / 4));
        ////Seq2.AppendInterval(introDuration / 4);


        GUISequence = DOTween.Sequence();
        GUISequence.AppendCallback(delegate {
            Debug.Log("Starting Intro");
            motionLock += 1;
        });

        //GUISequence.Append(cameraOrigin.DOLocalMove(cameraOrigin.localPosition - (Vector3.up * introDepth) + (cameraOrigin.forward * introDistance), 0));
        //GUISequence.Join(cameraLookTarget.DOLocalMove(cameraLookTarget.localPosition - Vector3.up * introDepth, 0));

        GUISequence.AppendCallback(delegate
        {
            cameraOrigin.localPosition = cameraOrigin.localPosition + logo.transform.localPosition / 2;
            Camera.main.transform.position = cameraOrigin.position;
            cameraLookTarget.transform.position = logo.transform.position + Vector3.up * -50f;
            GUISequence.Pause();
        });
        GUISequence.Append(cameraOrigin.DOLocalMove(cameraOrigin.localPosition, introDuration * 1.5f));
        GUISequence.Join(cameraOrigin.DOLocalMoveY(40f, introDuration * 2f));
        
        
        //GUISequence.Join(breaks});
        GUISequence.AppendCallback(delegate {
            Debug.Log("Completed Intro");
            logo.SetActive(false);
            motionLock -= 1;
            cameraLock -= 1;
        });

        
        Seq2.Pause();
        //Seq3.Pause();
        float currentTime = 0;
        while (inHold || currentTime < 3f)
        {
            currentTime += Time.deltaTime;
            yield return null;
        }
        snow.SetVector3("position", new Vector3(0,500,0));
        snow.SetVector3("bounds", new Vector3(2000,1000,2000));
        breaks -= 1;
        GUISequence.Play();
        Seq2.Play();
        //Seq3.Play();
    }
    public float lookAwayDuration = 5f;
    public Vector3 lookAwayPosition = Vector3.right;
    bool inLookAway = false;
    public void lookAway(bool input)
    {
        inLookAway = input;
        motionLock += 1;
        inspectLock += 1;
        Vector3 goalPosition = cameraLookTarget.localPosition;
        goalPosition += input ? lookAwayPosition : -lookAwayPosition;

        GUISequence = DOTween.Sequence();
        GUISequence.AppendCallback(delegate {
            Debug.Log("Looking Away");
        });
        GUISequence.Append(cameraLookTarget.DOLocalMove(goalPosition, lookAwayDuration));
        GUISequence.AppendCallback(delegate {
            Debug.Log("Complete");
            motionLock -= 1;
            inspectLock -= 1;
        });
    }

    bool ornamentActive = false;
    public void setOrnamentActive(bool input)
    {
        ornamentActive = input;
        motionLock += 1;
        Vector3 goalPosition = ornamentEditorGoalPosition;
        if(!input)goalPosition += ornamentEditorHiddenPosition;

        GUISequence = DOTween.Sequence();
        GUISequence.AppendCallback(delegate {
            if (input) ornamentEditor.gameObject.SetActive(input);
            
            Debug.Log("Moving Ornament");
        });
        GUISequence.Append(ornamentEditor.DOLocalMove(goalPosition, lookAwayDuration));
        GUISequence.AppendCallback(delegate {
            Debug.Log("Complete");
            if (!input) ornamentEditor.gameObject.SetActive(input);

            motionLock -= 1;
        });
    }

    public void setOrnamentPlaceMode(bool input)
    {
        if (input)
        {
            StartCoroutine(placeOrnament());
        }
        else//unplace Ornament
        {
            ornamentEditor.SetParent(this.transform);
            GUISequence = DOTween.Sequence();
            GUISequence.Append(Ornament_2020_Interface.Instance.GUI.transform.DOScale(1f, lookAwayDuration));
            GUISequence.Join(ornamentEditor.DOLocalMove(ornamentEditorGoalPosition, lookAwayDuration));
            
        }
    }
    public int inspectLock = 0;
    //public GameObject contactSprite;
    public IEnumerator placeOrnament()
    {
        Ornament_Master.Instance.fetchOrnaments();
        //Debug.Log("fetching ornaments");
        if (Mathf.Abs(cameraOrigin.localPosition.z) > (CameraDepthPosition.x+  CameraDepthPosition.y/6))
        {
            cameraOrigin.DOLocalMoveZ((CameraDepthPosition.x + CameraDepthPosition.y / 6), 1f);

        }
        if (Mathf.Abs(cameraOrigin.localPosition.y) < (CameraHeightPosition.x + CameraHeightPosition.y / 2))
        {
            cameraOrigin.DOLocalMoveY((CameraHeightPosition.x + CameraHeightPosition.y / 2), 1f);
        }

        inspectLock +=1;
        //contactSprite = Instantiate(contactSprite);
        //contactSprite.SetActive(false);
        lookAway(false);
        GUISequence = DOTween.Sequence();
        GUISequence.Append(Ornament_2020_Interface.Instance.GUI.transform.DOScale(0f, lookAwayDuration));
        GUISequence.Join(ornamentEditor.DOLocalMove(OrnamentThrowPosition.localPosition, lookAwayDuration));
        bool didPlace = false;
        RaycastHit hit;
        Ray ray;
        Vector3 hitTarget = Vector3.zero;
        Vector3 hitNormal = Vector3.zero;
        GameObject newOrnament = null;
        while (!didPlace)
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 1000f))
            {
                if (hit.collider.Equals(Tree) && hit.normal.y > -.1f) 
                {
                    hitTarget = hit.point;
                    hitNormal = hit.normal;
                    hitNormal.y /= 4;
                    cursorManager.Instance.setCursor(3);
                    if (Input.GetMouseButtonDown(0))
                    {
                        SoundBoard.Instance.playSound("click_yes");
                        didPlace = true;
                        newOrnament = Ornament_Master.Instance.submitOrnament(hit, Ornament_2020_Interface.Instance.localIdentity);
                    }
                }
                else
                {
                    cursorManager.Instance.setCursor(2);
                }
            }
            else
            {
                cursorManager.Instance.setCursor(0);
            }
            
            yield return null;
        }
        //Destroy(contactSprite);
        cursorManager.Instance.setCursor(0);
        ornamentEditor.SetParent(Tree.transform.parent);
        GUISequence = DOTween.Sequence();
        GUISequence.Append(ornamentEditor.DOMove(hitTarget, lookAwayDuration));
        GUISequence.AppendCallback(delegate {
            var seq2 = DOTween.Sequence();
            seq2.AppendInterval(lookAwayDuration * 3 / 4);
            GUISequence.AppendCallback(delegate {
                SoundBoard.Instance.playSound("ornament_hit");
            });
        });
            GUISequence.Join(ornamentEditor.DORotate(Quaternion.LookRotation(-hitNormal, Vector3.up).eulerAngles, lookAwayDuration));
        GUISequence.Join(ornamentEditor.DOScale(Ornament_Master.Instance.groupSizeFactor, lookAwayDuration));
        GUISequence.AppendCallback(delegate{
            //newOrnament.transform.localScale = Vector3.one * Ornament_Master.Instance.groupSizeFactor;
            ornamentEditor.transform.localScale = Vector3.one;
            ornamentEditor.gameObject.SetActive(false);
            newOrnament.SetActive(true);
            //SoundBoard.Instance.playSound("cheer");
            

            inspectLock -= 1;
        });
        GUISequence.AppendInterval(0.5f);
        GUISequence.AppendCallback(delegate {
            SoundBoard.Instance.playSound("cheer");
        });

        //StartCoroutine(placeOrnament());
        yield return null;
    }
    public float turbulenceFactor = 50f;
    public Vector3 rotation = new Vector3(15, 30, 45);
    public Vector2 CameraHeightPosition = new Vector2(0, 5);
    public Vector2 CameraDepthPosition = new Vector2(0, 5);
    public int breaks = 0;
    public int cameraLock = 1;
    void Update()
    {
        if (breaks <= 0)
        {
            orbitCenter.Rotate(inSpin ? (Vector3.up * filteredTurnIntent * mouseXSensitivity) : (rotation * lastDirection * Time.deltaTime), Space.Self);

        }
        if (cameraLock == 0)
        {
            if (Input.GetMouseButtonDown(1) && motionLock <= 0)
            {
                startMouseDrag(1);
            }
            cameraOrigin.localPosition += Vector3.forward * mouseYScrollSensitivity * (-Mathf.Sign(Input.mouseScrollDelta.y) * (Mathf.Min(maxSpinSpeed, Mathf.Pow(Input.mouseScrollDelta.y, 2))));

            cameraOrigin.localPosition = new Vector3(0, Mathf.Clamp(cameraOrigin.localPosition.y, CameraHeightPosition.x, CameraHeightPosition.y), Mathf.Clamp(cameraOrigin.localPosition.z, CameraDepthPosition.x, CameraDepthPosition.y));
        
        }
        turbulenceFactor = Mathf.Lerp(50f,5000f, Mathf.Max(0,Mathf.InverseLerp(500,2000f,Vector3.Distance(Camera.main.transform.position, Vector3.zero))));
        //snow.sha
    }

    public float mouseXSensitivity = 0.02f;
    public float mouseYSensitivity = 1;
    public float mouseYScrollSensitivity = 10;
    public float maxSpinSpeed = 8;
    public float maxScrollSpeed = 8;
    Vector2 mouseDelta;
    [HideInInspector]
    public bool inSpin = false;
    private int lastDirection = 1;
    private float filteredTurnIntent;

    public IEnumerator mouseDrag(int mouseID)
    {
        //Debug.Log("in drag");
        inSpin = true;
        while (Input.GetMouseButton(mouseID))
        {

            //Debug.Log("in drag 2");
            mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            if (mouseDelta.x != 0)
            {
                lastDirection = mouseDelta.x >= 0 ? 1 : -1;
                //Debug.Log(lastDirection);
            }
            filteredTurnIntent = lastDirection * Mathf.Min(maxSpinSpeed, mouseDelta.x * mouseDelta.x);

            cameraOrigin.localPosition += Vector3.up * mouseDelta.y * mouseYSensitivity;

            yield return null;
        }
        inSpin = false;
    }
    public void startMouseDrag(int mouseID)
    {
        if (motionLock <= 0 && !inLookAway && (inspectLock == 0|| mouseID == 1))
        {
            if (inSpin)
            {
                Debug.LogError("SHOULDNT BE ABLE TO CLICK SPIN TWICE");
            }
            else
            StartCoroutine(mouseDrag(mouseID));
            
        }
        else
        {
            //Debug.Log("failed check" + motionLock +" "+ !inLookAway + " " + (inspectLock == 0) + " " + (mouseID == 1));
        }
        
    }



}
