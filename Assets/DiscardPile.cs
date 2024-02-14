using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiscardPile : MonoBehaviour
{
    public int[] CardID, CardLevel;
    public int cardsInPile;

    public TMPro.TextMeshProUGUI CardsAmountValue;

    void Start()
    {
        UpdateInfo();
    }

    public void UpdateInfo()
    {
        CardsAmountValue.text = cardsInPile.ToString("");
    }

    public void ShuffleIn(int card, int level)
    {
        CardID[cardsInPile] = card;
        CardLevel[cardsInPile] = level;
        cardsInPile++;
        UpdateInfo();
    }
}