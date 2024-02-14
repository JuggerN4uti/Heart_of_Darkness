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
    public Image[] CardIcon, CardRarity;
    public Sprite[] CardsSprites;
    public Button[] CardButton;
    public TMPro.TextMeshProUGUI[] CardManaCostValue;

    [Header("Card Details")]
    public GameObject TheCard;
    public Image TheCardIcon, TheCardRarity;
    public TMPro.TextMeshProUGUI TheCardName, TheCardCost, TheCardEffect;

    [Header("Stats")]
    public int CardsInHand;
    public int[] CardsID, CardsLevel;

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
            CardRarity[i].sprite = CardsSprites[CardsLevel[i]];
            CardManaCostValue[i].text = Library.Cards[CardsID[i]].CardManaCost[CardsLevel[i]].ToString("");
            if (Player.mana >= Library.Cards[CardsID[i]].CardManaCost[CardsLevel[i]])
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
            CardsLevel[CardsInHand] = CardDraw.DrawLevel();
            CardsInHand++;
        }
        UpdateInfo();
    }

    public void CardHovered(int which)
    {
        TheCard.SetActive(true);
        TheCardIcon.sprite = Library.Cards[CardsID[which]].CardSprite;
        TheCardRarity.sprite = CardsSprites[CardsLevel[which]];
        TheCardName.text = Library.Cards[CardsID[which]].CardName;
        TheCardCost.text = Library.Cards[CardsID[which]].CardManaCost[CardsLevel[which]].ToString("");
        TheCardEffect.text = Player.AbilityInfo(CardsID[which], CardsLevel[which]);
    }

    public void Unhovered()
    {
        TheCard.SetActive(false);
    }

    public void PlayCard(int which)
    {
        CardDiscard.ShuffleIn(CardsID[which], CardsLevel[which]);

        Player.UseAbility(CardsID[which], CardsLevel[which]);
        Player.SpendMana(Library.Cards[CardsID[which]].CardManaCost[CardsLevel[which]]);

        for (int i = which; i < CardsInHand; i++)
        {
            CardsID[i] = CardsID[i + 1];
            CardsLevel[i] = CardsLevel[i + 1];
        }
        CardsInHand--;

        UpdateInfo();
        Unhovered();
    }

    public void ShuffleHand()
    {
        for (int i = 0; i < CardsInHand; i++)
        {
            CardDiscard.ShuffleIn(CardsID[i], CardsLevel[i]);
        }

        CardsInHand = 0;

        UpdateInfo();
    }
}
