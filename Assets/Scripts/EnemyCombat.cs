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
    public int[] effect;
    bool stunned;
    int tempi;

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

    [Header("Display")]
    public GameObject DisplayObject;
    public GameObject[] StatusObjects;
    public Image[] StatusImages;
    public TMPro.TextMeshProUGUI[] StatusValues;
    public Rigidbody2D Body;
    public Transform Origin;
    public Display Displayed;
    public Sprite DamageSprite, HealthSprite, ShieldSprite, BreakSprite, BlockSprite, SlowSprite;
    public Sprite[] effectSprite;
    public int[] effectsActive;
    int statusCount;

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
        else AttackValue.text = AttackDamage().ToString("") + movesText[currentMove];

        // status effects
        statusCount = 0;
        for (int i = 0; i < effect.Length; i++)
        {
            StatusObjects[i].SetActive(false);
        }
        for (int i = 0; i < effect.Length; i++)
        {
            if (effect[i] != 0)
            {
                effectsActive[statusCount] = i;
                StatusObjects[statusCount].SetActive(true);
                StatusImages[statusCount].sprite = effectSprite[i];
                StatusValues[statusCount].text = effect[i].ToString("0");
                statusCount++;
            }
        }
    }

    void Display(int amount, Sprite sprite)
    {
        Origin.rotation = Quaternion.Euler(Origin.rotation.x, Origin.rotation.y, Body.rotation + Random.Range(-25f, 25f));
        GameObject display = Instantiate(DisplayObject, Origin.position, transform.rotation);
        Displayed = display.GetComponent(typeof(Display)) as Display;
        Displayed.DisplayThis(amount, sprite);
        Rigidbody2D display_body = display.GetComponent<Rigidbody2D>();
        display_body.AddForce(Origin.up * Random.Range(1.75f, 2.5f), ForceMode2D.Impulse);
    }

    public void StartTurn()
    {
        if (effect[0] > 0)
            effect[0]--;
        currentMove = Random.Range(0, movesCount);
        IntentionSprite.sprite = MovesSprites[currentMove];
        if (attackIntentions[currentMove])
            AttackValue.text = AttackDamage().ToString("") + movesText[currentMove];
        else AttackValue.text = "";
    }

    public void EndTurn()
    {
        if (effect[1] > 0)
            TakeDamage(effect[1]);
    }

    public void Move()
    {
        if (stunned)
        {
            stunned = false;
            if (slow >= tenacity)
            {
                Stunned();
            }
        }
        else CombatScript.Player.TakeDamage(AttackDamage());
    }

    void Stunned()
    {
        stunned = true;
        slow -= tenacity;
        tenacity++;
        if (effect[2] > 0)
            TakeDamage(effect[2]);
        UpdateInfo();
    }

    public int AttackDamage()
    {
        if (effect[0] > 0)
            return (movesValue[currentMove] * 3) / 4;
        else return movesValue[currentMove];
    }

    public void TakeDamage(int amount)
    {
        Display(amount, DamageSprite);
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
        tempi = 0;
        if (block > 0)
        {
            if (block > amount)
            {
                Display(block, BreakSprite);
                block -= amount;
                amount = 0;
            }
            else
            {
                tempi += amount;
                amount -= block;
                block = 0;
            }
        }
        if (shield > 0)
        {
            if (shield > amount)
            {
                tempi += shield;
                Display(tempi, BreakSprite);
                shield -= amount;
                amount = 0;
            }
            else
            {
                tempi += amount;
                Display(tempi, BreakSprite);
                amount -= shield;
                shield = 0;
            }
        }
        UpdateInfo();
    }

    public void GainSlow(int amount)
    {
        slow += amount;
        Display(amount, SlowSprite);
        if (slow >= tenacity && !stunned)
        {
            Stunned();
        }
        UpdateInfo();
    }

    public void GainWeak(int amount)
    {
        Display(amount, effectSprite[0]);
        effect[0] += amount;
        UpdateInfo();
    }

    public void GainBleed(int amount)
    {
        Display(amount, effectSprite[1]);
        effect[1] += amount;
        UpdateInfo();
    }

    public void GainDaze(int amount)
    {
        Display(amount, effectSprite[2]);
        effect[2] += amount;
        UpdateInfo();
    }

    public bool IntentToAttack()
    {
        return attackIntentions[currentMove];
    }
}
