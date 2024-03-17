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
    int tempi, tempi2;

    //[Header("Additional Stats")]
    //public int baseHealth;

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
        if (PlayerScript.Item[18] && !Library.Enemies[ID].boss)
            level++;
        Reset();
        maxHealth = LevelCalculated(Library.Enemies[ID].UnitHealth);
        health = maxHealth;
        //baseHealth = maxHealth;
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
        if (unitID == 11)
            slow = LevelCalculated(36);
        if (PlayerScript.Item[15])
        {
            GainDaze(5);
            GainSlow(2);
        }
        if (PlayerScript.Item[24])
            GainBleed(3);
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
            effect[4] += effect[12] - 1;
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
        for (int i = 0; i < StatusObjects.Length; i++)
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
            effect[10]--;
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
            else if (unitID == 7 && currentMove == 0)
                currentMoveValue = EyeForAnEyeDamage();
            else currentMoveValue = movesValue[currentMove];
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
        if (effect[0] > 0 && stunned)
            effect[0]++;
        if (effect[5] > 0)
            CombatScript.Player.TakeMagicDamage(effect[5]);
        if (effect[6] > 0)
            CombatScript.Player.LoseSanity(effect[6]);
        if (effect[13] > 0)
            GainBlock(effect[13]);
        if (effect[18] > 0)
            LoseSlow(effect[18]);
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
                case (0, 1): // Strike & Defend
                    CombatScript.Player.TakeDamage(AttackDamage());
                    OnHit();
                    GainBlock(LevelCalculated(9));
                    break;
                case (0, 2): // Enrage
                    GainStrength(LevelCalculated(2));
                    GainBlock(LevelCalculated(5));
                    break;
                case (1, 1): // Cannibalize
                    if (AttackDamage() > CombatScript.Player.TotalBlock())
                        RestoreHealth(AttackDamage() - CombatScript.Player.TotalBlock());
                    CombatScript.Player.TakeDamage(AttackDamage());
                    OnHit();
                    GainStrength(LevelCalculated(1));
                    break;
                case (1, 2): // Only Purpose
                    CombatScript.Player.TakeDamage(AttackDamage());
                    OnHit();
                    CombatScript.Player.TakeDamage(AttackDamage());
                    OnHit();
                    CombatScript.Player.LoseSanity(Random.Range(LevelCalculated(3), LevelCalculated(5)));
                    break;
                case (2, 0): // Heavy Strike
                    CombatScript.Player.TakeDamage(AttackDamage());
                    OnHit();
                    GainSlow(1);
                    break;
                case (2, 1): // Dried Hide
                    GainBlock(LevelCalculated(12 + tenacity));
                    GainShield(LevelCalculated(4 + 2 * tenacity));
                    break;
                case (2, 2): // Haunting Wail
                    CombatScript.Player.TakeDamage(AttackDamage());
                    OnHit();
                    CombatScript.Player.GainWeak(1);
                    CombatScript.Player.LoseSanity(Random.Range(LevelCalculated(6), LevelCalculated(11)));
                    break;
                case (2, 3): // Fly Swarm
                    CombatScript.Player.TakeDamage(AttackDamage());
                    OnHit();
                    effect[5] += LevelCalculated(1);
                    Display(LevelCalculated(1), effectSprite[5]);
                    UpdateInfo();
                    break;
                case (3, 0): // Reap their Skin
                    CombatScript.Player.TakeDamage(AttackDamage());
                    OnHit();
                    RestoreHealth(LevelCalculated(8 + (2 * effect[3]) / 3));
                    break;
                case (3, 1): // Wither
                    CombatScript.Player.GainSlow(2);
                    CombatScript.Player.LoseSanity(Random.Range(LevelCalculated(7), LevelCalculated(14)));
                    break;
                case (3, 2): // Tear Flesh
                    if (AttackDamage() > CombatScript.Player.TotalBlock() + 1)
                        CombatScript.Player.GainBleed((AttackDamage() - CombatScript.Player.TotalBlock()) / 2);
                    CombatScript.Player.TakeDamage(AttackDamage());
                    OnHit();
                    break;
                case (4, 1): // Blind Rage
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
                case (4, 2): // Fuel Pump
                    GainBlock(LevelCalculated(27));
                    if (effect[10] > 0)
                    {
                        effect[10] += LevelCalculated(1);
                        Display(LevelCalculated(1), effectSprite[10]);
                        UpdateInfo();
                    }
                    else GainStrength(LevelCalculated(4));
                    break;
                case (4, 3): // Rusty Blade
                    CombatScript.Player.TakeDamage(AttackDamage());
                    OnHit();
                    CombatScript.Player.GainFrail(LevelCalculated(1));
                    CombatScript.Player.LoseSanity(Random.Range(LevelCalculated(4), LevelCalculated(7)));
                    GainShield(LevelCalculated(6 + 3 * effect[10]));
                    break;
                case (5, 1): // Slash
                    CombatScript.Player.TakeDamage(AttackDamage());
                    OnHit();
                    CombatScript.Player.GainBleed(LevelCalculated(4 + effect[3] / 2));
                    CombatScript.Player.LoseSanity(Random.Range(LevelCalculated(3), LevelCalculated(6)));
                    break;
                case (5, 2): // Might of the Grave
                    GainStrength(LevelCalculated(3 + effect[3] / 17));
                    effect[11] += LevelCalculated(3 + effect[3] / 13);
                    Display(LevelCalculated(3 + effect[3] / 13), effectSprite[11]);
                    tenacity += LevelCalculated(1);
                    UpdateInfo();
                    break;
                case (6, 0): // Void Strike
                    CombatScript.Player.TakeDamage(AttackDamage());
                    OnHit();
                    GainStrength(LevelCalculated(2 + CursesOnPlayer()));
                    break;
                case (6, 1): // Cryptic Gaze
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
                case (6, 2): // Tentacle Slam
                    CombatScript.Player.TakeDamage(AttackDamage());
                    OnHit();
                    GainDaze(AttackDamage() * tenacity / 72);
                    GainSlow(2);
                    break;
                case (6, 3): // Void Barrier
                    GainBlock(LevelCalculated(17 + 4 * effect[6]));
                    effect[6] += LevelCalculated(2 + CursesOnPlayer());
                    Display(LevelCalculated(2 + CursesOnPlayer()), effectSprite[6]);
                    UpdateInfo();
                    break;
                case (7, 0): // Eye for an Eye
                    CombatScript.Player.TakeDamage(AttackDamage());
                    OnHit();
                    CombatScript.Player.GainBleed(LevelCalculated(2 + (effect[1] * 1) / 6));
                    CombatScript.Player.LoseSanity(Random.Range(LevelCalculated(4), LevelCalculated(7)));
                    break;
                case (7, 1): // No Regrets
                    CombatScript.Player.TakeDamage(AttackDamage());
                    OnHit();
                    GainBleed(LevelCalculated(3));
                    break;
                case (7, 2): // Empty Stare
                    CombatScript.Player.GainFrail(LevelCalculated(1));
                    CombatScript.Player.GainTerror(LevelCalculated(2));
                    CombatScript.Player.LoseSanity(Random.Range(LevelCalculated(7), LevelCalculated(14)));
                    break;
                case (7, 3): // Flesh Hide
                    if (25 > effect[1] / 2)
                        GainBlock(LevelCalculated(25 - effect[1] / 2));
                    GainShield(LevelCalculated(3 + (effect[1] * 23) / 25));
                    break;
                case (8, 0): // Chain Lash
                    CombatScript.Player.TakeDamage(AttackDamage());
                    OnHit();
                    GainBlock(LevelCalculated(9 + (effect[3] * 11) / 4));
                    break;
                case (8, 1): // Flay
                    for (int i = 0; i < 3; i++)
                    {
                        CombatScript.Player.TakeDamage(AttackDamage());
                        OnHit();
                    }
                    break;
                case (8, 2): // Chain Up
                    CombatScript.Player.GainBleed(LevelCalculated(1 + (effect[3] * 1) / 3));
                    CombatScript.Player.GainSlow(LevelCalculated(1));
                    CombatScript.Player.GainWeak(LevelCalculated(1));
                    CombatScript.Player.LoseSanity(Random.Range(LevelCalculated(4), LevelCalculated(7)));
                    break;
                case (9, 0): // Lambs to the Slaughter
                    if (AttackDamage() > CombatScript.Player.TotalBlock())
                        GainHealth(AttackDamage() - CombatScript.Player.TotalBlock());
                    CombatScript.Player.TakeDamage(AttackDamage());
                    OnHit();
                    effect[14] += 1;
                    Display(1, effectSprite[14]);
                    CombatScript.Player.LoseSanity(Random.Range(LevelCalculated(9), LevelCalculated(13)));
                    break;
                case (9, 1): // Pulverize
                    if (AttackDamage() > CombatScript.Player.TotalBlock())
                        GainHealth(AttackDamage() - CombatScript.Player.TotalBlock());
                    CombatScript.Player.TakeDamage(AttackDamage());
                    OnHit();
                    CombatScript.Player.GainSlow(LevelCalculated(1));
                    CombatScript.Player.GainTerror(LevelCalculated(1));
                    CombatScript.Player.LoseSanity(Random.Range(LevelCalculated(7), LevelCalculated(12)));
                    GainLink();
                    break;
                case (9, 2): // Chop Chop
                    for (int i = 0; i < 2; i++)
                    {
                        if (AttackDamage() > CombatScript.Player.TotalBlock())
                            GainHealth(AttackDamage() - CombatScript.Player.TotalBlock());
                        CombatScript.Player.TakeDamage(AttackDamage());
                        OnHit();
                    }
                    GainHealth(LevelCalculated(3 + (effect[3] * 5) / 9));
                    tempi = (maxHealth - 720) / 20 - 1;
                    if (tempi > 0)
                        GainStrength(LevelCalculated(tempi));
                    break;
                case (9, 3): // Stitched Up
                    tempi = maxHealth - health;
                    if (tempi > 0)
                    {
                        RestoreHealth(tempi);
                        tempi2 = tempi / 11 - 8;
                        if (tempi2 > 0)
                            GainBleed(tempi2);
                        tempi2 = tempi / 76 - 2;
                        if (tempi2 > 0)
                            GainVulnerable(tempi2);
                        tempi2 = (tempi + 30) / 300;
                        tenacity -= tempi2;
                        if (tenacity < 1)
                            tenacity = 1;
                    }
                    break;
                case (10, 0): // Obliterate
                    CombatScript.Player.TakeDamage(AttackDamage());
                    OnHit();
                    GainSlow(2 + (tenacity * 2) / 5);
                    break;
                case (10, 1): // Death Sentence
                    tempi = LevelCalculated(5);
                    if (CombatScript.Player.effect[10] + tempi > 5)
                    {
                        tempi2 = 0;
                        while (CombatScript.Player.effect[10] + tempi > 5)
                        {
                            tempi -= 2;
                            tempi2 += 1;
                        }
                        if (tempi > 0)
                            CombatScript.Player.GainFrail(tempi);
                        CombatScript.Player.GainVulnerable(tempi2);
                        CombatScript.Player.LoseSanity(Random.Range(LevelCalculated(1 + 3 * tempi2), LevelCalculated(3 + 4 * tempi2)));
                        UpdateInfo();
                    }
                    else
                    {
                        CombatScript.Player.GainFrail(tempi);
                        CombatScript.Player.LoseSanity(Random.Range(LevelCalculated(1), LevelCalculated(3)));
                    }
                    break;
                case (10, 3): // Indestructible
                    GainShield(LevelCalculated(70));
                    movesCooldowns[3]++;
                    break;
                case (11, 0): // Strange Potion
                    CombatScript.Player.TakeDamage(AttackDamage());
                    OnHit();
                    tempi = Random.Range(1, 3);
                    CombatScript.Player.GainSlow(LevelCalculated(tempi));
                    tempi = Random.Range(1, 3);
                    CombatScript.Player.GainWeak(LevelCalculated(tempi));
                    break;
                case (11, 1): // Experimental Concoction
                    tempi = Random.Range(0, 3);
                    if (tempi > 0)
                        GainStrength(LevelCalculated(tempi));
                    tempi = Random.Range(2, 9);
                    GainHealth(LevelCalculated(tempi));
                    effect[18] += 1;
                    Display(1, effectSprite[18]);
                    break;
                case (11, 2): // Succumb
                    GainBlock(LevelCalculated(42));
                    tempi = Random.Range(1, 4 + effect[18] / 5);
                    LoseSlow(LevelCalculated(tempi));
                    break;
                case (12, 0): // Smash
                    CombatScript.Player.TakeDamage(AttackDamage());
                    OnHit();
                    CombatScript.Player.GainFrail(LevelCalculated(1 + effect[3] / 5));
                    break;
                case (12, 1): // Feral Howl
                    CombatScript.Player.GainWeak(LevelCalculated(2));
                    CombatScript.Player.GainTerror(LevelCalculated(2));
                    CombatScript.Player.LoseSanity(Random.Range(LevelCalculated(8), LevelCalculated(21)));
                    break;
                case (12, 2): // Onslaught
                    CombatScript.Player.TakeDamage(AttackDamage());
                    OnHit();
                    GainStrength(LevelCalculated(5));
                    tenacity += LevelCalculated(1);
                    //effect[4] += LevelCalculated(2);
                    //Display(LevelCalculated(2), effectSprite[4]);
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
        while (slow >= tenacity && !stunned)
        {
            slow -= tenacity;
            tenacity += 1 + effect[8];
            if (effect[2] > 0)
                TakeDamage(effect[2]);
            if (PlayerScript.Item[26])
                GainDaze(tenacity);
            if (PlayerScript.Item[13])
                TakeDamage(6);
            if (effect[16] > 0)
                TakeDamage(maxHealth / 20);
            else stunned = true;
        }
        UpdateInfo();
    }

    public int AttackDamage()
    {
        return DamageDealtModifier(currentMoveValue);
    }

    public int FlySwarmDamage()
    {
        tempi = movesValue[currentMove] + LevelCalculated(effect[5] * 3);
        return tempi;
    }

    public int TentacleSlamDamage()
    {
        tempi = movesValue[currentMove] + effect[3] * 3;
        return tempi;
    }

    public int EyeForAnEyeDamage()
    {
        tempi = movesValue[currentMove] + LevelCalculated((effect[1] * 5) / 9);
        return tempi;
    }

    int DamageDealtModifier(int value)
    {
        value += effect[3];
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
        if (CombatScript.Player.effect[16] > 0)
        {
            value *= 10 + CombatScript.Player.effect[16];
            value /= 10;
        }
        if (PlayerScript.Item[14])
        {
            if (effect[0] > 0)
            {
                value *= 3;
                value /= 5;
            }
            else
            {
                value *= 19;
                value /= 20;
            }
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
        if (PlayerScript.Item[31] && amount >= 32 && !CombatScript.Player.resistanceRing)
            CombatScript.Player.RingOfResistance();
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

    void GainHealth(int amount)
    {
        maxHealth += amount;
        health += amount;
        Display(amount, HealthSprite);
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

    void LoseSlow(int amount)
    {
        slow -= amount;
        if (slow <= 0 && unitID == 11)
            UnleashMonster();
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
        if (PlayerScript.Item[25] && amount >= 4)
            amount++;
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

    public void GainLink()
    {
        effect[15] += effect[14];
        while (effect[15] >= 9)
        {
            effect[15] -= 9;
            GainStrength(2);
        }
        UpdateInfo();
    }

    public bool IntentToAttack()
    {
        if (stunned)
            return false;
        else return attackIntentions[currentMove];
    }

    int CursesOnPlayer()
    {
        return CombatScript.Player.PlayerScript.CursesCount;
    }

    int LevelCalculated(int value)
    {
        value *= (14 + level);
        value /= 14;
        return value;
    }

    void UnleashMonster()
    {
        health += maxHealth;
        maxHealth *= 2;
        health += LevelCalculated(29);
        maxHealth += LevelCalculated(29);
        slow = 0;
        tenacity /= 4;
        if (!stunned)
            stunned = true;
        effect[3] *= 2;
        effect[3] += LevelCalculated(4);
        effect[4] = LevelCalculated(2);
        effect[8] = LevelCalculated(1);
        effect[18] = 0;
        unitID++;
        UnitSprite.sprite = Library.Enemies[unitID].UnitSprite;
        for (int i = 0; i < movesCount; i++)
        {
            MovesSprites[i] = Library.Enemies[unitID].MovesSprite[i];
            movesValue[i] = LevelCalculated(Library.Enemies[unitID].MovesValues[i]);
            movesText[i] = Library.Enemies[unitID].additionalText[i];
            attackIntentions[i] = Library.Enemies[unitID].attackIntention[i];
            normalAttacks[i] = Library.Enemies[unitID].normalAttack[i];
        }
    }

    // checks
    public int TotalBlock()
    {
        tempi = shield + block;
        return tempi;
    }
}
