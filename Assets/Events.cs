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
    public int slimeMergeCost, greenShroomRestore, pinkShroomRestore;

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
                if (PlayerScript.Silver >= 5 && DeckScript.CommonCardsInDeck() > 2)
                    EventButtons[1].interactable = true;
                else EventButtons[1].interactable = false;
                if (PlayerScript.Silver >= 25 && DeckScript.UncommonCardsInDeck() > 2)
                    EventButtons[3].interactable = true;
                else EventButtons[3].interactable = false;
                break;
            case 5:
                greenShroomRestore = (PlayerScript.MaxHealth * 12) / 100;
                pinkShroomRestore = 10 + PlayerScript.MaxSanity / 10;
                EventTexts[2].text = "Restore\n" + greenShroomRestore.ToString("0") + " Health";
                EventTexts[3].text = "Restore\n" + pinkShroomRestore.ToString("0") + " Sanity";
                break;
            case 6:
                if (PlayerScript.CursesCount > 0 && PlayerScript.Silver >= 50)
                    EventButtons[2].interactable = true;
                else EventButtons[2].interactable = false;
                break;
        }
        EventObject[roll].SetActive(true);
    }

    public void SlimeMerge()
    {
        PlayerScript.SpendSilver(slimeMergeCost);
        DeckScript.ShowCardsToMerge();
        slimeMergeCost += 9;
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

    public void ForgeUpgrade(bool common)
    {
        if (common)
        {
            PlayerScript.SpendSilver(5);
            CardPickScript.RollForge();
            CardEventObject.SetActive(true);
        }
        else
        {
            PlayerScript.SpendSilver(25);
            CardPickScript.RollForge(true);
            CardEventObject.SetActive(true);
        }
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

    public void CleanseCurse()
    {
        PlayerScript.SpendSilver(50);
        PlayerScript.RemoveCurse();
    }
}
