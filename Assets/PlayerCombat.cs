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
    public int health, energy, attacks, mana, attackDamage;
    public int strength, resistance;
    int tempi;

    [Header("UI")]
    public Image HealthBarFil;
    public Image EnergyBarFill;
    public TMPro.TextMeshProUGUI HealthValue, EnergyValue, ManaValue, AttackValue;

    void Start()
    {
        StartTurn();
    }

    public void StartTurn()
    {
        attacks = 1;
        mana = 3;
        Cards.Draw(5);
        UpdateInfo();
    }

    void UpdateInfo()
    {
        HealthBarFil.fillAmount = (health * 1f) / (maxHealth * 1f);
        EnergyBarFill.fillAmount = (energy * 1f) / 10f;
        HealthValue.text = health.ToString("") + "/" + maxHealth.ToString("");
        EnergyValue.text = energy.ToString("") + "/10";
        ManaValue.text = mana.ToString("");
        if (attacks > 1)
            AttackValue.text = attackDamage.ToString("") + " x" + attacks.ToString("");
        else AttackValue.text = attackDamage.ToString("");
    }

    public void EndTurn()
    {
        for (int i = 0; i < attacks; i++)
        {
            BasicAttack();
        }
    }

    void BasicAttack()
    {
        CombatScript.Enemy[CombatScript.targetedEnemy].TakeDamage(attackDamage);
    }

    public void TakeDamage(int amount)
    {
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
                return "Break " + SpearThrustBreak().ToString("") + "\n Deal " + SpearThrustDamage().ToString("") + " Damage";
            case 3:
                return "Deal " + JudgementDamage().ToString("") + " Damage";
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
        //GainBlock(DefendBlock());
    }

    int DefendBlock()
    {
        tempi = 8 + resistance;
        return tempi;
    }

    void SpearThrust() // ID 2
    {
        // Break Shield
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
}
