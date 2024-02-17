using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Display : MonoBehaviour
{
    public TMPro.TextMeshProUGUI Value;
    public Image Icon;

    public void DisplayThis(int value, Sprite sprite)
    {
        Value.text = value.ToString("0");
        Icon.sprite = sprite;
    }
}
