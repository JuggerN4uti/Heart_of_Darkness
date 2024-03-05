using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SkillHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Scripts")]
    public Perks PerksScript;

    [Header("Stats")]
    public string SkillName;
    public string SkillEffect;

    public void OnPointerEnter(PointerEventData eventData)
    {
        PerksScript.SkillHovered(SkillName, SkillEffect);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        PerksScript.Unhovered();
    }
}
