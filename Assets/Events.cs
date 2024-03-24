using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Events : MonoBehaviour
{
    [Header("Scripts")]
    public Player PlayerScript;
    public Deck DeckScript;
    public CardPick CardPickScript;

    [Header("Stats")]
    public int roll;
    public int slimeMergeCost, forgeUpgradeCost, greenShroomRestore, pinkShroomRestore;

    [Header("UI")]
    public GameObject[] EventObject;
    public GameObject CardEventObject;
    public TMPro.TextMeshProUGUI[] EventTexts;
    public Button[] EventButtons;

    public void EnterEvent()
    {
        roll = Random.Range(0, EventObject.Length);
        switch (roll)
        {
            case 0:
                slimeMergeCost = 0;
                EventTexts[0].text = slimeMergeCost.ToString("0") + " Silver";
                EventButtons[0].interactable = true;
                break;
            case 3:
                forgeUpgradeCost = 4;
                EventTexts[1].text = forgeUpgradeCost.ToString("0") + " Silver";
                if (PlayerScript.Silver >= forgeUpgradeCost && DeckScript.CommonCardsInDeck() > 2)
                    EventButtons[1].interactable = true;
                else EventButtons[1].interactable = false;
                break;
            case 5:
                greenShroomRestore = (PlayerScript.MaxHealth * 12) / 100;
                pinkShroomRestore = 10 + PlayerScript.MaxSanity / 10;
                EventTexts[2].text = "Restore\n" + greenShroomRestore.ToString("0") + " Health";
                EventTexts[3].text = "Restore\n" + pinkShroomRestore.ToString("0") + " Sanity";
                break;
        }
        EventObject[roll].SetActive(true);
    }

    public void SlimeMerge()
    {
        PlayerScript.SpendSilver(slimeMergeCost);
        DeckScript.ShowCardsToMerge();
        slimeMergeCost += 10;
        EventTexts[0].text = slimeMergeCost.ToString("0") + " Silver";
        if (PlayerScript.Silver >= slimeMergeCost)
            EventButtons[0].interactable = true;
        else EventButtons[0].interactable = false;
    }

    public void ThrowIntoFlame()
    {
        DeckScript.RemoveCard(true);
    }

    public void Duplicate()
    {
        DeckScript.DuplicateCard();
    }

    public void ForgeUpgrade()
    {
        PlayerScript.SpendSilver(forgeUpgradeCost);
        CardPickScript.RollForge();
        CardEventObject.SetActive(true);
        forgeUpgradeCost += 12;
        EventTexts[1].text = forgeUpgradeCost.ToString("0") + " Silver";
        if (PlayerScript.Silver >= forgeUpgradeCost && DeckScript.CommonCardsInDeck() > 2)
            EventButtons[1].interactable = true;
        else EventButtons[1].interactable = false;
    }

    public void EatShroom(int which)
    {
        switch (which)
        {
            case 0:
                PlayerScript.RestoreHealth(greenShroomRestore);
                break;
            case 1:
                PlayerScript.GainHP(6);
                break;
            case 2:
                PlayerScript.RestoreSanity(pinkShroomRestore);
                break;
        }    
    }
}
