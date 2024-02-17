using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawPile : MonoBehaviour
{
    [Header("Scripts")]
    public Deck CardDeck;
    public DiscardPile CardDiscard;

    [Header("Stats")]
    public int[] CardID;
    public int[] CardLevel;
    public int cardsInPile;
    int roll, card, level;

    public TMPro.TextMeshProUGUI CardsAmountValue;

    void Start()
    {
        UpdateInfo();
    }

    public void SetDeck()
    {
        for (int i = 0; i < CardDeck.cardsInDeck; i++)
        {
            CardID[i] = CardDeck.CardID[i];
            CardLevel[i] = CardDeck.CardLevel[i];
        }

        cardsInPile = CardDeck.cardsInDeck;

        UpdateInfo();
    }

    void UpdateInfo()
    {
        CardsAmountValue.text = cardsInPile.ToString("");
    }

    public int Draw()
    {
        if (cardsInPile < 1)
            ReshuffleDecks();

        if (cardsInPile > 0)
        {
            roll = Random.Range(0, cardsInPile);
            card = CardID[roll];
            level = CardLevel[roll];

            for (int i = roll; i < cardsInPile; i++)
            {
                CardID[i] = CardID[i + 1];
                CardLevel[i] = CardLevel[i + 1];
            }
            cardsInPile--;
        }

        UpdateInfo();
        return card;
    }

    public int DrawLevel()
    {
        return level;
    }

    void ReshuffleDecks()
    {
        for (int i = 0; i < CardDiscard.cardsInPile; i++)
        {
            CardID[i] = CardDiscard.CardID[i];
            CardLevel[i] = CardDiscard.CardLevel[i];
        }

        cardsInPile = CardDiscard.cardsInPile;
        CardDiscard.cardsInPile = 0;

        UpdateInfo();
        CardDiscard.UpdateInfo();
    }
}
