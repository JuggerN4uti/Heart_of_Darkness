using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WeaponHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public PlayerCombat Player;
    public bool equipment;

    public void OnPointerEnter(PointerEventData eventData)
    {
        Player.WeaponHovered(equipment);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Player.Unhovered();
    }
}
