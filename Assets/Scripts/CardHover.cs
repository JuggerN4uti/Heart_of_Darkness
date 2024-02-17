using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Hand Cards;
    public int which;

    public void OnPointerEnter(PointerEventData eventData)
    {
        Cards.CardHovered(which);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Cards.Unhovered();
    }
}
