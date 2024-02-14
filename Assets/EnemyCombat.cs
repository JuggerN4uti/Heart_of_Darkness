using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyCombat : MonoBehaviour
{
    [Header("Scripts")]
    public Combat CombatScript;

    [Header("Stats")]
    public int maxHealth;
    public int health, shield, block, slow, tenacity, attackDamage;
    public int bleed;

    [Header("UI")]
    public GameObject ShieldDisplay;
    public GameObject BlockDisplay;
    public Image HealthBarFil;
    public Image StunBarFill;
    public TMPro.TextMeshProUGUI HealthValue, ShieldValue, BlockValue, SlowVale, AttackValue;

    void Start()
    {
        UpdateInfo();
    }

    void UpdateInfo()
    {
        HealthBarFil.fillAmount = (health * 1f) / (maxHealth * 1f);
        StunBarFill.fillAmount = (slow * 1f) / (tenacity * 1f);
        HealthValue.text = health.ToString("") + "/" + maxHealth.ToString("");
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
        SlowVale.text = slow.ToString("") + "/" + tenacity.ToString("");
        AttackValue.text = attackDamage.ToString("");
    }

    public void EndTurn()
    {
        if (bleed > 0)
            TakeDamage(bleed);
    }

    public void Move()
    {
        CombatScript.Player.TakeDamage(attackDamage);
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

    public void BreakShield(int amount)
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
        UpdateInfo();
    }

    public void GainSlow(int amount)
    {
        slow += amount;
        UpdateInfo();
    }

    public void GainBleed(int amount)
    {
        bleed += amount;
        UpdateInfo();
    }
}
