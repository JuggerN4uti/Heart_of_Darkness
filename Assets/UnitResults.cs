using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitResults : MonoBehaviour
{
    [Header("Scripts")]
    public UnitChoice Unit;

    [Header("Perks")]
    public int[] PerksValue;
    public int[] ID;
    public float experience;

    [Header("UI")]
    public TMPro.TextMeshProUGUI UnitTitle;
    public GameObject[] PerksObject;
    public Image UnitImage;
    public Image[] PerkIcon;
    public TMPro.TextMeshProUGUI HoveredText;
    public TMPro.TextMeshProUGUI[] PerkValueText;

    [Header("Sprites")]
    public Sprite[] PerkSprites;

    [Header("Stats")]
    public int[] StatsWeights;
    public float[] ExperienceCosts;
    int current;

    public void UpdateInfo()
    {
        UnitTitle.text = Unit.UnitClass + " " + Unit.UnitName;
        UnitImage.sprite = Unit.UnitSprite;

        current = 0;
        for (int i = 0; i < PerksValue.Length; i++)
        {
            if (PerksValue[i] > 0)
            {
                PerksObject[current].SetActive(true);
                PerkIcon[current].sprite = PerkSprites[i];
                PerkValueText[current].text = "+" + PerksValue[i].ToString("0");
                ID[current] = i;
                current++;
            }
        }
    }

    public void GainExperience(float amount)
    {
        experience += amount * (1f + Unit.talentsValue[0]);
        while (experience > 0f)
        {
            if (Random.Range(0, 5) > 1)
                RollFromKnown(true);
            else RollFromKnown(false);
        }
    }

    void RollFromKnown(bool yes)
    {
        current = Random.Range(0, 655);
        for (int i = 0; i < 10; i++)
        {
            if (current <= StatsWeights[i])
            {
                if (yes && Unit.PerksValue[i] > 0)
                    GainStat(i);
                else GainStat(i);
                break;
            }
            else current -= StatsWeights[i];
        }
    }

    void GainStat(int which)
    {
        if (which == 3)
        {
            if (Unit.PerksValue[2] + PerksValue[2] > PerksValue[which])
            {
                PerksValue[which]++;
                experience -= ExperienceCosts[which];
            }
        }
        PerksValue[which]++;
        experience -= ExperienceCosts[which];
    }

    public void EffectHovered(int order)
    {
        switch (ID[order])
        {
            case 0:
                HoveredText.text = "Vitality\nIncreases Max Health of Your Army by " + PerksValue[ID[order]].ToString("0");
                break;
            case 1:
                HoveredText.text = "Bravery\nIncreases Max Sanity of Your Army by " + PerksValue[ID[order]].ToString("0");
                break;
            case 2:
                HoveredText.text = "Common Card\nStart Adventure with " + PerksValue[ID[order]].ToString("0") + " chosen common Card/s";
                break;
            case 3:
                HoveredText.text = "Uncommon Card\nStart Adventure with " + PerksValue[ID[order]].ToString("0") + " chosen uncommon Card/s";
                break;
            case 4:
                HoveredText.text = "Silver\nStart Adventure with " + PerksValue[ID[order]].ToString("0") + " Silver";
                break;
            case 5:
                HoveredText.text = "Shield\nStart each Combat with " + PerksValue[ID[order]].ToString("0") + " Shield";
                break;
            case 6:
                HoveredText.text = "Armor\nStart each Combat with " + PerksValue[ID[order]].ToString("0") + " Armor";
                break;
            case 7:
                HoveredText.text = "Resistance:\nStart each Combat with " + PerksValue[ID[order]].ToString("0") + " Resistance";
                break;
            case 8:
                HoveredText.text = "Strength:\nStart each Combat with " + PerksValue[ID[order]].ToString("0") + " Strength";
                break;
            case 9:
                HoveredText.text = "Dexterity:\nStart each Combat with " + PerksValue[ID[order]].ToString("0") + " Dexterity";
                break;
            case 10:
                HoveredText.text = "Shattered\nStart Adventure with " + PerksValue[ID[order]].ToString("0") + " Sanity Lost";
                break;
        }
    }

    public void Unhovered()
    {
        HoveredText.text = "";
    }
}
