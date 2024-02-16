using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EffectHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Combat CombatScript;
    public bool player;
    public int enemy, order;

    public void OnPointerEnter(PointerEventData eventData)
    {
        CombatScript.EffectHovered(player, enemy, order);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        CombatScript.Unhovered();
    }
}
