using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitChoice : MonoBehaviour
{
    [Header("Scripts")]
    public ArmySelect Army;

    [Header("Other Info")]
    public string UnitClass;
    public string UnitName;

    [Header("Perks")]
    public int[] PerksValue;
    public int[] ID;
    public float experience;
    int current;

    [Header("Hidden Talents")]
    public int[] talentsValue;

    [Header("UI")]
    public TMPro.TextMeshProUGUI UnitTitle;
    public GameObject[] PerksObject;
    public Image[] PerkIcon;
    public TMPro.TextMeshProUGUI[] PerkValueText;

    [Header("Sprites")]
    public Sprite[] PerkSprites;
    public Sprite UnitSprite, UnitMiniSprite;

    public void Start()
    {
        //UpdateInfo();
    }

    public void UpdateInfo()
    {
        UnitTitle.text = UnitClass + " " + UnitName;

        for (int i = 0; i < PerksValue.Length; i++)
        {
            if (PerksValue[i] > 0)
            {
                PerksObject[current].SetActive(true);
                PerkIcon[current].sprite = PerkSprites[i];
                PerkValueText[current].text = PerksValue[i].ToString("0");
                ID[current] = i;
                current++;
            }
        }
    }

    public void EffectHovered(int order)
    {
        Army.DisplayPerkInfo(ID[order], PerksValue[ID[order]]);
    }

    public void Unhovered()
    {
        Army.HoveredText.text = "";
    }
}
