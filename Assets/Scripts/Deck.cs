using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Deck : MonoBehaviour
{
    [Header("Scripts")]
    public CardLibrary Library;
    public Player PlayerScript;

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

    [Header("Forge/Merge")]
    public GameObject ForgeConfirmObject;
    public int selectedCard, forgeLevels;
    public TMPro.TextMeshProUGUI ChoiceMessage;

    [Header("Forge")]
    public bool forge;
    public Image[] ForgeCardRarity, ForgeCardIcon;
    public TMPro.TextMeshProUGUI[] ForgeCardManaCost, ForgeCardName, ForgeCardEffect;

    [Header("Merge")]
    public CardChoice CardEvent;
    public bool merge, found;
    public int selectedCardID, selectedCardLevel, foundCard;// foundCardID, foundCardLevel;
    public TMPro.TextMeshProUGUI ErrorMessage;

    [Header("Events")]
    public bool flame;
    public bool remove, duplicate;

    public void AddACard(int which, int level)
    {
        CardID[cardsInDeck] = which;
        CardLevel[cardsInDeck] = level;
        cardsInDeck++;
        CardsInDeckText.text = cardsInDeck.ToString("0");
        PlayerScript.UpdateInfo();
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
                ForgeCardRarity[i].sprite = Library.CardLevel[i * forgeLevels];
                ForgeCardIcon[i].sprite = Library.Cards[CardID[currentCard[slot]]].CardSprite;
                ForgeCardName[i].text = Library.Cards[CardID[currentCard[slot]]].CardName;
                ForgeCardManaCost[i].text = Library.Cards[CardID[currentCard[slot]]].CardManaCost[i * forgeLevels].ToString("");
                ForgeCardEffect[i].text = Library.Cards[CardID[currentCard[slot]]].CardTooltip[i * forgeLevels];
            }
            ForgeConfirmObject.SetActive(true);
            ChoiceMessage.text = "Upgrade";
        }
        else if (merge)
        {
            selectedCard = currentCard[slot];
            selectedCardID = CardID[currentCard[slot]];
            selectedCardLevel = CardLevel[currentCard[slot]];

            found = false;
            for (int i = 0; i < cardsInDeck; i++) // szukanie drugiej identycznej
            {
                if (i != selectedCard && CardID[i] == selectedCardID && CardLevel[i] == selectedCardLevel)
                {
                    foundCard = i;
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                ErrorMessage.text = "Found no Card to Merge";
                Invoke("ErrorEnd", 0.6f);
            }
            else
            {
                for (int i = 0; i < 2; i++)
                {
                    ForgeCardRarity[i].sprite = Library.CardLevel[i + selectedCardLevel];
                    ForgeCardIcon[i].sprite = Library.Cards[CardID[currentCard[slot]]].CardSprite;
                    ForgeCardName[i].text = Library.Cards[CardID[currentCard[slot]]].CardName;
                    ForgeCardManaCost[i].text = Library.Cards[CardID[currentCard[slot]]].CardManaCost[i + selectedCardLevel].ToString("");
                    ForgeCardEffect[i].text = Library.Cards[CardID[currentCard[slot]]].CardTooltip[i + selectedCardLevel];
                }
                ForgeConfirmObject.SetActive(true);
                ChoiceMessage.text = "Merge";
            }
        }
        else if (remove)
        {
            selectedCard = slot;

            if (flame)
            {
                if (CardID[selectedCard] > 1)
                    tempi = CardLevel[selectedCard] + 1;
                else if (CardLevel[selectedCard] > 0)
                    tempi = CardLevel[selectedCard];
                else tempi = 0;
                tempi = 5 * (tempi * (tempi + 1)) / 2;
                PlayerScript.GainSilver(tempi);
            }

            for (int i = selectedCard; i < cardsInDeck; i++)
            {
                CardID[i] = CardID[i + 1];
                CardLevel[i] = CardLevel[i + 1];
            }
            cardsInDeck--;
            CardObject[cardsInDeck].SetActive(false);
            CardsInDeckText.text = cardsInDeck.ToString("0");
            PlayerScript.UpdateInfo();
            SkipForge();
        }
        else if (duplicate)
        {
            selectedCard = slot;

            AddACard(CardID[selectedCard], 0);
            SkipForge();
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

    void DisplayMergeableCards() // currently commons & uncommons
    {
        for (int i = 0; i < cardsInDeck; i++)
        {
            CardObject[i].SetActive(false);
        }
        tempi = 0;
        for (int i = 0; i < cardsInDeck; i++)
        {
            if (CardLevel[i] <= 1)
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

    public void ShowCardsToForge(int by = 1)
    {
        forge = true;
        forgeLevels = by;
        DisplayCommonCards();
        DeckObjet.SetActive(true);
        DeckButton.SetActive(false);
        SkipButton.SetActive(true);
        PlayerInfoButton.SetActive(false);
    }

    public void RemoveCard(bool cursedFlame)
    {
        flame = cursedFlame;
        remove = true;
        DisplayDeckContents();
        DeckObjet.SetActive(true);
        DeckButton.SetActive(false);
        SkipButton.SetActive(true);
        PlayerInfoButton.SetActive(false);
    }

    public void DuplicateCard()
    {
        duplicate = true;
        DisplayDeckContents();
        DeckObjet.SetActive(true);
        DeckButton.SetActive(false);
        SkipButton.SetActive(true);
        PlayerInfoButton.SetActive(false);
    }

    public void SkipForge()
    {
        forge = false;
        merge = false;
        remove = false;
        flame = false;
        duplicate = false;
        DeckObjet.SetActive(false);
        DeckButton.SetActive(true);
        SkipButton.SetActive(false);
        PlayerInfoButton.SetActive(true);
    }

    public void BackFromForging()
    {
        ForgeConfirmObject.SetActive(false);
    }

    public void ShowCardsToMerge()
    {
        merge = true;
        DisplayMergeableCards();
        DeckObjet.SetActive(true);
        DeckButton.SetActive(false);
        SkipButton.SetActive(true);
        PlayerInfoButton.SetActive(false);
    }

    public void Upgrade()
    {
        ForgeConfirmObject.SetActive(false);
        if (forge)
            CardLevel[selectedCard] += forgeLevels;
        else if (merge)
        {
            CardLevel[selectedCard]++;
            for (int i = foundCard; i < cardsInDeck; i++)
            {
                CardID[i] = CardID[i + 1];
                CardLevel[i] = CardLevel[i + 1];
            }
            cardsInDeck--;
            CardObject[cardsInDeck].SetActive(false);
            CardsInDeckText.text = cardsInDeck.ToString("0");
            CardEvent.CardChoiceObject.SetActive(false);
        }
        PlayerScript.UpdateInfo();
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

    public int UncommonCardsInDeck()
    {
        tempi = 0;
        for (int i = 0; i < cardsInDeck; i++)
        {
            if (CardLevel[i] == 1)
                tempi++;
        }

        return tempi;
    }

    public int RareCardsInDeck()
    {
        tempi = 0;
        for (int i = 0; i < cardsInDeck; i++)
        {
            if (CardLevel[i] == 2)
                tempi++;
        }

        return tempi;
    }

    void ErrorEnd()
    {
        ErrorMessage.text = "";
    }
}
