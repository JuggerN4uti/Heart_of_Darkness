using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCombat : MonoBehaviour
{
    [Header("Scripts")]
    public Combat CombatScript;
    public Hand Cards;

    [Header("Stats")]
    public int maxHealth;
    public int health, shield, block, energy, mana;
    public int strength, resistance;
    int tempi;

    [Header("Weapon")]
    public GameObject TheWeapon;
    public int baseDamage, strengthScaling, energyCost;
    public TMPro.TextMeshProUGUI TheWeaponCost, TheWeaponEffect;

    [Header("UI")]
    public GameObject ShieldDisplay;
    public GameObject BlockDisplay;
    public Image HealthBarFil;
    public Image EnergyBarFill;
    public Button WeaponUseButton;
    public TMPro.TextMeshProUGUI HealthValue, ShieldValue, BlockValue, EnergyValue, ManaValue;

    void Start()
    {
        StartTurn();
    }

    public void StartTurn()
    {
        block = 0;
        GainEnergy(10);
        GainMana(3);
        Cards.Draw(5);
        UpdateInfo();
    }

    void UpdateInfo()
    {
        HealthBarFil.fillAmount = (health * 1f) / (maxHealth * 1f);
        EnergyBarFill.fillAmount = (energy * 1f) / (energyCost * 1f);
        HealthValue.text = health.ToString("") + "/" + maxHealth.ToString("");
        EnergyValue.text = energy.ToString("");
        ManaValue.text = mana.ToString("");
        if (shield > 0)
        {
            ShieldDisplay.SetActive(true);
            ShieldValue.text = shield.ToString("");
        }
        else ShieldDisplay.SetActive(false);
        if (block > 0)
        {
            BlockDisplay.SetActive(true);
            BlockValue.text = block.ToString("");
        }
        else BlockDisplay.SetActive(false);
        if (energy >= energyCost)
            WeaponUseButton.interactable = true;
        else WeaponUseButton.interactable = false;
    }

    public void EndTurn()
    {
        if (energy > 10)
            energy = 10;
        Cards.ShuffleHand();
    }

    void GainBlock(int amount)
    {
        block += amount;
        UpdateInfo();
    }

    void GainEnergy(int amount)
    {
        energy += amount;
        UpdateInfo();
    }

    void SpendEnergy(int amount)
    {
        energy -= amount;
        UpdateInfo();
    }

    void GainMana(int amount)
    {
        mana += amount;
        UpdateInfo();
    }

    public void SpendMana(int amount)
    {
        mana -= amount;
        UpdateInfo();
    }

    public void WeaponHovered()
    {
        TheWeapon.SetActive(true);
        //TheWeapon.sprite = Library.Cards[CardsID[which]].CardSprite;
        //TheCardName.text = Library.Cards[CardsID[which]].CardName;
        TheWeaponCost.text = energyCost.ToString("");
        TheWeaponEffect.text = "Deal " + WeaponDamage().ToString("") + " Damage";
    }

    public void Unhovered()
    {
        TheWeapon.SetActive(false);
    }

    public void UseWeapon()
    {
        CombatScript.Enemy[CombatScript.targetedEnemy].TakeDamage(WeaponDamage());
        SpendEnergy(energyCost);
        UpdateInfo();
    }

    int WeaponDamage()
    {
        tempi = baseDamage + strengthScaling * strength;
        return tempi;
    }

    public void TakeDamage(int amount)
    {
        if (block > 0)
        {
            if (block > amount)
            {
                block -= amount;
                amount = 0;
            }
            else
            {
                amount -= block;
                block = 0;
            }
        }
        if (shield > 0)
        {
            if (shield > amount)
            {
                shield -= amount;
                amount = 0;
            }
            else
            {
                amount -= shield;
                shield = 0;
            }
        }
        health -= amount;
        UpdateInfo();
    }

    public void UseAbility(int which)
    {
        switch (which)
        {
            case 0:
                Strike();
                break;
            case 1:
                Defend();
                break;
            case 2:
                SpearThrust();
                break;
            case 3:
                Judgement();
                break;
            case 4:
                BolaShot();
                break;
            case 5:
                CripplingStrike();
                break;
        }
    }

    public string AbilityInfo(int which)
    {
        switch (which)
        {
            case 0:
                return "Deal " + StrikeDamage().ToString("") + " Damage";
            case 1:
                return "Gain " + DefendBlock().ToString("") + " Block";
            case 2:
                return "Break up to " + SpearThrustBreak().ToString("") + "Shield\n Deal " + SpearThrustDamage().ToString("") + " Damage";
            case 3:
                return "Deal " + JudgementDamage().ToString("") + " Damage";
            case 4:
                return "Deal " + BolaShotDamage().ToString("") + " Damage\n Apply " + BolaShotSlow().ToString("") + " Slow";
            case 5:
                return "Deal " + CripplingStrikeDamage().ToString("") + " Damage\n Apply " + CripplingStrikeBleed().ToString("") + " Bleed";
        }
        return "";
    }

    // Abilities ---
    void Strike() // ID 0
    {
        CombatScript.Enemy[CombatScript.targetedEnemy].TakeDamage(StrikeDamage());
    }

    int StrikeDamage()
    {
        tempi = 9 + strength;
        return tempi;
    }

    void Defend() // ID 1
    {
        GainBlock(DefendBlock());
    }

    int DefendBlock()
    {
        tempi = 8 + resistance;
        return tempi;
    }

    void SpearThrust() // ID 2
    {
        CombatScript.Enemy[CombatScript.targetedEnemy].BreakShield(SpearThrustBreak());
        CombatScript.Enemy[CombatScript.targetedEnemy].TakeDamage(SpearThrustDamage());
    }

    int SpearThrustBreak()
    {
        tempi = 8;
        return tempi;
    }

    int SpearThrustDamage()
    {
        tempi = 12 + strength;
        return tempi;
    }

    void Judgement() // ID 3
    {
        CombatScript.Enemy[CombatScript.targetedEnemy].TakeDamage(JudgementDamage());
    }

    int JudgementDamage()
    {
        tempi = 13 + strength;
        // if intedns +6 dmg
        return tempi;
    }

    void BolaShot() // ID 4
    {
        CombatScript.Enemy[CombatScript.targetedEnemy].TakeDamage(BolaShotDamage());
        CombatScript.Enemy[CombatScript.targetedEnemy].GainSlow(BolaShotSlow());
    }

    int BolaShotDamage()
    {
        tempi = 10 + strength;
        return tempi;
    }

    int BolaShotSlow()
    {
        tempi = 2;
        return tempi;
    }

    void CripplingStrike() // ID 5
    {
        CombatScript.Enemy[CombatScript.targetedEnemy].TakeDamage(CripplingStrikeDamage());
        CombatScript.Enemy[CombatScript.targetedEnemy].GainBleed(CripplingStrikeBleed());
    }

    int CripplingStrikeDamage()
    {
        tempi = 7 + strength;
        return tempi;
    }

    int CripplingStrikeBleed()
    {
        tempi = 4;
        return tempi;
    }
}
