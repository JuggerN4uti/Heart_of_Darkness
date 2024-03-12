using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ForgeChoice : MonoBehaviour
{
    [Header("Scripts")]
    public Player PlayerScript;
    public Deck DeckScript;
    public CardPick CardPickScript;

    [Header("Stats")]
    public int roll;
    public bool viable;

    [Header("UI")]
    public GameObject ForgeEventObject;
    public GameObject CarEventObject;
    public Image SecondOptionImage;
    public TMPro.TextMeshProUGUI ErrorMessage;

    [Header("Gear")] //20.8 power
    public bool weapon;
    public int baseDamageIncrease, nextDamageIncrease;
    public float temp, nextCharge;
    public int shieldAmount, armorAmount, nextShield, nextArmor, nextResistance;
    public float shieldCharge, armorCharge, resistanceCharge;
    public TMPro.TextMeshProUGUI SecendOptionEffect;

    [Header("Sprites")]
    public Sprite[] SecondOptionSprite;

    public void Open()
    {
        roll = Random.Range(0, 3);
        SetGearOption(roll);
        ForgeEventObject.SetActive(true);
        CarEventObject.SetActive(false);
    }

    public void UpgradeCard()
    {
        if (DeckScript.CommonCardsInDeck() > 2)
            SetCardsOption();
        else
        {
            ErrorMessage.text = "Found not Enough Card to Upgrade";
            Invoke("ErrorEnd", 0.6f);
        }
    }

    public void UpgradeGear()
    {
        if (weapon)
        {
            PlayerScript.weaponDamage += (baseDamageIncrease + nextDamageIncrease);
            ChargeWeapon(temp);
        }
        else
        {
            switch (roll)
            {
                case 0:
                    PlayerScript.StatValues[5] += (shieldAmount + nextShield);
                    nextShield = 0;
                    break;
                case 1:
                    PlayerScript.StatValues[6] += (armorAmount + nextArmor);
                    nextArmor = 0;
                    break;
                case 2:
                    PlayerScript.StatValues[7] += nextResistance;
                    nextResistance = 0;
                    break;
            }
            ChargeArmor(5.51f); //26,5%
        }
        Close();
    }

    void SetCardsOption()
    {
        CardPickScript.RollForge();
        CarEventObject.SetActive(true);

        temp = PlayerScript.weaponEnergyRequirement * 0.0325f + PlayerScript.weaponStrengthBonus * 0.0033f;
        ChargeWeapon(temp);
        ChargeArmor(0.93f); // 4,5%
    }

    void SetGearOption(int which)
    {
        if (which == 0)
        {
            weapon = true;
            SecondOptionImage.sprite = SecondOptionSprite[0];
            baseDamageIncrease = PlayerScript.weaponEnergyRequirement / 4;
            temp = PlayerScript.weaponEnergyRequirement * 0.26f + PlayerScript.weaponStrengthBonus * 0.02f;
            temp -= baseDamageIncrease * 1f;
            SecendOptionEffect.text = "Increase Damage of\nyour Weapon by " + (baseDamageIncrease + nextDamageIncrease).ToString("0");
        }
        else
        {
            weapon = false;
            SecondOptionImage.sprite = SecondOptionSprite[1];

            viable = false;
            do
            {
                ChargeArmor(0.31f); // 1,5%
                roll = Random.Range(0, 3);
                if (roll == 2 && nextResistance == 0)
                    viable = false;
                else viable = true;
            } while (!viable);

            switch (roll)
            {
                case 0:
                    SecendOptionEffect.text = "Increase your Shield\nby " + (shieldAmount + nextShield).ToString("0");
                    break;
                case 1:
                    SecendOptionEffect.text = "Increase your Armor\nby " + (armorAmount + nextArmor).ToString("0");
                    break;
                case 2:
                    SecendOptionEffect.text = "Increase your Resistance\nby " + nextResistance.ToString("0");
                    break;
            }
        }
    }

    void ChargeWeapon(float amount)
    {
        nextCharge += amount;
        while (nextCharge >= 1f)
        {
            nextCharge -= 1f;
            nextDamageIncrease++;
        }
    }

    void ChargeArmor(float amount)
    {
        shieldCharge += amount;
        while (shieldCharge >= 5.5f)
        {
            shieldCharge -= 5.5f;
            nextShield++;
        }

        armorCharge += amount;
        while (armorCharge >= 14.1f)
        {
            armorCharge -= 14.1f;
            nextArmor++;
        }

        resistanceCharge += amount;
        while (resistanceCharge >= 26.6f)
        {
            resistanceCharge -= 26.6f;
            nextResistance++;
        }
    }

    public void Close()
    {
        ForgeEventObject.SetActive(false);
    }

    void ErrorEnd()
    {
        ErrorMessage.text = "";
    }
}
