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
    public int Sanity, MaxSanity;
    public int[] StatValues;
    public int[] DrawbackValues;
    public int unitUnderCommand;
    public bool opened;

    [Header("Curses")]
    public int CursesCount;
    public int[] CurseValue;
    public string[] CurseName;

    [Header("UI")]
    public GameObject InfoObject;
    public GameObject DeckOpenButton;
    public Image HealthFill, SanityFill;
    public TMPro.TextMeshProUGUI HealthText, SanityText;

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

    void DisplayPlayerInfo()
    {

    }
}
