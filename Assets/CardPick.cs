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
    public int cardsRarity;
    int roll;

    [Header("UI")]
    public GameObject CardPickObject;
    public Image[] CardRarityImage, CardIcon;
    public TMPro.TextMeshProUGUI[] CardManaCost, CardNameText, CardEffectText;

    public void RollCards(int rarity = 0)
    {
        roll = Random.Range(2, Library.Cards.Length);
        rolledID[0] = roll;

        do
        {
            roll = Random.Range(2, Library.Cards.Length);
        } while (roll == rolledID[0]);
        rolledID[1] = roll;

        do
        {
            roll = Random.Range(2, Library.Cards.Length);
        } while (roll == rolledID[0] || roll == rolledID[1]);
        rolledID[2] = roll;

        cardsRarity = rarity;
        SetCards();
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

    public void CollectCard(int slot)
    {
        CardDeck.AddACard(rolledID[slot], cardsRarity);
        CardPickObject.SetActive(false);
    }

    public void SkipChoice()
    {
        CardPickObject.SetActive(false);
    }
}
