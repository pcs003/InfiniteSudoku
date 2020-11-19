using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/* EventSystemScript
 * 
 * Prevents gridSquares from being deselected if the player clicks/touches
 * anything other than a different gridSquare. 
 */
public class EventSystemScript : MonoBehaviour
{
    private EventSystem eventSystem;
    private GameObject lastSelected = null;

    void Start()
    {
        eventSystem = GetComponent<EventSystem>();
    }

    void Update()
    {
        if (eventSystem != null)
        {
            if (eventSystem.currentSelectedGameObject != null) // if selecting a valid gameobject
            {
                if (eventSystem.currentSelectedGameObject.tag == "GridSquare")
                {
                    lastSelected = eventSystem.currentSelectedGameObject;
                } else
                {
                    eventSystem.SetSelectedGameObject(lastSelected);
                }
                
            }
            else // if selecting nothing
            {
                
                eventSystem.SetSelectedGameObject(lastSelected);
            }
        }
    }
}
