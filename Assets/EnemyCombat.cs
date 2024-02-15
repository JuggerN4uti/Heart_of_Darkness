using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyCombat : MonoBehaviour
{
    [Header("Scripts")]
    public Combat CombatScript;
    public EnemiesLibrary Library;

    [Header("Stats")]
    public int unitID;
    public int maxHealth, health, shield, block, slow, tenacity;
    // status effects:
    public int weak, bleed, daze;
    bool stunned;

    [Header("Moves")]
    public int[] movesValue;
    public int movesCount, currentMove;
    public bool[] attackIntentions;
    public string[] movesText;
    public Image IntentionSprite;
    public Sprite[] MovesSprites;
    public Sprite StunSprite;
    public TMPro.TextMeshProUGUI AttackValue;

    [Header("UI")]
    public GameObject ShieldDisplay;
    public GameObject BlockDisplay;
    public Image UnitSprite, HealthBarFil, StunBarFill;
    public TMPro.TextMeshProUGUI HealthValue, ShieldValue, BlockValue, SlowVale;

    void Start()
    {
        SetUnit(unitID);
        StartTurn();
        UpdateInfo();
    }

    void SetUnit(int ID)
    {
        maxHealth = Library.Enemies[ID].UnitHealth;
        health = maxHealth;
        shield = Library.Enemies[ID].UnitShield;
        UnitSprite.sprite = Library.Enemies[ID].UnitSprite;
        movesCount = Library.Enemies[ID].MovesCount;
        for (int i = 0; i < movesCount; i++)
        {
            MovesSprites[i] = Library.Enemies[ID].MovesSprite[i];
            movesValue[i] = Library.Enemies[ID].MovesValues[i];
            movesText[i] = Library.Enemies[ID].additionalText[i];
            attackIntentions[i] = Library.Enemies[ID].attackIntention[i];
        }
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
        if (stunned)
        {
            IntentionSprite.sprite = StunSprite;
            AttackValue.text = "";
        }
    }

    public void StartTurn()
    {
        if (weak > 0)
            weak--;
        currentMove = Random.Range(0, movesCount);
        IntentionSprite.sprite = MovesSprites[currentMove];
        if (attackIntentions[currentMove])
            AttackValue.text = AttackDamage().ToString("") + movesText[currentMove];
        else AttackValue.text = "";
    }

    public void EndTurn()
    {
        if (bleed > 0)
            TakeDamage(bleed);
    }

    public void Move()
    {
        if (stunned)
            stunned = false;
        else CombatScript.Player.TakeDamage(AttackDamage());
    }

    void Stunned()
    {
        stunned = true;
        slow -= tenacity;
        tenacity++;
        if (daze > 0)
            TakeDamage(daze);
        UpdateInfo();
    }

    public int AttackDamage()
    {
        if (weak > 0)
            return (movesValue[currentMove] * 3) / 4;
        else return movesValue[currentMove];
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
        if (slow >= tenacity && !stunned)
        {
            Stunned();
        }
        UpdateInfo();
    }

    public void GainWeak(int amount)
    {
        weak += amount;
        UpdateInfo();
    }

    public void GainBleed(int amount)
    {
        bleed += amount;
        UpdateInfo();
    }

    public void GainDaze(int amount)
    {
        daze += amount;
        UpdateInfo();
    }

    public bool IntentToAttack()
    {
        return attackIntentions[currentMove];
    }
}
