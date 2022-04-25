using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
[RequireComponent(typeof(Image))]

public class TabsButton : MonoBehaviour,IPointerClickHandler,IPointerExitHandler

{
    public Sprite spriteSelected;
    public Sprite spriteDeselected;
    public TabGroup tabGroup;
    public Image background;
    int defaultStart = 0; //PÁGINA INICIAL
    public void OnPointerClick(PointerEventData eventData)
    {
        tabGroup.OnTabSelected(this);
    }


    public void OnPointerExit(PointerEventData eventData)
    {
        tabGroup.OnTabExit(this);
    }

    private void Start()
    {
        background = GetComponent<Image>();
        tabGroup.Subscribe(this);
        if (this.transform.GetSiblingIndex() == defaultStart)
        {
            tabGroup.DefaultButtonActive(this);
        }
    }

}
