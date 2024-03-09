using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyCombat : MonoBehaviour
{
    [Header("Scripts")]
    public Combat CombatScript;
    public Player PlayerScript;
    public EnemiesLibrary Library;

    [Header("Stats")]
    public int unitID;
    public int order, level, maxHealth, health, shield, block, slow, tenacity;
    public int[] effect;
    bool stunned;
    int tempi;

    [Header("Moves")]
    public int[] movesValue;
    public int[] movesCooldowns, moveCooldown;
    public int movesCount, currentMove, currentMoveValue;
    public bool[] attackIntentions, normalAttacks;
    public bool viable;
    public string[] movesText;
    public Image IntentionSprite;
    public Sprite[] MovesSprites;
    public Sprite StunSprite;
    public TMPro.TextMeshProUGUI AttackValue;

    [Header("UI")]
    public GameObject Unit;
    public GameObject ShieldDisplay, BlockDisplay;
    public Image UnitSprite, HealthBarFil, StunBarFill;
    public TMPro.TextMeshProUGUI LevelValue, HealthValue, ShieldValue, BlockValue, SlowVale;

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
        //SetUnit(unitID);
        //StartTurn();
        UpdateInfo();
    }

    public void SetUnit(int ID, int Level = 0)
    {
        unitID = ID;
        level = Level;
        if (PlayerScript.Item[18])
            level++;
        Reset();
        maxHealth = LevelCalculated(Library.Enemies[ID].UnitHealth);
        health = maxHealth;
        shield = LevelCalculated(Library.Enemies[ID].UnitShield);
        tenacity = LevelCalculated(Library.Enemies[ID].UnitTenacity);
        UnitSprite.sprite = Library.Enemies[ID].UnitSprite;
        movesCount = Library.Enemies[ID].MovesCount;
        for (int i = 0; i < movesCount; i++)
        {
            movesCooldowns[i] = Library.Enemies[ID].MovesCooldowns[i];
            moveCooldown[i] = Library.Enemies[ID].MovesCooldowns[i];
            MovesSprites[i] = Library.Enemies[ID].MovesSprite[i];
            movesValue[i] = LevelCalculated(Library.Enemies[ID].MovesValues[i]);
            movesText[i] = Library.Enemies[ID].additionalText[i];
            attackIntentions[i] = Library.Enemies[ID].attackIntention[i];
            normalAttacks[i] = Library.Enemies[ID].normalAttack[i];
        }
        if (PlayerScript.Item[14])
            GainWeak(1);
        if (PlayerScript.Item[14])
        {
            GainDaze(5);
            GainSlow(2);
        }
        LevelValue.text = (level + 1).ToString("0");
        UpdateInfo();
        StartTurn();
    }

    void Reset()
    {
        block = 0;
        slow = 0;
        for (int i = 0; i < Library.Enemies[unitID].StartingEffects.Length; i++)
        {
            effect[i] = LevelCalculated(Library.Enemies[unitID].StartingEffects[i]);
        }
        if (effect[12] > 0)
        {
            effect[3] += (2 * effect[12]) - 2;
            effect[12] = 1;
        }
        stunned = false;
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
        else if (attackIntentions[currentMove])
            DisplayMove();
        else AttackValue.text = "";

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
        if (effect[7] > 0)
        {
            GainBlock(effect[7] * 3);
            GainStrength(effect[7]);
            GainSlow(1);
        }
        if (effect[10] > 0)
        {
            effect[10]--;
            if (effect[10] == 0)
            {
                GainSlow(2);
                GainVulnerable(1);
            }
        }
        UpdateInfo();
        if (!stunned)
        {
            SelectMove();
            DisplayMove();
        }
    }

    void SelectMove()
    {
        viable = false;
        do
        {
            currentMove = Random.Range(0, movesCount);
            moveCooldown[currentMove]--;
            if (moveCooldown[currentMove] == 0)
                viable = true;
        } while (!viable);

        moveCooldown[currentMove] += movesCooldowns[currentMove];
        IntentionSprite.sprite = MovesSprites[currentMove];

        if (attackIntentions[currentMove])
        {
            if (unitID == 2 && currentMove == 3)
                currentMoveValue = FlySwarmDamage();
            else if (unitID == 6 && currentMove == 2)
                currentMoveValue = TentacleSlamDamage();
            else currentMoveValue = movesValue[currentMove] + effect[3];
        }
    }

    void DisplayMove()
    {
        if (attackIntentions[currentMove])
            AttackValue.text = AttackDamage().ToString("") + movesText[currentMove];
        else AttackValue.text = "";
    }

    public void EndTurn()
    {
        if (effect[1] > 0)
            TakeDamage(effect[1]);
        if (block > 0)
            block = 0;
    }

    public void Move()
    {
        if (effect[5] > 0)
            CombatScript.Player.TakeMagicDamage(effect[5]);
        if (effect[6] > 0)
            CombatScript.Player.LoseSanity(effect[6]);
        if (stunned)
        {
            stunned = false;
            if (slow >= tenacity)
            {
                Stunned();
            }
        }
        else ExecuteMove();
    }

    void ExecuteMove()
    {
        if (normalAttacks[currentMove])
        {
            CombatScript.Player.TakeDamage(AttackDamage());
            OnHit();
        }
        else
        {
            switch (unitID, currentMove)
            {
                case (0, 1):
                    CombatScript.Player.TakeDamage(AttackDamage());
                    OnHit();
                    GainBlock(LevelCalculated(10));
                    break;
                case (0, 2):
                    GainStrength(LevelCalculated(2));
                    GainBlock(LevelCalculated(6));
                    break;
                case (1, 1):
                    if (AttackDamage() > CombatScript.Player.TotalBlock())
                        RestoreHealth(AttackDamage() - CombatScript.Player.TotalBlock());
                    CombatScript.Player.TakeDamage(AttackDamage());
                    OnHit();
                    GainStrength(LevelCalculated(1));
                    break;
                case (1, 2):
                    CombatScript.Player.TakeDamage(AttackDamage());
                    OnHit();
                    CombatScript.Player.TakeDamage(AttackDamage());
                    OnHit();
                    break;
                case (2, 0):
                    CombatScript.Player.TakeDamage(AttackDamage());
                    OnHit();
                    GainSlow(1);
                    break;
                case (2, 1):
                    GainBlock(LevelCalculated(13 + tenacity));
                    GainShield(LevelCalculated(5 + 2 * tenacity));
                    break;
                case (2, 2):
                    CombatScript.Player.TakeDamage(AttackDamage());
                    OnHit();
                    CombatScript.Player.GainWeak(1);
                    CombatScript.Player.LoseSanity(Random.Range(LevelCalculated(7), LevelCalculated(11)));
                    break;
                case (2, 3):
                    CombatScript.Player.TakeDamage(AttackDamage());
                    OnHit();
                    effect[5] += LevelCalculated(1);
                    Display(LevelCalculated(1), effectSprite[5]);
                    UpdateInfo();
                    break;
                case (3, 0):
                    CombatScript.Player.TakeDamage(AttackDamage());
                    OnHit();
                    RestoreHealth(LevelCalculated(9 + (2 * effect[3]) / 3));
                    break;
                case (3, 1):
                    CombatScript.Player.GainSlow(2);
                    CombatScript.Player.LoseSanity(Random.Range(LevelCalculated(5), LevelCalculated(9)));
                    break;
                case (3, 2):
                    if (AttackDamage() > CombatScript.Player.TotalBlock() + 1)
                        CombatScript.Player.GainBleed((AttackDamage() - CombatScript.Player.TotalBlock()) / 2);
                    CombatScript.Player.TakeDamage(AttackDamage());
                    OnHit();
                    break;
                case (4, 1):
                    CombatScript.Player.TakeDamage(AttackDamage());
                    OnHit();
                    CombatScript.Player.TakeDamage(AttackDamage());
                    OnHit();
                    if (effect[10] > 0)
                    {
                        effect[10]--;
                        Display(-1, effectSprite[10]);
                    }
                    else GainSlow(3);
                    break;
                case (4, 2):
                    GainBlock(LevelCalculated(28));
                    if (effect[10] > 0)
                    {
                        effect[10] += LevelCalculated(1);
                        Display(LevelCalculated(1), effectSprite[10]);
                        UpdateInfo();
                    }
                    else GainStrength(LevelCalculated(4));
                    break;
                case (4, 3):
                    CombatScript.Player.TakeDamage(AttackDamage());
                    OnHit();
                    CombatScript.Player.GainFrail(LevelCalculated(1));
                    GainShield(LevelCalculated(6 + 3 * effect[10]));
                    break;
                case (5, 1):
                    CombatScript.Player.TakeDamage(AttackDamage());
                    OnHit();
                    CombatScript.Player.GainBleed(LevelCalculated(4 + effect[3] / 2));
                    break;
                case (5, 2):
                    GainStrength(LevelCalculated(3 + effect[3] / 16));
                    effect[11] += LevelCalculated(3 + effect[3] / 12);
                    Display(LevelCalculated(3 + effect[3] / 13), effectSprite[11]);
                    tenacity += LevelCalculated(1);
                    UpdateInfo();
                    break;
                // Faceless One ID
                case (6, 0):
                    CombatScript.Player.TakeDamage(AttackDamage());
                    OnHit();
                    GainStrength(LevelCalculated(2 + CursesOnPlayer()));
                    break;
                case (6, 1):
                    CombatScript.Player.TakeDamage(AttackDamage());
                    OnHit();
                    for (int i = 0; i < LevelCalculated(3 + CursesOnPlayer()); i++)
                    {
                        tempi = Random.Range(0, 4);
                        switch (tempi)
                        {
                            case 0:
                                CombatScript.Player.GainWeak(1);
                                break;
                            case 1:
                                CombatScript.Player.GainSlow(1);
                                break;
                            case 2:
                                CombatScript.Player.GainFrail(1);
                                break;
                            case 3:
                                CombatScript.Player.GainTerror(1);
                                break;
                        }
                    }
                    CombatScript.Player.LoseSanity(Random.Range(LevelCalculated(13), LevelCalculated(20)));
                    break;
                case (6, 2):
                    CombatScript.Player.TakeDamage(AttackDamage());
                    OnHit();
                    GainDaze(AttackDamage() * tenacity / 72);
                    GainSlow(2);
                    break;
                case (6, 3):
                    GainBlock(LevelCalculated(17 + 4 * effect[6]));
                    effect[6] += LevelCalculated(2 + CursesOnPlayer());
                    Display(LevelCalculated(2 + CursesOnPlayer()), effectSprite[6]);
                    UpdateInfo();
                    break;
                case (7, 0):
                    CombatScript.Player.TakeDamage(AttackDamage());
                    OnHit();
                    CombatScript.Player.GainBleed(LevelCalculated(2 + (effect[1] * 6) / 25));
                    break;
                case (7, 1):
                    CombatScript.Player.TakeDamage(AttackDamage());
                    OnHit();
                    GainBleed(LevelCalculated(4));
                    break;
                case (7, 2):
                    CombatScript.Player.GainFrail(LevelCalculated(2));
                    CombatScript.Player.GainTerror(LevelCalculated(2));
                    CombatScript.Player.LoseSanity(Random.Range(LevelCalculated(10), LevelCalculated(20)));
                    break;
                case (7, 3):
                    if (37 > effect[1] / 2)
                        GainBlock(LevelCalculated(37 - effect[1] / 2));
                    GainShield(LevelCalculated(4 + (effect[1] * 17) / 13));
                    break;
            }
        }
    }

    void OnHit()
    {
        if (effect[4] > 0)
            CombatScript.Player.GainBleed(effect[4]);
        if (effect[11] > 0)
        {
            if (CombatScript.Player.TotalBlock() == 0)
                CombatScript.Player.TakeMagicDamage(effect[11]);
        }
    }

    void Stunned()
    {
        stunned = true;
        slow -= tenacity;
        tenacity += 1 + effect[8];
        if (effect[2] > 0)
            TakeDamage(effect[2]);
        UpdateInfo();
    }

    public int AttackDamage()
    {
        return DamageDealtModifier(currentMoveValue);
    }

    public int FlySwarmDamage()
    {
        tempi = movesValue[currentMove] + effect[3] + LevelCalculated(effect[5] * 3);
        return tempi;
    }

    public int TentacleSlamDamage()
    {
        tempi = movesValue[currentMove] + effect[3] * 4;
        return tempi;
    }

    public int EyeForAnEyeDamage()
    {
        tempi = movesValue[currentMove] + effect[3] + LevelCalculated((effect[1] * 7) / 10);
        return tempi;
    }

    int DamageDealtModifier(int value)
    {
        if (effect[0] > 0)
        {
            value *= 3;
            value /= 4;
        }
        if (effect[10] > 0)
        {
            value *= 32 + effect[10];
            value /= 25;
        }
        return value;
    }

    public void TakeDamage(int amount)
    {
        amount *= 10 + effect[9];
        amount /= 10;
        if (effect[12] > 0)
        {
            amount *= 3;
            amount /= 5;
            GainBleed(1);
        }
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
        if (health <= 0)
            Perish();
        UpdateInfo();
    }

    void Perish()
    {
        Unit.SetActive(false);
        CombatScript.EnemyDefeated(order);
    }

    public void BreakShield(int amount)
    {
        tempi = 0;
        if (block > 0)
        {
            if (block > amount)
            {
                tempi += amount;
                block -= amount;
                amount = 0;
            }
            else
            {
                tempi += block;
                amount -= block;
                block = 0;
            }
        }

        if (shield > 0 && amount > 0)
        {
            if (shield > amount)
            {
                tempi += amount;
                shield -= amount;
                amount = 0;
            }
            else
            {
                tempi += shield;
                shield = 0;
            }
        }

        if (tempi > 0)
            Display(tempi, BreakSprite);

        UpdateInfo();
    }

    void RestoreHealth(int amount)
    {
        health += amount;
        Display(amount, HealthSprite);
        if (health > maxHealth)
            health = maxHealth;
        UpdateInfo();
    }

    void GainBlock(int amount)
    {
        block += amount;
        Display(amount, BlockSprite);
        UpdateInfo();
    }

    void GainShield(int amount)
    {
        shield += amount;
        Display(amount, ShieldSprite);
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
        if (PlayerScript.Item[13])
            TakeDamage(6 * amount);
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

    public void GainStrength(int amount)
    {
        Display(amount, effectSprite[3]);
        effect[3] += amount;
        UpdateInfo();
    }

    public void GainVulnerable(int amount)
    {
        Display(amount, effectSprite[9]);
        effect[9] += amount;
        if (PlayerScript.Item[13])
            TakeDamage(6 * amount);
        UpdateInfo();
    }

    public bool IntentToAttack()
    {
        return attackIntentions[currentMove];
    }

    int CursesOnPlayer()
    {
        return CombatScript.Player.PlayerScript.CursesCount;
    }

    int LevelCalculated(int value)
    {
        value *= (16 + level);
        value /= 16;
        return value;
    }
}
