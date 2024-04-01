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
    bool stunned, slain;
    int tempi, tempi2;
    float temp;

    [Header("Additional Stats")]
    public float soulShards;
    public float unstableCharge;
    public int totalUnspentMana;

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
    public Transform Origin, Origin2;
    public Display Displayed;
    public Sprite DamageSprite, HealthSprite, RestoreSprite, ShieldSprite, BreakSprite, BlockSprite, SlowSprite, LevelSprite;
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
        maxHealth = LevelCalculatedDef(Library.Enemies[ID].UnitHealth);
        health = maxHealth;
        //baseHealth = maxHealth;
        shield = LevelCalculatedDef(Library.Enemies[ID].UnitShield);
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
        if (effect[18] > 5)
        {
            tenacity += effect[18] - 5;
            effect[18] = 5;
        }
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
        soulShards = 0f;
        unstableCharge = 0f;
        totalUnspentMana = 0;
        for (int i = 0; i < Library.Enemies[unitID].StartingEffects.Length; i++)
        {
            effect[i] = LevelCalculated(Library.Enemies[unitID].StartingEffects[i]);
        }
        if (effect[12] > 1)
        {
            effect[4] += effect[12] - 1;
            effect[12] = 1;
        }
        stunned = false;
        slain = false;
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

    public void Effect(GameObject effect, float rotation)
    {
        Origin.rotation = Quaternion.Euler(Origin.rotation.x, Origin.rotation.y, Body.rotation + rotation);
        GameObject display = Instantiate(effect, Origin2.position, Origin.rotation);
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
        if (effect[18] > 0)
        {
            effect[18]--;
            if (effect[18] == 0)
                UnleashMonster();
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
            if (moveCooldown[currentMove] <= 0)
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
            else if (unitID == 13 && currentMove == 1)
                currentMoveValue = PiercingPainDamage();
            else if (unitID == 14 && currentMove == 3)
                currentMoveValue = DamnationDamage();
            else if (unitID == 15 && currentMove == 0)
                currentMoveValue = GorgeDamage();
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
        {
            CombatScript.Effect(false, 1, false, order);
            TakeDamage(effect[1]);
        }
        if (PlayerScript.Item[43])
        {
            CombatScript.Effect(false, 5, true, order);
            TakeDamage(2 + CombatScript.turn / 3);
        }
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
        if (stunned)
        {
            stunned = false;
            if (slow >= tenacity)
            {
                Stunned();
            }
        }
        else ExecuteMove();
        if (effect[19] > 0)
        {
            if (CombatScript.Player.mana == 0)
            {
                GainStrength(effect[19]);
                GainShield(effect[19] * 3);
            }
        }
        if (effect[20] > 0)
        {
            if (CombatScript.Player.mana > 0)
            {
                GainStrength(2 * CombatScript.Player.mana);
                unstableCharge -= 0.05f * CombatScript.Player.mana * CombatScript.Player.mana;
            }
        }
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
                        tempi = AttackDamage() - CombatScript.Player.TotalBlock();
                    RestoreHealth(AttackDamage() / 5 + tempi);
                    CombatScript.Player.TakeDamage(AttackDamage());
                    OnHit();
                    GainStrength(LevelCalculated(1));
                    break;
                case (1, 2): // Only Purpose
                    CombatScript.Player.TakeDamage(AttackDamage());
                    OnHit();
                    CombatScript.Player.TakeDamage(AttackDamage());
                    OnHit();
                    CombatScript.Player.LoseSanity(Random.Range(3, 5));
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
                    CombatScript.Player.LoseSanity(Random.Range(6, 11));
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
                    CombatScript.Player.LoseSanity(Random.Range(6, 13));
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
                    else GainSlow(2);
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
                    CombatScript.Player.LoseSanity(Random.Range(4, 7));
                    GainShield(LevelCalculated(7 + 3 * effect[10]));
                    break;
                case (5, 1): // Slash
                    CombatScript.Player.TakeDamage(AttackDamage());
                    OnHit();
                    CombatScript.Player.GainBleed(LevelCalculated(3 + (effect[3] * 12) / 21));
                    CombatScript.Player.LoseSanity(Random.Range(3, 6));
                    break;
                case (5, 2): // Might of the Grave
                    GainStrength(LevelCalculated(3 + effect[3] / 15));
                    effect[11] += LevelCalculated(3 + effect[3] / 12);
                    Display(LevelCalculated(3 + effect[3] / 12), effectSprite[11]);
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
                    CombatScript.Player.LoseSanity(Random.Range(13, 21));
                    break;
                case (6, 2): // Tentacle Slam
                    CombatScript.Player.TakeDamage(AttackDamage());
                    OnHit();
                    GainDaze(AttackDamage() * tenacity / 70);
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
                    CombatScript.Player.GainBleed(LevelCalculated(2 + (effect[1] * 1) / 7));
                    CombatScript.Player.LoseSanity(Random.Range(4, 6));
                    break;
                case (7, 1): // No Regrets
                    CombatScript.Player.TakeDamage(AttackDamage());
                    OnHit();
                    GainBleed(LevelCalculated(3));
                    break;
                case (7, 2): // Empty Stare
                    CombatScript.Player.GainFrail(LevelCalculated(1));
                    CombatScript.Player.GainTerror(LevelCalculated(2));
                    CombatScript.Player.LoseSanity(Random.Range(7, 10));
                    break;
                case (7, 3): // Flesh Hide
                    if (23 > effect[1] / 2)
                        GainBlock(LevelCalculated(23 - effect[1] / 2));
                    GainShield(LevelCalculated(3 + (effect[1] * 23) / 25));
                    break;
                case (8, 0): // Chain Lash
                    CombatScript.Player.TakeDamage(AttackDamage());
                    OnHit();
                    GainBlock(LevelCalculated(9 + (effect[3] * 21) / 8));
                    break;
                case (8, 1): // Flay
                    for (int i = 0; i < 3; i++)
                    {
                        CombatScript.Player.TakeDamage(AttackDamage());
                        OnHit();
                    }
                    break;
                case (8, 2): // Chain Up
                    CombatScript.Player.GainBleed(LevelCalculated(1 + (effect[3] * 6) / 19));
                    CombatScript.Player.GainSlow(LevelCalculated(1));
                    CombatScript.Player.GainWeak(LevelCalculated(1));
                    CombatScript.Player.LoseSanity(Random.Range(4, 7));
                    break;
                case (9, 0): // Lambs to the Slaughter
                    if (AttackDamage() > CombatScript.Player.TotalBlock())
                        GainHealth(AttackDamage() - CombatScript.Player.TotalBlock());
                    CombatScript.Player.TakeDamage(AttackDamage());
                    OnHit();
                    effect[14] += 1;
                    Display(1, effectSprite[14]);
                    CombatScript.Player.LoseSanity(Random.Range(7, 13));
                    break;
                case (9, 1): // Pulverize
                    if (AttackDamage() > CombatScript.Player.TotalBlock())
                        GainHealth(AttackDamage() - CombatScript.Player.TotalBlock());
                    CombatScript.Player.TakeDamage(AttackDamage());
                    OnHit();
                    CombatScript.Player.GainSlow(LevelCalculated(1));
                    CombatScript.Player.GainTerror(LevelCalculated(1));
                    CombatScript.Player.LoseSanity(Random.Range(7, 12));
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
                    tempi = (maxHealth - 800) / 19 - 2;
                    if (tempi > 0)
                        GainStrength(LevelCalculated(tempi));
                    break;
                case (9, 3): // Stitched Up
                    tempi = maxHealth - health;
                    if (tempi > 0)
                    {
                        if (tempi > maxHealth / 2)
                            tempi = maxHealth / 2;
                        RestoreHealth(tempi);
                        tempi2 = tempi / 16 - 8;
                        if (tempi2 > 0)
                        {
                            GainBleed(tempi2);
                            GainDaze(tempi2);
                        }
                        tempi2 = tempi / 80 - 2;
                        if (tempi2 > 0)
                            GainVulnerable(tempi2);
                    }
                    break;
                case (10, 0): // Obliterate
                    CombatScript.Player.TakeDamage(AttackDamage());
                    OnHit();
                    GainSlow(2 + (tenacity * 5) / 12);
                    break;
                case (10, 1): // Death Sentence
                    tempi = LevelCalculated(5);
                    if (CombatScript.Player.effect[10] + tempi > 5)
                    {
                        tempi2 = 0;
                        while (CombatScript.Player.effect[10] + tempi > 5)
                        {
                            tempi -= 3;
                            tempi2 += 1;
                        }
                        if (tempi > 0)
                            CombatScript.Player.GainFrail(tempi);
                        CombatScript.Player.GainVulnerable(tempi2);
                        CombatScript.Player.LoseSanity(Random.Range(1 + 4 * tempi2, 3 + 6 * tempi2));
                        UpdateInfo();
                    }
                    else
                    {
                        CombatScript.Player.GainFrail(tempi);
                        CombatScript.Player.LoseSanity(Random.Range(1, 3));
                    }
                    GainDaze(LevelCalculated(2));
                    break;
                case (10, 3): // Indestructible
                    GainShield(LevelCalculated(66));
                    GainDaze(LevelCalculated(2 * movesCooldowns[3] - 3));
                    movesCooldowns[3]++;
                    moveCooldown[3]++;
                    break;
                case (11, 0): // Strange Potion
                    CombatScript.Player.TakeDamage(AttackDamage());
                    OnHit();
                    tempi = LevelCalculated(Random.Range(8, 13)) / 10;
                    if (tempi > 0)
                        CombatScript.Player.GainSlow(tempi);
                    tempi = LevelCalculated(Random.Range(9, 14)) / 10;
                    if (tempi > 0)
                        CombatScript.Player.GainWeak(tempi);
                    break;
                case (11, 1): // Experimental Concoction
                    tempi = Random.Range(0, 3);
                    if (tempi > 0)
                        GainStrength(LevelCalculated(tempi));
                    tempi = Random.Range(4, 10);
                    GainHealth(LevelCalculated(tempi));
                    break;
                case (11, 2): // Succumb
                    tempi = 16;
                    tempi2 = LevelCalculated(2);
                    if (slow >= tempi2)
                        slow -= tempi2;
                    else
                    {
                        tempi2 -= slow;
                        tempi += 6 * tempi2;
                        slow = 0;
                    }
                    GainBlock(LevelCalculated(tempi));
                    break;
                case (12, 0): // Smash
                    CombatScript.Player.TakeDamage(AttackDamage());
                    OnHit();
                    CombatScript.Player.GainFrail(LevelCalculated(1 + effect[3] / 6));
                    break;
                case (12, 1): // Feral Howl
                    CombatScript.Player.GainWeak(LevelCalculated(2));
                    CombatScript.Player.GainTerror(LevelCalculated(1));
                    CombatScript.Player.LoseSanity(Random.Range(10, 13));
                    break;
                case (12, 2): // Onslaught
                    CombatScript.Player.TakeDamage(AttackDamage());
                    OnHit();
                    GainStrength(LevelCalculated(4));
                    tenacity += LevelCalculated(1);
                    //effect[4] += LevelCalculated(2);
                    //Display(LevelCalculated(2), effectSprite[4]);
                    break;
                case (13, 0): // Soul Drain
                    if (AttackDamage() > CombatScript.Player.TotalBlock())
                        tempi = AttackDamage() - CombatScript.Player.TotalBlock();
                    else tempi = 0;
                    CombatScript.Player.TakeDamage(AttackDamage());
                    OnHit();
                    soulShards += AttackDamage() * 0.01f;
                    soulShards += tempi * 0.06f;
                    while (soulShards >= 1f)
                    {
                        soulShards -= 1f;
                        effect[19] += LevelCalculated(1);
                        Display(LevelCalculated(1), effectSprite[19]);
                    }
                    break;
                case (13, 1): // Piercing Pain
                    if (AttackDamage() > CombatScript.Player.TotalBlock())
                        tempi = AttackDamage() - CombatScript.Player.TotalBlock();
                    else tempi = 0;
                    CombatScript.Player.TakeDamage(AttackDamage());
                    OnHit();
                    CombatScript.Player.GainFrail(1);
                    if (tempi > 0)
                        CombatScript.Player.LoseSanity(Random.Range((tempi * 2) / 5, 1 + (tempi * 3) / 5));
                    break;
                case (13, 2): // Dark Shroud
                    GainShield(LevelCalculated(14));
                    CombatScript.Player.GainSlow(LevelCalculated(1));
                    CombatScript.Player.GainTerror(LevelCalculated(1));
                    break;
                case (14, 0): // Terrify
                    CombatScript.Player.TakeDamage(AttackDamage());
                    OnHit();
                    CombatScript.Player.GainTerror(LevelCalculated(2));
                    CombatScript.Player.LoseSanity(Random.Range(6 + CursesOnPlayer(), 13 + CursesOnPlayer()));
                    Unstable(0.15f);
                    break;
                case (14, 1): // Reap
                    if (AttackDamage() > CombatScript.Player.TotalBlock())
                        tempi = AttackDamage() - CombatScript.Player.TotalBlock();
                    else tempi = 0;
                    CombatScript.Player.TakeDamage(AttackDamage());
                    OnHit();
                    tempi2 = tempi / 6;
                    if (tempi2 > 0)
                        GainStrength(tempi2);
                    tempi2 = LevelCalculated(12);
                    tempi2 += (AttackDamage() * 2 + tempi * 3) / 6;
                    GainBlock(tempi2);
                    Unstable(0.2f + 0.018f * effect[3]);
                    break;
                case (14, 2): // More!
                    GainStrength(LevelCalculated(5 + effect[3] / 7));
                    effect[11] += LevelCalculated(2 + (effect[3] * 1) / 5);
                    Display(LevelCalculated(2 + effect[3] / 3), effectSprite[11]);
                    GainBlock(LevelCalculated(23));
                    Unstable(1.25f + 0.1f * effect[21]);
                    movesCooldowns[0] += 1;
                    moveCooldown[0] += 1;
                    break;
                case (14, 3): // Damnation
                    CombatScript.Player.TakeDamage(AttackDamage());
                    OnHit();
                    tempi = LevelCalculated(4 + totalUnspentMana);
                    tempi /= 10;
                    if (tempi > 0)
                        CombatScript.Player.GainVulnerable(tempi);
                    Unstable(0.24f + 0.028f * totalUnspentMana);
                    break;
                case (15, 0): // Gorge
                    CombatScript.Player.TakeDamage(AttackDamage());
                    OnHit();
                    GainHealth((AttackDamage() * 4) / 15);
                    temp = 1.03f + level * 0.15f;
                    tempi = 0;
                    while (temp >= 1f)
                    {
                        temp -= 1f;
                        tempi++;
                    }
                    GainLevel(tempi);
                    while (temp > 0.3f)
                    {
                        tempi = Random.Range(0, 3);
                        switch (tempi)
                        {
                            case 0:
                                GainStrength(2);
                                break;
                            case 1:
                                effect[4] += 1;
                                Display(1, effectSprite[4]);
                                break;
                            case 2:
                                tenacity++;
                                UpdateInfo();
                                break;
                        }
                        temp -= 0.3f;
                    }
                    movesCooldowns[0] += 3;
                    moveCooldown[0] += 3;
                    break;
                case (15, 1): // Hook
                    CombatScript.Player.TakeDamage(AttackDamage());
                    OnHit();
                    CombatScript.Player.GainBleed(LevelCalculated(8) / 3);
                    moveCooldown[0] -= LevelCalculated(2);
                    break;
                case (15, 3): // Vile Gas
                    CombatScript.Player.GainPoison(LevelCalculated(3) / 2);
                    CombatScript.Player.GainWeak(LevelCalculated(1));
                    CombatScript.Player.LoseSanity(Random.Range(5, 10));
                    moveCooldown[0] -= LevelCalculated(1);
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
        if (CombatScript.Player.effect[4] > 0)
            TakeDamage(CombatScript.Player.effect[4]);
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
            else if (effect[18] > 0)
                effect[18]++;
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
        tempi = movesValue[currentMove] + LevelCalculated((effect[1] * 9) / 17);
        return tempi;
    }

    public int PiercingPainDamage()
    {
        tempi = movesValue[currentMove] + effect[3] + CursesOnPlayer();
        return tempi;
    }

    public int DamnationDamage()
    {
        tempi = movesValue[currentMove] + LevelCalculated((totalUnspentMana * 21) / 41);
        return tempi;
    }

    public int GorgeDamage()
    {
        tempi = movesValue[currentMove] + LevelCalculated(CombatScript.Player.health / 8);
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
            value *= 60 + effect[10];
            value /= 50;
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
        if (PlayerScript.Item[30] && amount >= 22)
        {
            if (PlayerScript.Item[33])
                amount += 8;
            else amount += 4;
        }
        /*if (PlayerScript.Item[31] && amount >= 32 && CombatScript.Player.resistanceRing)
            CombatScript.Player.RingOfResistance();*/
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
        if (health <= 0 && !slain)
            Perish();
        UpdateInfo();
    }

    void Perish()
    {
        slain = true;
        if (PlayerScript.Item[39])
            CombatScript.Player.GainHealth(1);
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
        Display(amount, RestoreSprite);
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

    public void GainWeak(int amount)
    {
        effect[0] += amount;
        Display(amount, effectSprite[0]);
        if (PlayerScript.Item[13])
            TakeDamage(6 * amount);
        if (PlayerScript.Item[42])
            GainSlow(amount * 2);
        UpdateInfo();
    }

    public void GainBleed(int amount)
    {
        if (PlayerScript.Item[25] && amount >= 4)
            amount++;
        effect[1] += amount;
        Display(amount, effectSprite[1]);
        UpdateInfo();
    }

    public void GainDaze(int amount)
    {
        effect[2] += amount;
        Display(amount, effectSprite[2]);
        UpdateInfo();
    }

    public void GainStrength(int amount)
    {
        effect[3] += amount;
        Display(amount, effectSprite[3]);
        UpdateInfo();
    }

    public void GainVulnerable(int amount)
    {
        effect[9] += amount;
        Display(amount, effectSprite[9]);
        if (PlayerScript.Item[13])
            TakeDamage(6 * amount);
        if (PlayerScript.Item[42])
            GainSlow(amount * 2);
        UpdateInfo();
    }

    public void GainLink()
    {
        effect[15] += effect[14];
        while (effect[15] >= 10)
        {
            effect[15] -= 10;
            GainStrength(2);
        }
        UpdateInfo();
    }

    void Unstable(float amount)
    {
        unstableCharge += amount;
        tempi = 0;
        while (unstableCharge >= 1f + effect[21] * 0.25f)
        {
            tempi++;
            unstableCharge -= 1f;
        }
        if (tempi > 0)
        {
            Display(tempi, effectSprite[21]);
            effect[21] += tempi;
        }
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
        value *= (12 + level);
        value /= 12;
        return value;
    }

    int LevelCalculatedDef(int value)
    {
        value *= (10 + level);
        value /= 10;
        return value;
    }

    void UnleashMonster()
    {
        tempi = (maxHealth - health) * 9;
        tempi /= maxHealth;
        if (tempi > 0)
            GainBleed(LevelCalculated(tempi));
        health += maxHealth;
        maxHealth *= 2;
        health += LevelCalculated(30);
        maxHealth += LevelCalculated(30);
        slow = 0;
        tenacity = LevelCalculated(6);
        slow = LevelCalculated(tempi) / 4;
        if (!stunned)
            stunned = true;
        effect[3] += effect[3] / 2;
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

    void GainLevel(int levels)
    {
        int oldHealth = LevelCalculatedDef(Library.Enemies[unitID].UnitHealth);
        int oldTenacity = LevelCalculated(Library.Enemies[unitID].UnitTenacity);
        int oldEnormous = LevelCalculated(Library.Enemies[unitID].StartingEffects[8]);

        level += levels;
        Display(levels, LevelSprite);

        tempi2 = LevelCalculatedDef(Library.Enemies[unitID].UnitHealth) - oldHealth;
        maxHealth += tempi2;
        health += tempi2;
        tempi2 = LevelCalculated(Library.Enemies[unitID].UnitTenacity) - oldTenacity;
        tenacity += tempi2;
        tempi2 = LevelCalculated(Library.Enemies[unitID].StartingEffects[8]) - oldEnormous;
        effect[8] += tempi2;

        LevelValue.text = (level + 1).ToString("0");
        UpdateInfo();
    }

    // checks
    public int TotalBlock()
    {
        tempi = shield + block;
        return tempi;
    }
}
