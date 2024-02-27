using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EffectHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Combat CombatScript;
    public bool player, curse;
    public int enemy, order;

    public void OnPointerEnter(PointerEventData eventData)
    {
        CombatScript.EffectHovered(player, curse, enemy, order);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        CombatScript.Unhovered();
    }
}
