using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.Events;

public class TabGroup : MonoBehaviour
{

    public List<TabButton> tabButtons;
    public Color tabIdle = Color.white;
    public Color tabHover = Color.yellow;
    public Color TabActive = Color.red;
    public TabButton selectedTab;
    public List<GameObject> objectsToSwap;

    public PanelGroup panelGroup;

    private void Awake()
    {
        foreach (TabButton button in tabButtons)
        {
            button.tabGroup = this;
        }
    }

    public void Subscribe(TabButton button)
    {
        if(tabButtons == null)
        {
            tabButtons = new List<TabButton>();

        }
        tabButtons.Add(button);
    }

    public void OnTabEnter(TabButton button)
    {
        ResetTabs();
        if (selectedTab == null || button != selectedTab)
        {
            button.background.color = tabHover;
        }
    }
    public void OnTabExit(TabButton button)
    {
        ResetTabs();
        //button.background.sprite = tabIdle;
    }
    public void OnTabSelected(TabButton button)
    {
        if(selectedTab != null)
        {
            selectedTab.Deselect();
        }
        selectedTab = button;
        selectedTab.Select();

        ResetTabs();
        button.background.color = TabActive;
        int index = button.transform.GetSiblingIndex();
        for(int i=0; i < objectsToSwap.Count; i++)
        {
            if(i==index)
            {
                objectsToSwap[i].SetActive(true);
            }
            else
            {
                objectsToSwap[i].SetActive(false);
            }
        }
        if (panelGroup != null)
        {
            panelGroup.SetPageIndex(button.transform.GetSiblingIndex());
        }
    }

    public void ResetTabs()
    {
        foreach(TabButton button in tabButtons)
        {
            if (selectedTab != null &&button == selectedTab) { continue; }
            button.background.color = tabIdle;
        }
    }

    public void UnselectAllTabs()
    {

        selectedTab = null;
        foreach (TabButton button in tabButtons)
        {
            //if (selectedTab != null && button == selectedTab) { continue; }
            button.background.color = tabIdle;
        }
    }

}
