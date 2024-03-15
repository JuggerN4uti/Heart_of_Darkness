using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MerchantChoice : MonoBehaviour
{
    [Header("Scripts")]
    public CardLibrary Library;
    public ItemLibrary ILibrary;
    public Player PlayerScript;
    public Deck CardDeck;
    public ForgeChoice ForgeScript;

    [Header("Stats")]
    public int[] rolledID;
    public int[] itemID;
    public int[] CardRarity, CardCost, ItemCost, EventCost;
    public int min, max;
    int roll;

    [Header("UI")]
    public GameObject MerchantEventObject;
    public GameObject CardEventObject;
    public GameObject[] CardObject, ItemObject;
    public Button[] CardButton, ItemButton, EventButton;
    public Image[] CardRarityImage, CardIcon, ItemIcon;
    public TMPro.TextMeshProUGUI[] CardManaCost, CardNameText, CardEffectText, CardSilverCost, EventSilverCost, ItemName, ItemEffectText, ItemCostText;

    public void Open()
    {
        SetRange();
        RollCards();
        RollItems();
        EventCost[0] = 15;
        EventCost[1] = 20;

        UpdateInfo();

        MerchantEventObject.SetActive(true);
    }

    void UpdateInfo()
    {
        for (int i = 0; i < 4; i++)
        {
            if (PlayerScript.Silver >= CardCost[i])
                CardButton[i].interactable = true;
            else CardButton[i].interactable = false;
        }

        for (int i = 0; i < 2; i++)
        {
            if (PlayerScript.Silver >= ItemCost[i])
                ItemButton[i].interactable = true;
            else ItemButton[i].interactable = false;
        }

        for (int i = 0; i < 2; i++)
        {
            EventSilverCost[i].text = EventCost[i].ToString("0");
            if (PlayerScript.Silver >= EventCost[i])
                EventButton[i].interactable = true;
            else EventButton[i].interactable = false;
        }
    }

    public void BuyCard(int slot)
    {
        CardDeck.AddACard(rolledID[slot], CardRarity[slot]);
        CardObject[slot].SetActive(false);
        PlayerScript.SpendSilver(CardCost[slot]);

        UpdateInfo();
    }

    public void BuyItem(int slot)
    {
        PlayerScript.CollectItem(itemID[slot]);
        ItemObject[slot].SetActive(false);
        PlayerScript.SpendSilver(ItemCost[slot]);

        UpdateInfo();
    }

    public void BuyEvent(int slot)
    {
        PlayerScript.SpendSilver(EventCost[slot]);
        if (slot == 0)
        {
            CardEventObject.SetActive(true);
            EventCost[slot] += 3;
        }
        else 
        {
            ForgeScript.Open();
            EventCost[slot] += 5;
        }

        UpdateInfo();
    }

    public void Leave()
    {
        MerchantEventObject.SetActive(false);
    }

    void RollCards()
    {
        roll = Random.Range(min, max);
        rolledID[0] = roll;

        do
        {
            roll = Random.Range(min, max);
        } while (roll == rolledID[0]);
        rolledID[1] = roll;

        do
        {
            roll = Random.Range(min, max);
        } while (roll == rolledID[0] || roll == rolledID[1]);
        rolledID[2] = roll;

        do
        {
            roll = Random.Range(min, max);
        } while (roll == rolledID[0] || roll == rolledID[1] || roll == rolledID[2]);
        rolledID[3] = roll;

        SetCards();
    }

    public void RollItems()
    {
        do
        {
            roll = Random.Range(2, ILibrary.Items.Length);
        } while (PlayerScript.Item[roll]);
        itemID[0] = roll;

        do
        {
            roll = Random.Range(2, ILibrary.Items.Length);
        } while (PlayerScript.Item[roll] || roll == itemID[0]);
        itemID[1] = roll;

        for (int i = 0; i < 2; i++)
        {
            ItemObject[i].SetActive(true);
            ItemIcon[i].sprite = ILibrary.Items[itemID[i]].ItemSprite;
            ItemName[i].text = ILibrary.Items[itemID[i]].ItemName;
            ItemEffectText[i].text = ILibrary.Items[itemID[i]].ItemTooltip;
            ItemCost[i] = Random.Range(89, 101);
            ItemCostText[i].text = ItemCost[i].ToString("0");
        }
    }

    void SetCards()
    {
        for (int i = 0; i < 4; i++)
        {
            CardObject[i].SetActive(true);
            CardIcon[i].sprite = Library.Cards[rolledID[i]].CardSprite;
            CardNameText[i].text = Library.Cards[rolledID[i]].CardName;

            if (Random.Range(0, 4) == 3)
                CardRarity[i] = 1;
            else CardRarity[i] = 0;

            CardRarityImage[i].sprite = Library.CardLevel[CardRarity[i]];
            CardEffectText[i].text = Library.Cards[rolledID[i]].CardTooltip[CardRarity[i]];
            CardManaCost[i].text = Library.Cards[rolledID[i]].CardManaCost[CardRarity[i]].ToString("0");

            CardCost[i] = Random.Range(15 + 22 * CardRarity[i], 21 + 25 * CardRarity[i]);
            CardSilverCost[i].text = CardCost[i].ToString("0");
        }
    }

    void SetRange()
    {
        switch (CardDeck.PlayerScript.Class)
        {
            case 0:
                min = 2;
                max = 2 + Library.lightCards;
                break;
            case 1:
                min = 2 + Library.lightCards;
                max = Library.Cards.Length;
                break;
        }
    }
}
