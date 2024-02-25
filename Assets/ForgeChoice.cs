using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ForgeChoice : MonoBehaviour
{
    [Header("Scripts")]
    public Player PlayerScript;
    public Deck DeckScript;

    [Header("Stats")]
    public int roll;
    public bool viable;

    [Header("UI")]
    public GameObject ForgeEventObject;
    public Image SecondOptionImage;
    public TMPro.TextMeshProUGUI ErrorMessage;

    [Header("Gear")]
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
    }

    public void UpgradeCard()
    {
        if (DeckScript.CommonCardsInDeck() > 0)
        {
            viable = false;
            do
            {
                roll = Random.Range(0, DeckScript.cardsInDeck);
                if (DeckScript.CardLevel[roll] == 0)
                    viable = true;
            } while (!viable);
            DeckScript.CardLevel[roll]++;
            Close(true);
        }
        else
        {
            ErrorMessage.text = "Found no Card to Upgrade";
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
                    PlayerScript.StatValues[2] += (shieldAmount + nextShield);
                    break;
                case 1:
                    PlayerScript.StatValues[3] += (armorAmount + nextArmor);
                    break;
                case 2:
                    PlayerScript.StatValues[5] += nextResistance;
                    break;
            }
            ChargeArmor(5.4f);
        }
        Close(false);
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
                ChargeArmor(0.3f);
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
        while (shieldCharge >= 5.8f)
        {
            shieldCharge -= 5.8f;
            nextShield++;
        }

        armorCharge += amount;
        while (armorCharge >= 15f)
        {
            armorCharge -= 15f;
            nextArmor++;
        }

        resistanceCharge += amount;
        while (resistanceCharge >= 26f)
        {
            resistanceCharge -= 26f;
            nextResistance++;
        }
    }

    public void Close(bool cardChosen)
    {
        if (cardChosen)
        {
            temp = PlayerScript.weaponEnergyRequirement * 0.0325f + PlayerScript.weaponStrengthBonus * 0.0033f;
            ChargeWeapon(temp);
            ChargeArmor(0.9f);
        }
        ForgeEventObject.SetActive(false);
    }

    void ErrorEnd()
    {
        ErrorMessage.text = "";
    }
}
