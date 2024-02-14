using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawPile : MonoBehaviour
{
    [Header("Scripts")]
    public DiscardPile CardDiscard;

    [Header("Stats")]
    public int[] CardID;
    public int cardsInPile;
    int roll, card;

    public TMPro.TextMeshProUGUI CardsAmountValue;

    void Start()
    {
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

            for (int i = roll; i < cardsInPile; i++)
            {
                CardID[i] = CardID[i + 1];
            }
            cardsInPile--;
        }

        UpdateInfo();
        return card;
    }

    void ReshuffleDecks()
    {
        for (int i = 0; i < CardDiscard.cardsInPile; i++)
        {
            CardID[i] = CardDiscard.CardID[i];
        }

        cardsInPile = CardDiscard.cardsInPile;
        CardDiscard.cardsInPile = 0;

        UpdateInfo();
        CardDiscard.UpdateInfo();
    }
}
