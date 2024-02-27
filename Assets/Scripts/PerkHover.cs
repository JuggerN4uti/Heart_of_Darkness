using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PerkHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Scripts")]
    public UnitChoice Unit;

    [Header("Stats")]
    public int order;

    public void OnPointerEnter(PointerEventData eventData)
    {
        Unit.EffectHovered(order);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Unit.Unhovered();
    }
}
