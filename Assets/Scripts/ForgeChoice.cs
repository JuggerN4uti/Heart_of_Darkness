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
    public TMPro.TextMeshProUGUI[] CostText;

    [Header("Gear")]
    public bool[] set;
    public int[] cost;
    public float[] charge, chargesReq, chargeIncrease;

    //[Header("Sprites")]
    //public Sprite[] SecondOptionSprite;

    public void Open()
    {
        for (int i = 0; i < 3; i++)
        {
            ChargeGear(i, Random.Range(0.5f, 1.5f));
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
                ChargeGear(i, cost[slot] * 0.02f);
        }
        cost[slot] = 0;
        set[slot] = false;
        SetGear();
    }

    void SetGear()
    {
        for (int i = 0; i < 3; i++)
        {
            while (!set[i])
            {
                cost[i]++;
                ChargeGear(i, 1f);
            }
            CostText[i].text = cost[i].ToString("0");
            if (PlayerScript.Iron >= cost[i])
                UpgradeButton[i].interactable = true;
            else UpgradeButton[i].interactable = false;
        }
    }

    void ChargeGear(int which, float amount)
    {
        charge[which] += amount;
        if (charge[which] >= chargesReq[which] && !set[which])
        {
            charge[which] -= chargesReq[which];
            chargesReq[which] += chargeIncrease[which];
            set[which] = true;
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
