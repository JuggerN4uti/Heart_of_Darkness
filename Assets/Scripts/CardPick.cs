using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardPick : MonoBehaviour
{
    [Header("Scripts")]
    public CardLibrary Library;
    public Deck CardDeck;

    [Header("Stats")]
    public int[] rolledID;
    public int cardsRarity, min, max;
    public bool forge, viable;
    int roll;

    [Header("UI")]
    public GameObject CardPickObject;
    public Image[] CardRarityImage, CardIcon;
    public TMPro.TextMeshProUGUI[] CardManaCost, CardNameText, CardEffectText;

    public void RollCards(int rarity = 0)
    {
        SetRange();

        roll = Random.Range(min, max);
        rolledID[0] = roll;

        do
        {
            roll = Random.Range(min, max);
        } while (roll == rolledID[0]);
        rolledID[1] = roll;

        do
        {
            roll = Random.Range(min, max);
        } while (roll == rolledID[0] || roll == rolledID[1]);
        rolledID[2] = roll;

        cardsRarity = rarity;
        SetCards();
    }

    public void RollForge()
    {
        viable = false;
        do
        {
            roll = Random.Range(0, CardDeck.cardsInDeck);
            if (CardDeck.CardLevel[roll] == 0)
                viable = true;
        } while (!viable);
        rolledID[0] = roll;

        viable = false;
        do
        {
            roll = Random.Range(0, CardDeck.cardsInDeck);
            if (CardDeck.CardLevel[roll] == 0 && roll != rolledID[0])
                viable = true;
        } while (!viable);
        rolledID[1] = roll;

        viable = false;
        do
        {
            roll = Random.Range(0, CardDeck.cardsInDeck);
            if (CardDeck.CardLevel[roll] == 0 && roll != rolledID[0] && roll != rolledID[1])
                viable = true;
        } while (!viable);
        rolledID[2] = roll;

        cardsRarity = 0;
        SetForgeCards();
    }

    void SetCards()
    {
        for (int i = 0; i < 3; i++)
        {
            CardIcon[i].sprite = Library.Cards[rolledID[i]].CardSprite;
            CardRarityImage[i].sprite = Library.CardLevel[cardsRarity];
            CardManaCost[i].text = Library.Cards[rolledID[i]].CardManaCost[cardsRarity].ToString("0");
            CardNameText[i].text = Library.Cards[rolledID[i]].CardName;
            CardEffectText[i].text = Library.Cards[rolledID[i]].CardTooltip[cardsRarity];
        }
    }

    void SetForgeCards()
    {
        for (int i = 0; i < 3; i++)
        {
            CardIcon[i].sprite = Library.Cards[CardDeck.CardID[rolledID[i]]].CardSprite;
            CardRarityImage[i].sprite = Library.CardLevel[0];
            CardManaCost[i].text = Library.Cards[CardDeck.CardID[rolledID[i]]].CardManaCost[0].ToString("0");
            CardNameText[i].text = Library.Cards[CardDeck.CardID[rolledID[i]]].CardName;
            CardEffectText[i].text = Library.Cards[CardDeck.CardID[rolledID[i]]].CardTooltip[0];
        }
    }

    public void CollectCard(int slot)
    {
        if (forge)
        {
            CardDeck.CardLevel[rolledID[slot]]++;
            CardDeck.PlayerScript.UpdateInfo();
        }
        else CardDeck.AddACard(rolledID[slot], cardsRarity);
        CardPickObject.SetActive(false);
    }

    public void SkipChoice()
    {
        CardPickObject.SetActive(false);
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
