using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerInfoHovered : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Player PlayerScript;
    public bool curse;
    public int order;

    public void OnPointerEnter(PointerEventData eventData)
    {
        PlayerScript.InfoHovered(curse, order);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        PlayerScript.Unhovered();
    }
}
