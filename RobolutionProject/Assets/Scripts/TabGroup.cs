using System.Collections.Generic;
using UnityEngine;

public class TabGroup : MonoBehaviour
{
    [SerializeField] bool swapInitial;
    public List<TabsButton> tabsButtons;
    public List<GameObject> objectsToSwap;

    public TabsButton selectedTab;
    public int initialTab;
    public void Subscribe(TabsButton button)
    {
        if (tabsButtons == null)
        {
            tabsButtons = new List<TabsButton>();
        }
        tabsButtons.Add(button);
    }

    public void OnTabExit(TabsButton button)
    {
        ResetTabs();
    }
    public void OnTabSelected(TabsButton button)
    {
        selectedTab = button;
        ResetTabs();
        int index = button.transform.GetSiblingIndex();
        button.background.sprite = button.spriteSelected;
        for (int i = 0; i < objectsToSwap.Count; i++)
        {
            if (i == index)
            {
                objectsToSwap[i].SetActive(true);
            }
            else
            {
                objectsToSwap[i].SetActive(false);
            }
        }
    }
    public void DefaultButtonActive(TabsButton button)
    {
        selectedTab = button;
        int index = button.transform.GetSiblingIndex();
        button.background.sprite = button.spriteSelected;
        if (swapInitial)
        {
            for (int i = 0; i < objectsToSwap.Count; i++)
            {
                if (i == initialTab)
                {
                    objectsToSwap[i].SetActive(true);
                }
                else
                {
                    objectsToSwap[i].SetActive(false);
                }
            }
        }
    }
    public void ResetTabs() //Enfocar de manera distinta el reseteo de la imagen del botón
    {
        
        foreach (TabsButton button in tabsButtons)
        {
            if (selectedTab != null && button == selectedTab) { continue; }
            button.background.sprite = button.spriteDeselected;
        }
    }
}
