using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class EditToggle : Selectable, IPointerClickHandler, ISubmitHandler, IPointerUpHandler, IPointerExitHandler
{
    public Sprite toggledOnSprite;
    public Sprite toggledOffSprite;



    void Start()
    {
        GetComponent<Image>().sprite = toggledOffSprite;
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        if (GridSquare.inEditMode)
        {
            GetComponent<Image>().sprite = toggledOffSprite;
        } else
        {
            GetComponent<Image>().sprite = toggledOnSprite;
        }
        GameEvents.UpdateEditModeMethod();
    }

    public void OnSubmit(BaseEventData eventData)
    {

    }
}
