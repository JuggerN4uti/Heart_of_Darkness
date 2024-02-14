using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WeaponHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public PlayerCombat Player;

    public void OnPointerEnter(PointerEventData eventData)
    {
        Player.WeaponHovered();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Player.Unhovered();
    }
}
