using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PerkHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Scripts")]
    public UnitChoice Unit;
    public UnitResults Unit2;

    [Header("Stats")]
    public int order;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (Unit)
            Unit.EffectHovered(order);
        else Unit2.EffectHovered(order);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (Unit)
            Unit.Unhovered();
        else Unit2.Unhovered();
    }
}
