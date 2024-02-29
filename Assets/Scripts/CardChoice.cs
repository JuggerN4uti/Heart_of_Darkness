using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardChoice : MonoBehaviour
{
    [Header("Scripts")]
    public CardPick AddCard;
    public Deck DeckScript;

    [Header("UI")]
    public GameObject CardChoiceObject;
    public GameObject CardPickObject;

    public void MergeCars()
    {
        DeckScript.ShowCardsToMerge();
    }

    public void CollectACard()
    {
        CardChoiceObject.SetActive(false);
        AddCard.RollCards();
        CardPickObject.SetActive(true);
    }

    public void Skip()
    {
        CardChoiceObject.SetActive(false);
    }
}
