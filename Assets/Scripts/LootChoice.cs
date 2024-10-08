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
    public BossLoot BossLootScript;

    [Header("Stats")]
    public int silver;
    public int iron, cards, uncommonCards, merges, roll, rollsCount, gemID;
    public int bonusCardGotten;
    public bool item, gem;
    public int[] lootID; // 0 - silver, 1 - common card, 2 - uncommon card, 3 - gem, 4 - item, 5 - merge, 6 - iron
    public float gemCharge, bonusCardCharge, qualityUpgradeCharge, mergeCharge;
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
    public Sprite IronSprite, ItemSprite, BossSprite, CommonSprite, UncommonSprite, MergeSprite;
    public Sprite[] GemSprites;

    public void SetRewards(float dangerBonus, bool elite)
    {
        silver = Random.Range(4, 6);
        iron = Random.Range(2, 5);
        item = elite;
        temp = dangerBonus + gemCharge;
        if (temp >= 40f && temp >= Random.Range(0f, 200f + temp))
        {
            gem = true;
            //gemID = Random.Range(0, GemSprites.Length);
            gemCharge = 0f;
        }
        else
        {
            gem = false;
            gemCharge += dangerBonus * 0.4f;
        }
        cards = 1;
        uncommonCards = 0;
        merges = 0;
        rollsCount = 0;
        while (dangerBonus > 0f)
        {
            silver++;
            ChargeCard(0.02f);
            ChargeMerge(0.011f);
            ChargeQuality(0.004f);
            roll = Random.Range(0, 8);
            switch (roll)
            {
                case 0:
                    silver += Random.Range(1, 4);
                    break;
                case 1:
                    silver += Random.Range(1, 4);
                    break;
                case 2:
                    ChargeCard(0.2f);
                    break;
                case 3:
                    ChargeQuality(0.0424f);
                    break;
                case 4:
                    ChargeMerge(0.1166f);
                    break;
                case 5:
                    ChargeCard(0.08f);
                    ChargeQuality(0.0252f);
                    break;
                case 6:
                    ChargeMerge(0.0699f);
                    ChargeQuality(0.0169f);
                    break;
                case 7:
                    iron++;
                    break;
            }
            dangerBonus -= 1.074f + rollsCount * 0.174f;
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
        if (iron > 0)
        {
            PicksObject[current].SetActive(true);
            PicksImages[current].sprite = IronSprite;
            PicksValue[current].text = iron.ToString("0");
            lootID[current] = 6;
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
            PicksImages[current].sprite = BossSprite; //GemSprites[gemID];
            PicksValue[current].text = "!!!";//"Gem!";
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
        for (int i = 0; i < merges; i++)
        {
            PicksObject[current].SetActive(true);
            PicksImages[current].sprite = MergeSprite;
            PicksValue[current].text = "Merge";
            lootID[current] = 5;
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
                BossLootScript.Open(1);
                //PlayerScript.Gems[gemID]++;
                gem = false;
                break;
            case 4:
                ItemPickScript.RollItems();
                item = false;
                break;
            case 5:
                PlayerScript.DeckScript.ShowCardsToMerge();
                merges--;
                break;
            case 6:
                PlayerScript.GainIron(iron);
                iron = 0;
                break;
        }

        UpdateLoot();
        CheckForEmpty();
    }

    void CheckForEmpty()
    {
        if (silver == 0 && iron == 0 && cards == 0 && uncommonCards == 0 && merges == 0 && !gem && !item)
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
            bonusCardGotten++;
            bonusCardCharge -= 1.04f + 0.02f * bonusCardGotten;
            cards++;
            ChargeQuality(0.01f + 0.002f * bonusCardGotten);
            ChargeMerge(0.044f + 0.022f * bonusCardGotten);
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

    void ChargeMerge(float amount)
    {
        mergeCharge += amount;
        while (mergeCharge >= 1f)
        {
            mergeCharge -= 1f;
            merges++;
        }
    }
}
