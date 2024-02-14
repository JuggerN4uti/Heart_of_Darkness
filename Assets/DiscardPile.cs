using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiscardPile : MonoBehaviour
{
    public int[] CardID;
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

    public void ShuffleIn(int card)
    {
        CardID[cardsInPile] = card;
        cardsInPile++;
        UpdateInfo();
    }
}