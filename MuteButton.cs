using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/* MuteButton
 * 
 * Attached to each of the two mute buttons. Handles muting and unmuting of music
 */
public class MuteButton : Selectable, IPointerClickHandler, IPointerUpHandler, IPointerExitHandler
{
    public Sprite mutedSprite;
    public Sprite unmutedSprite;



    void Start()
    {
        if (MenuButtons.muteMusic)
        {
            GetComponent<Image>().sprite = mutedSprite;
            
        } else
        {
            GetComponent<Image>().sprite = unmutedSprite;
            
        }
        
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        if (MenuButtons.muteMusic) // if currently muted
        {
            GetComponent<Image>().sprite = unmutedSprite;
            MenuButtons.muteMusic = false;
            MenuButtons.gameMusic.Play();
        }
        else // if not currently muted
        {
            GetComponent<Image>().sprite = mutedSprite;
            MenuButtons.muteMusic = true;
            MenuButtons.gameMusic.Stop();
        }
        
    }
}
