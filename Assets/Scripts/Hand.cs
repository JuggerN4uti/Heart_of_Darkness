using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hand : MonoBehaviour
{
    [Header("Scripts")]
    public PlayerCombat Player;
    public Player PlayerScript;
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
    public int skipDraw;
    public bool destroyed;
    int playedID, playedLevel;

    [Header("Item Stats")]
    public int cardsPlayed;

    [Header("Sprites")]
    public Sprite CardDrawSkipSprite;

    void Start()
    {
        //Draw(5);
    }

    public void UpdateInfo()
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
            if (skipDraw >= 5)
            {
                skipDraw -= 5;
                Player.Display(1, CardDrawSkipSprite);
            }
            else
            {
                if (Player.PlayerScript.CurseValue[3] > 0)
                    skipDraw += Player.PlayerScript.CurseValue[3];
                if (CardsInHand < 10)
                {
                    CardsID[CardsInHand] = CardDraw.Draw();
                    CardsLevel[CardsInHand] = CardDraw.DrawLevel();
                    CardsInHand++;
                }
                else
                {
                    CardDiscard.ShuffleIn(CardDraw.Draw(), CardDraw.DrawLevel());
                }
            }
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
        destroyed = false;
        playedID = CardsID[which];
        playedLevel = CardsLevel[which];

        if (!Library.Cards[playedID].SingleUse)
        {
            if (playedID == 10)
            {
                if (Player.HealthProcentage() >= 0.5f)
                    CardDiscard.ShuffleIn(playedID, playedLevel);
                else destroyed = true;
            }
            else CardDiscard.ShuffleIn(playedID, playedLevel);
        }
        else destroyed = true;

        for (int i = which; i < CardsInHand; i++)
        {
            if (i < 9)
            {
                CardsID[i] = CardsID[i + 1];
                CardsLevel[i] = CardsLevel[i + 1];
            }
        }
        CardsInHand--;

        Player.UseAbility(playedID, playedLevel);
        Player.SpendMana(Library.Cards[playedID].CardManaCost[playedLevel]);
        if (PlayerScript.Item[38] && Library.Cards[playedID].CardManaCost[playedLevel] >= 2)
            Player.EquipmentCooldown(4);

        if (PlayerScript.Item[19])
        {
            cardsPlayed++;
            Player.ItemsScript.SetText();
            if (cardsPlayed % 7 == 0)
                Draw(1);
        }

        if (PlayerScript.Item[21] && destroyed)
        {
            Player.GainBlock(5);
            Draw(1);
        }

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
