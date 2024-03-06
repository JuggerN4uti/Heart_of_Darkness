using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArmySelect : MonoBehaviour
{
    [Header("Scripts")]
    public Player PlayerScript;
    public PlayerCombat PlayerCombatScript;
    public CardLibrary Library;
    public UnitChoice[] Units;
    public Story StoryScript;

    [Header("Stats")]
    public int unitsCount;
    public int unitSlots, selectedCount, current;
    public int[] unitsSelected;
    public bool[] selected, slotFilled;

    [Header("UI")]
    public TMPro.TextMeshProUGUI HoveredText;
    public Button ProceedButton;
    public GameObject[] UnitObject;
    public Image[] SelectedUnits, UnitButtons;

    [Header("Sprites")]
    public Sprite BlockedSprite;
    public Sprite FreeSprite;

    public void Start()
    {
        SetUnits();
        UpdateInfo();
    }

    void SetUnits()
    {
        for (int i = 0; i < UnitObject.Length; i++)
        {
            UnitObject[i].SetActive(false);
        }
        for (int i = 0; i < unitsCount; i++)
        {
            UnitObject[i].SetActive(true);
            Units[i].UpdateInfo();
        }
    }

    public void UpdateInfo()
    {
        for (int i = 0; i < unitsCount; i++)
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
        for (int i = 0; i < unitSlots; i++)
        {
            PlayerScript.Units[i] = Units[unitsSelected[i]];
            PlayerCombatScript.UnitObject[i].SetActive(true);
            PlayerCombatScript.UnitSprite[i].sprite = Units[unitsSelected[i]].UnitSprite;
        }
        PlayerScript.unitUnderCommand = unitSlots;
        PlayerScript.GainUnitsStats();

        StoryScript.dialogues[1].CharacterDialogue[0] = Units[unitsSelected[0]].UnitName + ", " + Units[unitsSelected[1]].UnitName + " and I are going in. The rest, stay vigilant.\nIf we are not to return in two hours, inform the King...";
        StoryScript.dialogues[2].CharacterName[1] = Units[unitsSelected[0]].UnitClass + " " + Units[unitsSelected[0]].UnitName;
        StoryScript.dialogues[2].CharacterName[4] = Units[unitsSelected[1]].UnitClass + " " + Units[unitsSelected[1]].UnitName;
        StoryScript.dialogues[2].CharacterName[6] = Units[unitsSelected[1]].UnitClass + " " + Units[unitsSelected[1]].UnitName;
        StoryScript.dialogues[2].CharacterName[8] = Units[unitsSelected[0]].UnitClass + " " + Units[unitsSelected[0]].UnitName;
        StoryScript.dialogues[2].RightCharacterSprite[0] = Units[unitsSelected[0]].UnitSprite;
        StoryScript.dialogues[2].RightCharacterSprite[1] = Units[unitsSelected[0]].UnitSprite;
        StoryScript.dialogues[2].RightCharacterSprite[2] = Units[unitsSelected[0]].UnitSprite;
        StoryScript.dialogues[2].RightCharacterSprite[3] = Units[unitsSelected[0]].UnitSprite;
        StoryScript.dialogues[2].RightCharacterSprite[4] = Units[unitsSelected[1]].UnitSprite;
        StoryScript.dialogues[2].RightCharacterSprite[5] = Units[unitsSelected[1]].UnitSprite;
        StoryScript.dialogues[2].RightCharacterSprite[6] = Units[unitsSelected[1]].UnitSprite;
        StoryScript.dialogues[2].RightCharacterSprite[7] = Units[unitsSelected[1]].UnitSprite;
        StoryScript.dialogues[2].RightCharacterSprite[8] = Units[unitsSelected[0]].UnitSprite;
        StoryScript.dialogues[3].CharacterName[0] = Units[unitsSelected[0]].UnitClass + " " + Units[unitsSelected[0]].UnitName;
        StoryScript.dialogues[3].CharacterName[3] = Units[unitsSelected[1]].UnitClass + " " + Units[unitsSelected[1]].UnitName;
        StoryScript.dialogues[3].RightCharacterSprite[0] = Units[unitsSelected[0]].UnitSprite;
        StoryScript.dialogues[3].RightCharacterSprite[1] = Units[unitsSelected[0]].UnitSprite;
        StoryScript.dialogues[3].RightCharacterSprite[2] = Units[unitsSelected[1]].UnitSprite;
        StoryScript.dialogues[3].RightCharacterSprite[3] = Units[unitsSelected[1]].UnitSprite;
        StoryScript.dialogues[3].RightCharacterSprite[4] = Units[unitsSelected[0]].UnitSprite;
        StoryScript.Fade.StartDarken();
        Invoke("ContinueStory", 0.4f);
    }

    void ContinueStory()
    {
        StoryScript.NewDialogue();
        StoryScript.StoryScene.SetActive(true);
        StoryScript.ArmySelectScene.SetActive(false);
    }


    public void DisplayPerkInfo(int which, int amount)
    {
        switch (which)
        {
            case 0:
                HoveredText.text = "Vitality\nIncreases Max Health of Your Army by " + amount.ToString("0");
                break;
            case 1:
                HoveredText.text = "Bravery\nIncreases Max Sanity of Your Army by " + amount.ToString("0");
                break;
            case 2:
                HoveredText.text = "Common Card\nStart Adventure with " + amount.ToString("0") + " chosen common Card/s";
                break;
            case 3:
                HoveredText.text = "Uncommon Card\nStart Adventure with " + amount.ToString("0") + " chosen uncommon Card/s";
                break;
            case 4:
                HoveredText.text = "Silver\nStart Adventure with " + amount.ToString("0") + " Silver";
                break;
            case 5:
                HoveredText.text = "Shield\nStart each Combat with " + amount.ToString("0") + " Shield";
                break;
            case 6:
                HoveredText.text = "Armor\nStart each Combat with " + amount.ToString("0") + " Armor";
                break;
            case 7:
                HoveredText.text = "Resistance:\nStart each Combat with " + amount.ToString("0") + " Resistance";
                break;
            case 8:
                HoveredText.text = "Strength:\nStart each Combat with " + amount.ToString("0") + " Strength";
                break;
            case 9:
                HoveredText.text = "Dexterity:\nStart each Combat with " + amount.ToString("0") + " Dexterity";
                break;
            case 10:
                HoveredText.text = "Shattered\nStart Adventure with " + amount.ToString("0") + " Sanity Lost";
                break;
        }
    }
}
