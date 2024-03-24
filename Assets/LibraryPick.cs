using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LibraryPick : MonoBehaviour
{
    [Header("Scripts")]
    public CardLibrary Library;
    public Deck CardDeck;

    [Header("Stats")]
    public int[] rolledID;
    public int min, max, cards;
    int roll;

    [Header("UI")]
    public GameObject LibraryObject;
    public GameObject[] CardsObject;
    public Image[] CardIcon;
    public TMPro.TextMeshProUGUI[] CardManaCost, CardNameText, CardEffectText;

    public void RollCards()
    {
        SetRange();
        cards = 2;

        for (int i = 0; i < 8; i++)
        {
            roll = Random.Range(min, max);
            rolledID[i] = roll;
        }

        SetCards();
    }

    void SetCards()
    {
        for (int i = 0; i < 8; i++)
        {
            CardsObject[i].SetActive(true);
            CardIcon[i].sprite = Library.Cards[rolledID[i]].CardSprite;
            CardManaCost[i].text = Library.Cards[rolledID[i]].CardManaCost[0].ToString("0");
            CardNameText[i].text = Library.Cards[rolledID[i]].CardName;
            CardEffectText[i].text = Library.Cards[rolledID[i]].CardTooltip[0];
        }
    }

    public void CollectCard(int slot)
    {
        CardDeck.AddACard(rolledID[slot], 0);
        CardsObject[slot].SetActive(false);
        cards--;
        if (cards == 0)
            LibraryObject.SetActive(false);
    }

    void SetRange()
    {
        switch (CardDeck.PlayerScript.Class)
        {
            case 0:
                min = 2;
                max = 2 + Library.lightCards;
                break;
            case 1:
                min = 2 + Library.lightCards;
                max = Library.Cards.Length;
                break;
        }
    }
}
