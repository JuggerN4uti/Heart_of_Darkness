using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitChoice : MonoBehaviour
{
    [Header("Scripts")]
    public ArmySelect Army;
    public CardLibrary Library;

    [Header("Other Info")]
    public int UnitLevel;
    public string UnitClass, UnitName;

    [Header("Abilities, Perks & Flaws")]
    public int AbilitiesAmount;
    public int PerksAmount, FlawsAmount;
    public int[] Abilities, Perks, Flaws;
    public int[] AbilitiesLevel, PerksValue, FlawsValue;

    [Header("UI")]
    public TMPro.TextMeshProUGUI UnitTitle;
    public GameObject[] AbilityObject, PerksObject, FlawsObject;
    public Image[] AbilityCard, AbilityIcon, PerkIcon, FlawsIcon;
    public TMPro.TextMeshProUGUI[] PerkValueText, FlawsValueText;

    [Header("Sprites")]
    public Sprite[] PerkSprites;
    public Sprite[] FlawsSprites;
    public Sprite UnitSprite, UnitMiniSprite;

    public void Start()
    {
        UpdateInfo();
    }

    public void UpdateInfo()
    {
        UnitTitle.text = UnitClass + " " + UnitName + " - Level " + UnitLevel.ToString("0");

        // Abilities
        for (int i = 0; i < 3; i++)
        {
            AbilityObject[i].SetActive(false);
        }
        for (int i = 0; i < AbilitiesAmount; i++)
        {
            AbilityObject[i].SetActive(true);
            AbilityCard[i].sprite = Library.CardLevel[AbilitiesLevel[i]];
            AbilityIcon[i].sprite = Library.Cards[Abilities[i]].CardSprite;
        }

        // Perks
        for (int i = 0; i < 3; i++)
        {
            PerksObject[i].SetActive(false);
        }
        for (int i = 0; i < PerksAmount; i++)
        {
            PerksObject[i].SetActive(true);
            PerkIcon[i].sprite = PerkSprites[Perks[i]];
            PerkValueText[i].text = PerksValue[i].ToString("0");
        }

        // Flaws
        for (int i = 0; i < 3; i++)
        {
            FlawsObject[i].SetActive(false);
        }
        for (int i = 0; i < FlawsAmount; i++)
        {
            FlawsObject[i].SetActive(true);
            FlawsIcon[i].sprite = FlawsSprites[Flaws[i]];
            FlawsValueText[i].text = FlawsValue[i].ToString("0");
        }
    }

    public void EffectHovered(bool ability, bool perk, int order)
    {
        if (ability)
        {
            Army.DisplayCardInfo(Abilities[order], AbilitiesLevel[order]);
        }
        else
        {
            if (perk)
                Army.DisplayPerkInfo(Perks[order], PerksValue[order]);
            else Army.DisplayFlawInfo(Flaws[order], FlawsValue[order]);
        }
    }

    public void Unhovered()
    {
        Army.HoveredText.text = "";
    }
}
