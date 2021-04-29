using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
public class Ornament_2020_Interface : MonoBehaviour
{
    #region Singleton / Start / Awake
    private static Ornament_2020_Interface instance;
    public static Ornament_2020_Interface Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject Ornament_2020_Interface = new GameObject("Ornament_2020_Interface");
                instance = Ornament_2020_Interface.AddComponent<Ornament_2020_Interface>();
            }
            return instance;
        }
    }
    #endregion

    public GameObject GUI;
    public Ornament2020 localIdentity;
    public UnityEvent StatsUpdated;
    public Squishy squish;
    private mode _currentMode = mode.Placed;

    public enum mode { Inspect, Placed};

    [SerializeField]
    public mode currentMode
    {
        get { return _currentMode; }
        set
        {
                _currentMode = value;
                switch (_currentMode)
                {
                    case mode.Inspect:
                    this.gameObject.GetComponent<MeshCollider>().enabled = true;
                    this.gameObject.GetComponent<SphereCollider>().enabled = false;
                    squish.locked = false;
                    instance = this;
                    break;
                    case mode.Placed:

                    this.gameObject.GetComponent<MeshCollider>().enabled = false;
                    this.gameObject.GetComponent<SphereCollider>().enabled = true;
                    squish.locked = true;
                    break;
                    default:
                        break;
                }
        }
    }
    private void Awake()
    {
        if (currentMode == mode.Inspect)
        {
            instance = this;
        }
        localIdentity = new Ornament2020();
        //maybe randomize colors here
    }
    private void Start()
    {
        target = this.GetComponent<Renderer>().material;
        OscillationRandom = Random.Range(0, 10);
        //gets some nice starting colors
        if (currentMode == mode.Inspect && Ornament_Master.Instance.colorPresets != null && Ornament_Master.Instance.colorPresets.Length > 0)
        {
            Debug.Log("randomizing colors");
            //int randomColorPresetIndex = Random.Range(0, Ornament_Master.Instance.colorPresets.Length);
            colorPreset selected = Ornament_Master.Instance.colorPresets[Random.Range(0, Ornament_Master.Instance.colorPresets.Length)];
            localIdentity.colors[0] = selected.a;
            localIdentity.colors[1] = selected.b;
            localIdentity.colors[2] = selected.c;
            
        }
        if (currentMode == mode.Inspect)
        {
            Debug.Log("randomizingShape");
            localIdentity.shapeID = Random.Range(0, Ornament_Master.Instance.shapes.Count);
            setMesh(Ornament_Master.Instance.shapes[localIdentity.shapeID]);
        }
        ApplyColors();
        StatsUpdated.Invoke();

    }
    Material target;
    public void setColor(int index, Color color)
    {
        target.SetColor("_col" + index, color);
        localIdentity.colors[index] = color;
    }
    public void ApplyColors()
    {
        for (int i = 0; i < localIdentity.colors.Length; i++)
        {
            target.SetColor("_col" + i, localIdentity.colors[i]);
        }
    }


    private void OnMouseDown()
    {
        dragging = true;
        switch (currentMode)
        {
            case mode.Inspect:
                //lastMousePosition = Input.mousePosition;
                cursorManager.Instance.setCursor(5);
                break;
            case mode.Placed:
                if (true)
                {

                }
                //Debug.Log("DELETE");
                //Ornament_Master.Instance.DeleteObject(localIdentity, () => {
                //    //Ornament_Master.Instance.GameObjectCollection.Remove(this.gameObject);
                //    Debug.Log("INSIDE");
                //    this.gameObject.SetActive(false);
                //});
                break;
            default:
                break;
        }
    }
    Sequence tween;
    GuestSignature comment;
    public bool inInspect = false;
    private float timer = 0;
    //private void OnMouseOver()
    //{
    //    if (currentMode == mode.Placed && !Camera_Manager.Instance.didInspect && Camera_Manager.Instance.breaks == 0)
    //    {
    //        Debug.Log(timer);
    //        timer += Time.deltaTime;
    //        if (timer > 1.75)
    //        {
    //            Camera_Manager.Instance.didInspect = true;
    //        }
    //    }
    //}
    private void OnMouseEnter()
    {
        if (currentMode == mode.Placed && Camera_Manager.Instance.inspectLock < 1)
        {
            timer = 0;
            comment = guestBook.Instance.getObjectByID(localIdentity.unityID);
            if (comment != null && (comment.approved || masterSwitchInterface.Instance.user == masterSwitchInterface.userType.admin) && currentMode == mode.Placed && !Input.GetMouseButton(1) && !Input.GetMouseButton(2))
            {
                inInspect = true;
                //if (!Camera_Manager.Instance.didInspect && Camera_Manager.Instance.breaks == 0 )
                //{
                //    Camera_Manager.Instance.didInspect = true;
                //}
                squish.locked = false;
                cursorManager.Instance.setCursor(1);
                tween = DOTween.Sequence().AppendInterval(0.25f).AppendCallback(delegate
                {
                    
                    //Debug.Log("showing comment");
                    //Debug.Log(comment.name);
                    //Debug.Log(comment.location);
                    //Debug.Log(comment.message);
                    tooltip.Instance.Show(
                        "<color=#606060>" + "from:  " + "<color=#000000>" + comment.name,
                        "<color=#606060>" + "in:  " + "<color=#000000>" + comment.location,
                        comment.message);

                });

            }
            else if (comment == null)//no comment //|| masterSwitchInterface.Instance.user == masterSwitchInterface.userType.client
            {
                //Debug.Log("disable squish");
                cursorManager.Instance.setCursor(8);
                squish.locked = true;
            }
            else if (!comment.approved)//pending
            {
                cursorManager.Instance.setCursor(7);
                squish.locked = true;
            }
        }
        if (currentMode == mode.Inspect)//orbit inspected ornament
        {
            inInspect = true;
            cursorManager.Instance.setCursor(4);
        }
        
    }
    private void OnMouseUp()
    {
        dragging = false;
        if (currentMode == mode.Inspect && inInspect)
        {
            cursorManager.Instance.setCursor(4);
           
        }
    }
    private void OnMouseExit()
    {
        tween.Kill();
        if (inInspect)
        {
            cursorManager.Instance.setCursor(0);
            tooltip.Instance.Hide();
            inInspect = false;
        }
        else
        {
            cursorManager.Instance.setCursor(0);
        }
        
    }

    //private Vector3 lastMousePosition;
    private Vector3 deltaMousePosition;
    public Vector2 sensitivity = Vector2.one;
    private void OnMouseDrag()
    {
        switch (currentMode)
        {
            case mode.Inspect:
                //Debug.Log("HERE");
                deltaMousePosition = new Vector3 (Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"),0);
                //val += deltaMousePosition.y * sensitivity.y;  -Mathf.Sign(Camera.main.transform.forward.x) *
                //this.transform.localRotation *= new Quaternion.eulerAngles()

                this.transform.Rotate( 0, -deltaMousePosition.x * sensitivity.x,0 , Space.World);
                var difference = ((this.transform.position - Camera.main.transform.position)).normalized;
                difference.y = 0f;
                //this.transform.rotation *= Quaternion.FromToRotation(Camera.main.transform.up, Camera.main.transform.forward) -deltaMousePosition.y * sensitivity.y);
                this.transform.Rotate(Quaternion.AngleAxis(-deltaMousePosition.y * sensitivity.y, Vector3.Cross(difference, Vector3.up)).eulerAngles, Space.World);
                //Quaternion.Euler(-deltaMousePosition.y * sensitivity.y, 0, 0);
                   // Quaternion.AngleAxis(-deltaMousePosition.y * sensitivity.y, Camera.main.transform.right);
                //this.transform.Rotate(-deltaMousePosition.y * sensitivity.y, 0, 0, Space.World);
                
                //lastMousePosition = Input.mousePosition;
                break;
            case mode.Placed:
                //Debug.Log("DELETE");
                //Ornament_Master.Instance.DeleteObject(localIdentity, () => {
                //    //Ornament_Master.Instance.GameObjectCollection.Remove(this.gameObject);
                //    Debug.Log("INSIDE");
                //    this.gameObject.SetActive(false);
                //});
                
                break;
            default:
                break;
        }

    }

    private bool dragging = false;

    Color outlineColor;
    public float snapbackRate = 30;
    private float OscillationRandom;
    void Update()
    {
        switch (currentMode)
        {
            case mode.Inspect:
                if (!dragging)
                {
                    //Quaternion.Slerp
                    this.transform.localRotation = Quaternion.Slerp(this.transform.localRotation, (Quaternion.identity * Quaternion.Euler(-90f + Ocsillation.Between(-15, 15, 1, OscillationRandom), 180f + Ocsillation.Between(-15, 15, 1, OscillationRandom), Ocsillation.Between(-15, 15, 1.1f, OscillationRandom+2))), snapbackRate * Time.deltaTime);
                }
                break;
            case mode.Placed:
                break;
            default:
                break;
        }

        
    }
    public void setMesh(Mesh input)
    {
        this.GetComponent<MeshFilter>().mesh = input;
        this.GetComponent<MeshCollider>().sharedMesh = input;
    }
    public void nextShape()
    {
        localIdentity.shapeID += 1;
        if (localIdentity.shapeID >= Ornament_Master.Instance.shapes.Count )
        {
            localIdentity.shapeID = 0;
        }
        setMesh(Ornament_Master.Instance.shapes[localIdentity.shapeID]);
        //do we try to update our data back in the master or do we not care unless submitting to server?
    }
}
public class Ocsillation
{
    public static float Between(float min, float max, float frequency, float timeOffset = 0)
    {
        return Mathf.Lerp(min, max, Mathf.InverseLerp(-1, 1, Mathf.Sin(timeOffset + (Time.time * frequency))));
    }
}
