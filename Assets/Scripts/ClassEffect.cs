using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClassEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Combat CombatScript;
    public int classEffect;

    public void OnPointerEnter(PointerEventData eventData)
    {
        CombatScript.ClassEffectHovered(classEffect);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        CombatScript.Unhovered();
    }
}
