using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArmySelect : MonoBehaviour
{
    [Header("Scripts")]
    public CardLibrary Library;
    public UnitChoice[] Units;

    [Header("Stats")]
    public int unitSlots;
    public int selectedCount, current;
    public int[] unitsSelected;
    public bool[] selected, slotFilled;

    [Header("UI")]
    public TMPro.TextMeshProUGUI HoveredText;
    public Button ProceedButton;
    public Image[] SelectedUnits, UnitButtons;
    public string[] cardLevelsNames;

    [Header("Sprites")]
    public Sprite BlockedSprite;
    public Sprite FreeSprite;

    public void Start()
    {
        UpdateInfo();
    }

    public void UpdateInfo()
    {
        for (int i = 0; i < 4; i++)
        {
            if (i >= unitSlots)
                SelectedUnits[i].sprite = BlockedSprite;
            else SelectedUnits[i].sprite = FreeSprite;

            if (selected[i])
                UnitButtons[i].color = new Color(0.5f, 1f, 0.5f, 1f);
            else UnitButtons[i].color = new Color(1f, 1f, 1f, 1f);

            if (slotFilled[i])
            {
                SelectedUnits[i].sprite = Units[unitsSelected[i]].UnitMiniSprite;
            }

            if (selectedCount == unitSlots)
                ProceedButton.interactable = true;
            else ProceedButton.interactable = false;
        }
    }

    public void SelectUnit(int which)
    {
        if (selected[which])
        {
            UnSelect(which);
        }
        else
        {
            Select(which);
        }

        UpdateInfo();
    }

    void UnSelect(int which)
    {
        for (int i = 0; i < 4; i++)
        {
            if (unitsSelected[i] == which && slotFilled[i])
            {
                current = i;
                slotFilled[i] = false;
            }
        }
        selected[which] = false;
        selectedCount--;
    }

    void Select(int which)
    {
        if (selectedCount < unitSlots)
        {
            unitsSelected[current] = which;
            slotFilled[current] = true;
            selected[which] = true;
            selectedCount++;
            if (selectedCount < unitSlots)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (!slotFilled[i])
                    {
                        current = i;
                        break;
                    }
                }
            }
        }
        else
        {
            selected[unitsSelected[current]] = false;
            unitsSelected[current] = which;
            slotFilled[current] = true;
            selected[which] = true;
        }
    }

    public void Proceed()
    {

    }

    public void DisplayCardInfo(int card, int level)
    {
        HoveredText.text = cardLevelsNames[level] + " '" + Library.Cards[card].CardName + "' - " + Library.Cards[card].CardManaCost[level].ToString("0") + " Mana\n" + Library.Cards[card].CardTooltip[level];
    }

    public void DisplayPerkInfo(int which, int amount)
    {
        switch (which)
        {
            case 0:
                HoveredText.text = "Vitality\nIncreases Max Health of Your Army by " + amount.ToString("0");
                break;
            case 1:
                HoveredText.text = "Resilience\nRestores " + amount.ToString("0") + " Health after each Combat";
                break;
            case 2:
                HoveredText.text = "Shield\nStart each Combat with " + amount.ToString("0") + " Shield";
                break;
            case 3:
                HoveredText.text = "Armor\nGives " + amount.ToString("0") + " Block at the end of each Turn during Combat";
                break;
            case 4:
                HoveredText.text = "Strength:\nIncrease Damage Dealt by " + amount.ToString("0");
                break;
            case 5:
                HoveredText.text = "Resistance:\nIncrease Block Gained by " + amount.ToString("0");
                break;
            case 6:
                HoveredText.text = "Dexterity:\nIncrease Energy Gained by " + amount.ToString("0");
                break;
        }
    }

    public void DisplayFlawInfo(int which, int amount)
    {
        switch (which)
        {
            case 0:
                HoveredText.text = "Tired\nReduce Mana gained each Turn by 1 for first " + amount.ToString("0") + " Turns";
                break;
            case 1:
                HoveredText.text = "Injured\nStart Combat with " + amount.ToString("0") + " Bleed";
                break;
        }
    }
}
