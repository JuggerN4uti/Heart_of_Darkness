using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [Header("Scripts")]
    public Deck DeckScript;
    public UnitChoice[] Units;

    [Header("Stats")]
    public int Health;
    public int Sanity, MaxSanity, Silver;
    public int[] StatValues, DrawbackValues, EffectID, CurseID;
    public int unitUnderCommand;
    public bool opened, map;
    public bool[] virtue;

    [Header("Curses")]
    public int CursesCount;
    public int[] CurseValue;
    public string[] CurseName;

    [Header("Weapon")]
    public int weaponLevel;
    public int weaponDamage, weaponStrengthBonus, weaponEnergyRequirement;

    [Header("Info")]
    public TMPro.TextMeshProUGUI[] WeaponInfoText;
    public GameObject[] EffectObject, CurseObject;
    public Image[] EffectIcon, CurseIcon;
    public TMPro.TextMeshProUGUI HoveredText;
    public TMPro.TextMeshProUGUI[] EffectValueText, CurseValueText;
    int count;

    [Header("UI")]
    public GameObject InfoObject;
    public GameObject DeckOpenButton, MapInfo;
    public Image HealthFill, SanityFill;
    public TMPro.TextMeshProUGUI HealthText, SanityText, SilverText;

    [Header("Sprites")]
    public Sprite[] EffectSprite;
    public Sprite[] DrawbackSprite, CurseSprite;

    public void GainUnitsStats()
    {
        for (int j = 0; j < unitUnderCommand; j++)
        {
            for (int i = 0; i < Units[j].AbilitiesAmount; i++)
            {
                DeckScript.AddACard(Units[j].Abilities[i], Units[j].AbilitiesLevel[i]);
            }
            for (int i = 0; i < Units[j].PerksAmount; i++)
            {
                StatValues[Units[j].Perks[i]] += Units[j].PerksValue[i];
            }
            for (int i = 0; i < Units[j].FlawsAmount; i++)
            {
                DrawbackValues[Units[j].Flaws[i]] += Units[j].FlawsValue[i];
            }
        }
        MaxSanity += StatValues[7];
        Health = StatValues[0];
        Sanity = MaxSanity;
        UpdateInfo();
    }

    public void UpdateInfo()
    {
        HealthFill.fillAmount = (Health * 1f) / (StatValues[0] * 1f);
        HealthText.text = Health.ToString("") + "/" + StatValues[0].ToString("");

        SanityFill.fillAmount = (Sanity * 1f) / (MaxSanity * 1f);
        SanityText.text = Sanity.ToString("") + "/" + MaxSanity.ToString("");

        SilverText.text = Silver.ToString("");
    }

    public void ShowInfo()
    {
        if (opened)
        {
            opened = false;
            InfoObject.SetActive(false);
            DeckOpenButton.SetActive(true);
        }
        else
        {
            opened = true;
            DisplayPlayerInfo();
            InfoObject.SetActive(true);
            DeckOpenButton.SetActive(false);
        }
    }

    public void ShowMap()
    {
        if (!map)
        {
            MapInfo.SetActive(true);
            map = true;
        }
        else
        {
            MapInfo.SetActive(false);
            map = false;
        }
    }

    public void RestoreHealth(int amount)
    {
        Health += amount;
        if (Health > StatValues[0])
            Health = StatValues[0];
        UpdateInfo();
    }

    public void RestoreSanity(int amount)
    {
        Sanity += amount;
        UpdateInfo();
    }

    public void GainSilver(int amount)
    {
        Silver += amount;
        UpdateInfo();
    }

    public void SpendSilver(int amount)
    {
        Silver -= amount;
        UpdateInfo();
    }

    void DisplayPlayerInfo()
    {
        WeaponInfoText[0].text = weaponDamage.ToString("");
        WeaponInfoText[1].text = "+" + weaponStrengthBonus.ToString("") + "/";
        WeaponInfoText[2].text = weaponEnergyRequirement.ToString("");

        count = 0;
        for (int i = 1; i < StatValues.Length; i++)
        {
            if (StatValues[i] > 0)
            {
                EffectObject[count].SetActive(true);
                EffectIcon[count].sprite = EffectSprite[i];
                EffectValueText[count].text = StatValues[i].ToString("");
                EffectID[count] = i;
                virtue[count] = true;
                count++;
            }
        }
        for (int i = 0; i < DrawbackValues.Length; i++)
        {
            if (DrawbackValues[i] > 0)
            {
                EffectObject[count].SetActive(true);
                EffectIcon[count].sprite = DrawbackSprite[i];
                EffectValueText[count].text = DrawbackValues[i].ToString("");
                EffectID[count] = i;
                virtue[count] = false;
                count++;
            }
        }

        count = 0;
        for (int i = 0; i < CurseValue.Length; i++)
        {
            if (CurseValue[i] > 0)
            {
                CurseObject[count].SetActive(true);
                CurseIcon[count].sprite = CurseSprite[i];
                CurseValueText[count].text = CurseValue[i].ToString("");
                CurseID[count] = i;
                count++;
            }
        }
    }

    public void InfoHovered(bool curse, int order)
    {
        if (curse)
        {
            switch (CurseID[order])
            {
                case 0:
                    HoveredText.text = "Doubt:\nGain " + (2 * CurseValue[CurseID[order]]).ToString("0") + " Weak. Weak is more effective";
                    break;
                case 1:
                    HoveredText.text = "Madness:\nAt the end of each Turn take " + (4 * CurseValue[CurseID[order]]).ToString("0") + " Damage for every Card left in your hand";
                    break;
                case 2:
                    HoveredText.text = "Pride:\nEnemies gain " + (2 * CurseValue[CurseID[order]]).ToString("0") + " Strength. Each Turn enemies gain " + CurseValue[CurseID[order]].ToString("0") + " Strength";
                    break;
                case 3:
                    HoveredText.text = "Fear:\nGain " + (20 * CurseValue[CurseID[order]]).ToString("0") + "% Card draw skip. Taking unblocked Damage also reduces Sanity";
                    break;
                case 4:
                    HoveredText.text = "Frailty:\nGain " + (2 * CurseValue[CurseID[order]]).ToString("0") + " Frail. Frail is more effective";
                    break;
            }
        }
        else
        {
            if (virtue[order])
            {
                switch (EffectID[order])
                {
                    case 1:
                        HoveredText.text = "Resilience\nRestores " + StatValues[EffectID[order]].ToString("0") + " Health after each Combat";
                        break;
                    case 2:
                        HoveredText.text = "Shield\nStart each Combat with " + StatValues[EffectID[order]].ToString("0") + " Shield";
                        break;
                    case 3:
                        HoveredText.text = "Armor\nGives " + StatValues[EffectID[order]].ToString("0") + " Block at the end of each Turn during Combat";
                        break;
                    case 4:
                        HoveredText.text = "Strength:\nIncrease Damage Dealt by " + StatValues[EffectID[order]].ToString("0");
                        break;
                    case 5:
                        HoveredText.text = "Resistance:\nIncrease Block Gained by " + StatValues[EffectID[order]].ToString("0");
                        break;
                    case 6:
                        HoveredText.text = "Dexterity:\nIncrease Energy Gained by " + StatValues[EffectID[order]].ToString("0");
                        break;
                    case 7:
                        HoveredText.text = "Brave\nIncreases Max Sanity of Your Army by " + StatValues[EffectID[order]].ToString("0");
                        break;
                }
            }
            else
            {
                switch (EffectID[order])
                {
                    case 0:
                        HoveredText.text = "Sluggish\nReduce Mana gained each Turn by 1 for first " + DrawbackValues[EffectID[order]].ToString("0") + " Turns";
                        break;
                    case 1:
                        HoveredText.text = "Injured\nStart Combat with " + DrawbackValues[EffectID[order]].ToString("0") + " Bleed";
                        break;
                }
            }
        }
    }

    public void Unhovered()
    {
        HoveredText.text = "";
    }
}
