using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hand : MonoBehaviour
{
    [Header("Scripts")]
    public PlayerCombat Player;
    public CardLibrary Library;
    public DrawPile CardDraw;
    public DiscardPile CardDiscard;

    [Header("UI")]
    public RectTransform CardsPosition;
    public GameObject[] Card;
    public Image[] CardIcon;
    public Button[] CardButton;
    public TMPro.TextMeshProUGUI[] CardManaCostValue;

    [Header("Card Details")]
    public GameObject TheCard;
    public Image TheCardIcon;
    public TMPro.TextMeshProUGUI TheCardName, TheCardCost, TheCardEffect;

    [Header("Stats")]
    public int[] CardsID;
    public int CardsInHand;

    void Start()
    {
        //Draw(5);
    }

    void UpdateInfo()
    {
        for (int i = 0; i < 10; i++)
        {
            Card[i].SetActive(false);
        }
        for (int i = 0; i < CardsInHand; i++)
        {
            Card[i].SetActive(true);
            CardIcon[i].sprite = Library.Cards[CardsID[i]].CardSprite;
            CardManaCostValue[i].text = Library.Cards[CardsID[i]].CardManaCost.ToString("");
            if (Player.mana >= Library.Cards[CardsID[i]].CardManaCost)
                CardButton[i].interactable = true;
            else CardButton[i].interactable = false;
        }
        CardsPosition.position = new Vector2((280f - 28f * CardsInHand)/45f, -5f);
    }

    public void Draw(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            CardsID[CardsInHand] = CardDraw.Draw();
            CardsInHand++;
        }
        UpdateInfo();
    }

    public void CardHovered(int which)
    {
        TheCard.SetActive(true);
        TheCardIcon.sprite = Library.Cards[CardsID[which]].CardSprite;
        TheCardName.text = Library.Cards[CardsID[which]].CardName;
        TheCardCost.text = Library.Cards[CardsID[which]].CardManaCost.ToString("");
        TheCardEffect.text = Player.AbilityInfo(CardsID[which]);
    }

    public void Unhovered()
    {
        TheCard.SetActive(false);
    }

    public void PlayCard(int which)
    {
        CardDiscard.ShuffleIn(CardsID[which]);

        Player.UseAbility(CardsID[which]);

        for (int i = which; i < CardsInHand; i++)
        {
            CardsID[i] = CardsID[i + 1];
        }
        CardsInHand--;

        UpdateInfo();
        Unhovered();
    }
}
