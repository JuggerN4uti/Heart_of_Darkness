using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Deck : MonoBehaviour
{
    [Header("Scripts")]
    public CardLibrary Library;

    [Header("Cards")]
    public int[] CardID;
    public int[] CardLevel, currentCard;
    public int cardsInDeck;
    public bool opened;
    int tempi;

    [Header("UI")]
    public TMPro.TextMeshProUGUI CardsInDeckText;
    public GameObject DeckObjet, DeckButton, SkipButton, PlayerInfoButton;
    public GameObject[] CardObject;
    public Image[] CardRarity, CardIcon;
    public TMPro.TextMeshProUGUI[] CardManaCost;

    [Header("Forge")]
    public bool forge;
    public GameObject ForgeConfirmObject;
    public Image[] ForgeCardIcon;
    public TMPro.TextMeshProUGUI[] ForgeCardManaCost, ForgeCardName, ForgeCardEffect;
    public int selectedCard;

    public void AddACard(int which, int level)
    {
        CardID[cardsInDeck] = which;
        CardLevel[cardsInDeck] = level;
        cardsInDeck++;
        CardsInDeckText.text = cardsInDeck.ToString("0");
    }

    public void ShowDeck()
    {
        if (opened)
        {
            opened = false;
            DeckObjet.SetActive(false);
            PlayerInfoButton.SetActive(true);
        }
        else
        {
            opened = true;
            DisplayDeckContents();
            DeckObjet.SetActive(true);
            PlayerInfoButton.SetActive(false);
        }
    }

    public void SelectCard(int slot)
    {
        if (forge)
        {
            selectedCard = currentCard[slot];
            for (int i = 0; i < 2; i++)
            {
                ForgeCardIcon[i].sprite = Library.Cards[CardID[currentCard[slot]]].CardSprite;
                ForgeCardName[i].text = Library.Cards[CardID[currentCard[slot]]].CardName;
                ForgeCardManaCost[i].text = Library.Cards[CardID[currentCard[slot]]].CardManaCost[i].ToString("");
                ForgeCardEffect[i].text = Library.Cards[CardID[currentCard[slot]]].CardTooltip[i];
            }
            ForgeConfirmObject.SetActive(true);
        }
    }

    void DisplayDeckContents()
    {
        for (int i = 0; i < cardsInDeck; i++)
        {
            CardObject[i].SetActive(true);
            CardRarity[i].sprite = Library.CardLevel[CardLevel[i]];
            CardIcon[i].sprite = Library.Cards[CardID[i]].CardSprite;
            CardManaCost[i].text = Library.Cards[CardID[i]].CardManaCost[CardLevel[i]].ToString("");
        }
    }

    void DisplayCommonCards()
    {
        for (int i = 0; i < cardsInDeck; i++)
        {
            CardObject[i].SetActive(false);
        }
        tempi = 0;
        for (int i = 0; i < cardsInDeck; i++)
        {
            if (CardLevel[i] == 0)
            {
                currentCard[tempi] = i;
                CardObject[tempi].SetActive(true);
                CardRarity[tempi].sprite = Library.CardLevel[CardLevel[i]];
                CardIcon[tempi].sprite = Library.Cards[CardID[i]].CardSprite;
                CardManaCost[tempi].text = Library.Cards[CardID[i]].CardManaCost[CardLevel[i]].ToString("");
                tempi++;
            }
        }
    }

    public void ShowCardsToForge()
    {
        forge = true;
        DisplayCommonCards();
        DeckObjet.SetActive(true);
        DeckButton.SetActive(false);
        SkipButton.SetActive(true);
        PlayerInfoButton.SetActive(false);
    }

    public void SkipForge()
    {
        forge = false;
        DeckObjet.SetActive(false);
        DeckButton.SetActive(true);
        SkipButton.SetActive(false);
        PlayerInfoButton.SetActive(true);
    }

    public void BackFromForging()
    {
        ForgeConfirmObject.SetActive(false);
    }

    public void Upgrade()
    {
        ForgeConfirmObject.SetActive(false);
        CardLevel[selectedCard]++;
        SkipForge();
    }

    public int CommonCardsInDeck()
    {
        tempi = 0;
        for (int i = 0; i < cardsInDeck; i++)
        {
            if (CardLevel[i] == 0)
                tempi++;
        }

        return tempi;
    }
}
