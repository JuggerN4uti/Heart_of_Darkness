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
    public int[] StatValues;
    public int[] DrawbackValues;
    public int unitUnderCommand;
    public bool opened;

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
    public TMPro.TextMeshProUGUI[] EffectValueText, CurseValueText;
    int count;

    [Header("UI")]
    public GameObject InfoObject;
    public GameObject DeckOpenButton;
    public Image HealthFill, SanityFill;
    public TMPro.TextMeshProUGUI HealthText, SanityText, SilverText;

    [Header("Sprites")]
    public Sprite[] EffectSprite;
    public Sprite[] CurseSprite;

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
        Health = StatValues[0];
        MaxSanity = 50;
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
                count++;
            }
        }
    }
}
