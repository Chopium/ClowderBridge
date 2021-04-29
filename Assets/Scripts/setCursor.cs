using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class setCursor : MonoBehaviour
{
    [SerializeField]
    public int onHover = -1;
    [SerializeField]
    public int onClick = -1;
    [SerializeField]
    public int onDrag = -1;
    [SerializeField]
    public int onExit = -1;
    public int pendingstate;
    public bool currentlyInDrag = false;

    public string downSound;
    public string hoverSound;
    public string dragsound;
    public string exitSound;

    private void OnMouseDown()
    {
        if (onClick != -1)
        {
            if (!currentlyInDrag)
            {
                cursorManager.Instance.setCursor(onClick);
                pendingstate = onHover;
            }
            else
            {
                pendingstate = onClick;
            }
            
        }
        if (downSound != null)
        {
            SoundBoard.Instance.playSound(downSound);
        }
    }
    private void OnMouseEnter()
    {
        if (onHover != -1)
        {
            if (!currentlyInDrag)
            {
                cursorManager.Instance.setCursor(onHover);
                pendingstate = onHover;
            }
            else
            {
                pendingstate = onHover;
            }
        }
        if (hoverSound != null)
        {
            SoundBoard.Instance.playSound(hoverSound);
        }
    }
    public void OnMouseExit()
    {
        if (!currentlyInDrag)
        {
            cursorManager.Instance.setCursor(onExit);
            pendingstate = onHover;
        }
        else
        {
            pendingstate = onExit;
        }
        if (exitSound != null)
        {
            SoundBoard.Instance.playSound(exitSound);
        }
    }
    private void OnMouseDrag()
    {
        
        if (!currentlyInDrag)
        {
            currentlyInDrag = true;
            StartCoroutine(doDrag());
        }
        
        if (onDrag != -1)
        {
            cursorManager.Instance.setCursor(onDrag);

        }
    }
    public IEnumerator doDrag()
    {
        AudioSource player = SoundBoard.Instance.getAudioSource(dragsound);
        player.Play();
        while (Input.GetMouseButton(0))
        {
            yield return null;
        }
        currentlyInDrag = false;
        cursorManager.Instance.setCursor(pendingstate);
        Destroy(player.gameObject);
    }
}
