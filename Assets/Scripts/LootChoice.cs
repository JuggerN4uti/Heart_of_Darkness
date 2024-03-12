using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LootChoice : MonoBehaviour
{
    [Header("Scripts")]
    public Player PlayerScript;
    public CardPick CardCollection;
    public ItemPick ItemPickScript;

    [Header("Stats")]
    public int silver;
    public int cards, uncommonCards, roll, rollsCount, gemID;
    public bool item, gem;
    public int[] lootID; // 0 - silver, 1 - common card, 2 - uncommon card, 3 - gem
    public float gemCharge, bonusCardCharge, qualityUpgradeCharge;
    int current;
    float temp;

    [Header("UI")]
    public GameObject LootEventObject;
    public GameObject CardPickObject;
    public GameObject[] PicksObject;
    public Image[] PicksImages;
    public TMPro.TextMeshProUGUI[] PicksValue;

    [Header("Sprites")]
    public Sprite SilverSprite;
    public Sprite ItemSprite, CommonSprite, UncommonSprite;
    public Sprite[] GemSprites;

    public void SetRewards(float dangerBonus, bool elite)
    {
        silver = Random.Range(8, 14);
        item = elite;
        temp = dangerBonus + gemCharge;
        if (temp >= Random.Range(0f, 100f + temp))
        {
            gem = true;
            gemID = Random.Range(0, GemSprites.Length);
            gemCharge = 0f;
        }
        else
        {
            gem = false;
            gemCharge += dangerBonus * 0.4f;
        }
        cards = 1;
        uncommonCards = 0;
        rollsCount = 0;
        while (dangerBonus > 0f)
        {
            silver++;
            ChargeCard(0.02f);
            ChargeQuality(0.004f);
            roll = Random.Range(0, 5);
            switch (roll)
            {
                case 0:
                    silver += Random.Range(1, 4);
                    break;
                case 1:
                    ChargeCard(0.2f);
                    break;
                case 2:
                    ChargeQuality(0.04f);
                    break;
                case 3:
                    silver += Random.Range(1, 4);
                    break;
                case 4:
                    ChargeCard(0.08f);
                    ChargeQuality(0.024f);
                    break;
            }
            dangerBonus -= 0.96f + rollsCount * 0.16f;
            rollsCount++;
        }
        UpdateLoot();
        LootEventObject.SetActive(true);
    }

    void UpdateLoot()
    {
        for (int i = 0; i < PicksObject.Length; i++)
        {
            PicksObject[i].SetActive(false);
        }
        current = 0;
        if (silver > 0)
        {
            PicksObject[current].SetActive(true);
            PicksImages[current].sprite = SilverSprite;
            PicksValue[current].text = silver.ToString("0");
            lootID[current] = 0;
            current++;
        }
        if (item)
        {
            PicksObject[current].SetActive(true);
            PicksImages[current].sprite = ItemSprite;
            PicksValue[current].text = "Item";
            lootID[current] = 4;
            current++;
        }
        if (gem)
        {
            PicksObject[current].SetActive(true);
            PicksImages[current].sprite = GemSprites[gemID];
            PicksValue[current].text = "Gem!";
            lootID[current] = 3;
            current++;
        }
        for (int i = 0; i < cards; i++)
        {
            PicksObject[current].SetActive(true);
            PicksImages[current].sprite = CommonSprite;
            PicksValue[current].text = "Card";
            lootID[current] = 1;
            current++;
        }
        for (int i = 0; i < uncommonCards; i++)
        {
            PicksObject[current].SetActive(true);
            PicksImages[current].sprite = UncommonSprite;
            PicksValue[current].text = "Card";
            lootID[current] = 2;
            current++;
        }
    }

    public void CollectLoot(int which)
    {
        switch (lootID[which])
        {
            case 0:
                PlayerScript.GainSilver(silver);
                silver = 0;
                break;
            case 1:
                CardCollection.RollCards(0);
                CardPickObject.SetActive(true);
                cards--;
                break;
            case 2:
                CardCollection.RollCards(1);
                CardPickObject.SetActive(true);
                uncommonCards--;
                break;
            case 3:
                PlayerScript.Gems[gemID]++;
                gem = false;
                break;
            case 4:
                ItemPickScript.RollItems();
                item = false;
                break;
        }

        UpdateLoot();
        CheckForEmpty();
    }

    void CheckForEmpty()
    {
        if (silver == 0 && cards == 0 && uncommonCards == 0 && !gem && !item)
            Close();
    }

    public void Close()
    {
        LootEventObject.SetActive(false);
    }

    void ChargeCard(float amount)
    {
        bonusCardCharge += amount;
        while (bonusCardCharge >= 1f)
        {
            bonusCardCharge -= 1f;
            cards++;
            ChargeQuality(0.01f);
        }
    }

    void ChargeQuality(float amount)
    {
        qualityUpgradeCharge += amount;
        while (qualityUpgradeCharge >= 1f)
        {
            qualityUpgradeCharge -= 1f;
            uncommonCards++;
            cards--;
        }
    }
}
