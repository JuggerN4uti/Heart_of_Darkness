using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ForgeChoice : MonoBehaviour
{
    [Header("Scripts")]
    public Player PlayerScript;
    //public Deck DeckScript;
    //public CardPick CardPickScript;

    //[Header("Stats")]
    //public int roll;
    //public bool viable;

    [Header("UI")]
    public GameObject ForgeEventObject;
    //public GameObject CarEventObject;
    //public Image SecondOptionImage;
    public Button[] UpgradeButton;
    public TMPro.TextMeshProUGUI[] CostText, Info;

    [Header("Gear")]
    public bool[] set;
    public int[] cost;
    public float[] charge, chargesReq, chargeIncrease;
    float temp;

    //[Header("Sprites")]
    //public Sprite[] SecondOptionSprite;

    public void Open()
    {
        for (int i = 0; i < 3; i++)
        {
            charge[i] += Random.Range(0.71f, 1.74f);
        }
        SetGear();
        ForgeEventObject.SetActive(true);
    }

    /*public void UpgradeCard()
    {
        if (DeckScript.CommonCardsInDeck() > 2)
            SetCardsOption();
        else
        {
            ErrorMessage.text = "Found not Enough Card to Upgrade";
            Invoke("ErrorEnd", 0.6f);
        }
    }*/

    public void UpgradeGear(int slot)
    {
        PlayerScript.StatValues[5 + slot]++;
        PlayerScript.SpendIron(cost[slot]);
        for (int i = 0; i < 3; i++)
        {
            if (i != slot)
                charge[i] += cost[slot] * 0.0278f;
        }
        while (charge[slot] >= 1f)
        {
            charge[slot] -= 1f;
        }
        chargesReq[slot] += chargeIncrease[slot];
        SetGear();
    }

    void SetGear()
    {
        for (int i = 0; i < 3; i++)
        {
            cost[i] = 0;
            temp = charge[i];
            while (temp < chargesReq[i])
            {
                cost[i]++;
                temp += 1f;
            }
            CostText[i].text = cost[i].ToString("0");
            Info[i].text = "Current: " + PlayerScript.StatValues[5 + i].ToString("0");
            if (PlayerScript.Iron >= cost[i])
                UpgradeButton[i].interactable = true;
            else UpgradeButton[i].interactable = false;
        }
    }

    public void Close()
    {
        ForgeEventObject.SetActive(false);
    }

    void ErrorEnd()
    {
        //ErrorMessage.text = "";
    }
}
