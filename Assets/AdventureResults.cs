using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdventureResults : MonoBehaviour
{
    [Header("Scripts")]
    public Player PlayerScript;
    public Map MapScript;

    [Header("Stats")]
    public int SkillPoints, SanityLost;
    public int[] UnitSanityLost;
    public float SkillPointCharge;
    int tempi, roll, current;
    float temp;

    [Header("Units")]
    public UnitResults[] Units;

    [Header("UI")]
    public GameObject[] UnitObjects;
    public GameObject[] GemObjects;
    public Image[] GemImages;
    public TMPro.TextMeshProUGUI SkillPointValue;
    public TMPro.TextMeshProUGUI[] GemCount;

    [Header("Sprites")]
    public Sprite[] GemSprites;

    public void AdventureComplete(bool won, float danger)
    {
        SkillPoints = 0;
        temp = 1f + danger + MapScript.experience;
        if (won)
            temp += 1f + temp * 0.2f;
        ChargeSkillPoints(temp);
        SkillPointValue.text = "+" + SkillPoints.ToString("0");

        temp *= 4;
        temp /= (4 + PlayerScript.unitUnderCommand);
        AdvanceUnits(temp);

        SanityLost = PlayerScript.SanityLost;
        if (won)
            SanityLost /= 2;

        SetInsanity();

        DisplayGems();
    }

    void ChargeSkillPoints(float charge)
    {
        while (charge > 1.5f + 0.5f * SkillPoints)
        {
            charge -= 1.5f + 0.5f * SkillPoints;
            SkillPoints++;
        }

        SkillPointCharge += charge / (1.5f + 0.5f * SkillPoints);
        if (SkillPointCharge > 1f)
        {
            SkillPointCharge -= 1f;
            SkillPoints++;
        }
    }

    void AdvanceUnits(float amount)
    {
        for (int i = 0; i < 4; i++)
        {
            UnitObjects[i].SetActive(false);
        }
        for (int i = 0; i < PlayerScript.unitUnderCommand; i++)
        {
            Units[i].Unit = PlayerScript.Units[i];
            Units[i].GainExperience(amount);
            UnitObjects[i].SetActive(true);
        }
    }

    void SetInsanity()
    {
        tempi = (SanityLost * 3) / (8 * PlayerScript.unitUnderCommand);
        for (int i = 0; i < PlayerScript.unitUnderCommand; i++)
        {
            UnitSanityLost[i] = tempi;
        }
        SanityLost -= Random.Range(tempi * PlayerScript.unitUnderCommand, (41 * tempi * PlayerScript.unitUnderCommand) / 20);

        while (SanityLost > 0)
        {
            roll = Random.Range(0, PlayerScript.unitUnderCommand);
            UnitSanityLost[roll] += 4 + SanityLost / 8;
            SanityLost -= Random.Range(5 + SanityLost / 8, 8 + SanityLost / 5);
        }

        /*for (int i = 0; i < PlayerScript.unitUnderCommand; i++) potem dodaæ talencik ¿e mniej sanity tracone
        {
            temp = UnitSanityLost[i] * 0.4f;
            if ()
        }*/

        for (int i = 0; i < PlayerScript.unitUnderCommand; i++)
        {
            Units[i].PerksValue[10] = UnitSanityLost[i] - Units[i].PerksValue[10];
            Units[i].UpdateInfo();
        }
    }

    void DisplayGems()
    {

    }

    public void Proceed()
    {
        for (int i = 0; i < GemSprites.Length; i++)
        {
            if (PlayerScript.Gems[i] > 0)
            {
                GemObjects[current].SetActive(true);
                GemImages[current].sprite = GemSprites[i];
                GemCount[current].text = PlayerScript.Gems[i].ToString("0");
                current++;
            }
        }
    }
}
