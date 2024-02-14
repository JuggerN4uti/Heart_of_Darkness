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
        mana = 0;
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

    public void UseAbility(int which, int level)
    {
        switch (which)
        {
            case 0:
                Strike(level);
                break;
            case 1:
                Defend(level);
                break;
            case 2:
                SpearThrust(level);
                break;
            case 3:
                Judgement(level);
                break;
            case 4:
                BolaShot(level);
                break;
            case 5:
                CripplingStrike(level);
                break;
        }
    }

    public string AbilityInfo(int which, int level)
    {
        switch (which)
        {
            case 0:
                return "Deal " + StrikeDamage(level).ToString("") + " Damage";
            case 1:
                return "Gain " + DefendBlock(level).ToString("") + " Block";
            case 2:
                return "Break up to " + SpearThrustBreak(level).ToString("") + "Shield\n Deal " + SpearThrustDamage(level).ToString("") + " Damage";
            case 3:
                return "Deal " + JudgementDamage(level).ToString("") + " Damage";
            case 4:
                return "Deal " + BolaShotDamage(level).ToString("") + " Damage\n Apply " + BolaShotSlow(level).ToString("") + " Slow";
            case 5:
                return "Deal " + CripplingStrikeDamage(level).ToString("") + " Damage\n Apply " + CripplingStrikeBleed(level).ToString("") + " Bleed";
        }
        return "";
    }

    // Abilities ---
    void Strike(int level) // ID 0
    {
        CombatScript.Enemy[CombatScript.targetedEnemy].TakeDamage(StrikeDamage(level));
    }

    int StrikeDamage(int level)
    {
        tempi = 9 + strength;
        tempi += 3 * level;
        return tempi;
    }

    void Defend(int level) // ID 1
    {
        GainBlock(DefendBlock(level));
    }

    int DefendBlock(int level)
    {
        tempi = 8 + resistance;
        tempi += 3 * level;
        return tempi;
    }

    void SpearThrust(int level) // ID 2
    {
        CombatScript.Enemy[CombatScript.targetedEnemy].BreakShield(SpearThrustBreak(level));
        CombatScript.Enemy[CombatScript.targetedEnemy].TakeDamage(SpearThrustDamage(level));
    }

    int SpearThrustBreak(int level)
    {
        tempi = 8;
        tempi += 4 * level;
        return tempi;
    }

    int SpearThrustDamage(int level)
    {
        tempi = 11 + strength;
        tempi += 3 * level;
        return tempi;
    }

    void Judgement(int level) // ID 3
    {
        CombatScript.Enemy[CombatScript.targetedEnemy].TakeDamage(JudgementDamage(level));
    }

    int JudgementDamage(int level)
    {
        tempi = 12 + strength;
        tempi += 4 * level;
        if (CombatScript.Enemy[CombatScript.targetedEnemy].IntentToAttack())
            tempi += 8 + 4 * level;
        return tempi;
    }

    void BolaShot(int level) // ID 4
    {
        CombatScript.Enemy[CombatScript.targetedEnemy].TakeDamage(BolaShotDamage(level));
        CombatScript.Enemy[CombatScript.targetedEnemy].GainSlow(BolaShotSlow(level));
    }

    int BolaShotDamage(int level)
    {
        tempi = 10 + strength;
        tempi += 3 * level;
        return tempi;
    }

    int BolaShotSlow(int level)
    {
        tempi = 2;
        tempi += 1 * level;
        return tempi;
    }

    void CripplingStrike(int level) // ID 5
    {
        CombatScript.Enemy[CombatScript.targetedEnemy].TakeDamage(CripplingStrikeDamage(level));
        CombatScript.Enemy[CombatScript.targetedEnemy].GainBleed(CripplingStrikeBleed(level));
    }

    int CripplingStrikeDamage(int level)
    {
        tempi = 7 + strength;
        tempi += 2 * level;
        return tempi;
    }

    int CripplingStrikeBleed(int level)
    {
        tempi = 4;
        tempi += 2 * level;
        return tempi;
    }
}
