using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawPile : MonoBehaviour
{
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
}
