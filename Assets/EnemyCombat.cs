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
    public int health, slow, tenacity, attackDamage;

    [Header("UI")]
    public Image HealthBarFil;
    public Image StunBarFill;
    public TMPro.TextMeshProUGUI HealthValue, SlowVale, AttackValue;

    void Start()
    {
        UpdateInfo();
    }

    void UpdateInfo()
    {
        HealthBarFil.fillAmount = (health * 1f) / (maxHealth * 1f);
        StunBarFill.fillAmount = (slow * 1f) / (tenacity * 1f);
        HealthValue.text = health.ToString("") + "/" + maxHealth.ToString("");
        SlowVale.text = slow.ToString("") + "/" + tenacity.ToString("");
        AttackValue.text = attackDamage.ToString("");
    }

    public void EndTurn()
    {
        CombatScript.Player.TakeDamage(attackDamage);
    }

    public void TakeDamage(int amount)
    {
        health -= amount;
        UpdateInfo();
    }
}
