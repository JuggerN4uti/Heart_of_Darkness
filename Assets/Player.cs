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
    public int[] StatValues;
    public int[] DrawbackValues;
    public int unitUnderCommand;
    public bool opened;

    [Header("UI")]
    public GameObject InfoObject;
    public GameObject DeckOpenButton;
    public Image HealthFill;
    public TMPro.TextMeshProUGUI HealthText;

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
        UpdateInfo();
    }

    public void UpdateInfo()
    {
        HealthFill.fillAmount = (Health * 1f) / (StatValues[0] * 1f);
        HealthText.text = Health.ToString("") + "/" + StatValues[0].ToString("");
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
