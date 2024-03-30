using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossLoot : MonoBehaviour
{
    [Header("Scripts")]
    public Player PlayerScript;
    public Deck DeckScript;
    public ItemPick ItemPickScript;
    public EquipmentChoice EquipmentScript;

    [Header("Stats")]
    public int picksLeft;
    public int roll;
    public int[] rolls;
    public bool secondEquipment;
    public bool[] taken;

    [Header("UI")]
    public GameObject HUD;
    public GameObject EquipmentChoiceScene, SecondEquipmentObject;
    public GameObject[] LootObjects;
    public Image[] LootIcon;
    public TMPro.TextMeshProUGUI[] LootText;
    public TMPro.TextMeshProUGUI Title;

    [Header("Loot")]
    public Sprite[] LootSprites;
    public string[] LootInfo, LootInfo2;

    public void Open(int prizes)
    {
        picksLeft = prizes;
        Title.text = "Choose 2: (" + picksLeft.ToString("0") + " Left)";
        RollOptions();
        SetLoot();
        HUD.SetActive(true);
    }

    void RollOptions()
    {
        for (int i = 0; i < taken.Length; i++)
        {
            taken[i] = false;
        }
        taken[5] = secondEquipment;
        if (PlayerScript.equipmentLevel == 2)
            taken[8] = true;

        for (int i = 0; i < 6; i++)
        {
            do
            {
                roll = Random.Range(0, LootSprites.Length);
            } while (taken[roll]);
            rolls[i] = roll;
            taken[roll] = true;
        }
    }

    void SetLoot()
    {
        for (int i = 0; i < 6; i++)
        {
            LootObjects[i].SetActive(true);
            LootIcon[i].sprite = LootSprites[rolls[i]];
            LootText[i].text = LootInfo[rolls[i]] + "\n" + LootInfo2[rolls[i]];
        }
    }

    public void TakeLoot(int which)
    {
        LootObjects[which].SetActive(false);
        picksLeft--;
        Title.text = "Choose 2: (" + picksLeft.ToString("0") + " Left)";
        switch (rolls[which])
        {
            case 0:
                PlayerScript.GainHP(5);
                PlayerScript.RestoreHealth(25);
                break;
            case 1:
                PlayerScript.GainMaxSanity(5);
                PlayerScript.RestoreSanity(15);
                break;
            case 2:
                ItemPickScript.RollItems();
                break;
            case 3:
                PlayerScript.GainSilver(120);
                break;
            case 4:
                PlayerScript.GainIron(60);
                break;
            case 5:
                EquipmentScript.RollChoices();
                EquipmentChoiceScene.SetActive(true);
                PlayerScript.secondEquipment = true;
                SecondEquipmentObject.SetActive(true);
                break;
            case 6:
                DeckScript.ShowCardsToForge(2);
                break;
            case 7:
                PlayerScript.RestoreHealth(PlayerScript.MaxHealth);
                break;
            case 8: // unaviable for now
                PlayerScript.equipmentLevel++;
                break;
        }
        if (picksLeft == 0)
            HUD.SetActive(false);
    }
}
