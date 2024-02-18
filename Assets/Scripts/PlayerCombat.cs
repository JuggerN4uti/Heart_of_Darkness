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
    public int[] effect;
    int tempi;
    float temp;

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

    [Header("Display")]
    public GameObject DisplayObject;
    public GameObject[] StatusObjects;
    public Image[] StatusImages;
    public TMPro.TextMeshProUGUI[] StatusValues;
    public Rigidbody2D Body;
    public Transform Origin;
    public Display Displayed;
    public Sprite DamageSprite, MagicDamageSprite, HealthSprite, ShieldSprite, BlockSprite;
    public Sprite[] effectSprite;
    public int[] effectsActive;
    int statusCount;

    void Start()
    {
        Cards.CardDraw.SetDeck();
        StartTurn();
    }

    public void StartTurn()
    {
        block = 0;
        if (effect[3] > 0)
        {
            GainBlock(effect[3]);
            effect[3] = 0;
        }
        GainEnergy(10 + effect[2]);
        GainMana(3);
        Cards.Draw(5);
        if (effect[5] > 0)
            GainValor(effect[5]);
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
        Display(amount, BlockSprite);
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

    void GainStrength(int amount)
    {
        effect[0] += amount;
        Display(amount, effectSprite[0]);
        UpdateInfo();
    }

    void GainResistance(int amount)
    {
        effect[1] += amount;
        Display(amount, effectSprite[1]);
        UpdateInfo();
    }

    void GainValor(int amount)
    {
        effect[4] += amount;
        Display(amount, effectSprite[4]);
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
        tempi = baseDamage + strengthScaling * effect[0];
        return tempi;
    }

    public float HealthProcentage()
    {
        temp = (health * 1f) / (maxHealth * 1f);
        return temp;
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
            case 6:
                Fortify(level);
                break;
            case 7:
                Empower(level);
                break;
            case 8:
                Inspire(level);
                break;
            case 9:
                ShieldBash(level);
                break;
            case 10:
                DesperateStand(level);
                break;
            case 11:
                DecisiveStrike(level);
                break;
            case 12:
                BulwarkOfLight(level);
                break;
            case 13:
                GoldenAegis(level);
                break;
            case 14:
                ShieldWall(level);
                break;
            case 15:
                ShieldGlare(level);
                break;
            case 16:
                Smite(level);
                break;
            case 17:
                BlindingLight(level);
                break;
            case 18:
                Consecration(level);
                break;
            case 19:
                HolyLight(level);
                break;
            case 20:
                Chastise(level);
                break;
            case 21:
                HolyBolt(level);
                break;
            case 22:
                LayOnHands(level);
                break;
            case 23:
                CounterAttack(level);
                break;
            case 24:
                SurgeOfLight(level);
                break;
            case 25:
                PatientStrike(level);
                break;
            case 26:
                CrushingBlow(level);
                break;
            case 27:
                HeavyArmor(level);
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
                return "Break up to " + SpearThrustBreak(level).ToString("") + " Shield\nDeal " + SpearThrustDamage(level).ToString("") + " Damage";
            case 3:
                return "Deal " + JudgementDamage(level).ToString("") + " Damage";
            case 4:
                return "Deal " + BolaShotDamage(level).ToString("") + " Damage\nApply " + BolaShotSlow(level).ToString("") + " Slow";
            case 5:
                return "Deal " + CripplingStrikeDamage(level).ToString("") + " Damage\nApply " + CripplingStrikeBleed(level).ToString("") + " Bleed";
            case 6:
                return "Gain " + FortifyBlock(level).ToString("") + " Block\nGain " + FortifyBlock(level).ToString("") + " Block Next Trun";
            case 7:
                return "Gain " + EmpowerStrength(level).ToString("") + " Strength";
            case 8:
                return "Draw " + InspireCardDraw(level).ToString("") + " Cards\nGain " + InspireBlock(level).ToString("") + " Block";
            case 9:
                if (level > 0)
                    return "Gain " + ShieldBashBlock(level).ToString("") + " Block\nDeal " + (ShieldBashBlock(level) + ShieldBashDamage(level)).ToString("") + " Damage";
                else return "Deal " + ShieldBashDamage(level).ToString("") + " Damage";
            case 10:
                if (HealthProcentage() < 0.6f)
                    return "Gain " + DesperateStandBlock(level).ToString("") + " Block\nDestroy";
                else return "Gain " + DesperateStandBlock(level).ToString("") + " Block";
            case 11:
                return "Deal " + DecisiveStrikeDamage(level).ToString("") + " Damage\nGain " + DecisiveStrikeValor(level).ToString("") + " Valor";
            case 12:
                return "Gain " + BulwarkOfLightBlock(level).ToString("") + " Block\nGain " + BulwarkOfLightValor(level).ToString("") + " Valor";
            case 13:
                return "Gain " + GoldenAegisBlock(level).ToString("") + " Block\nApply " + GoldenAegisSlow(level).ToString("") + " Slow\nto All Enemies";
            case 14:
                return "Gain " + ShieldWallBlock(level).ToString("") + " Block\nGain " + ShieldWallResistance(level).ToString("") + " Resistance";
            case 15:
                return "Gain " + ShieldGlareBlock(level).ToString("") + " Block\nApply " + ShieldGlareWeak(level).ToString("") + " Weak";
            case 16:
                return "Deal " + SmiteDamage(level).ToString("") + " Damage\nApply " + SmiteSlow(level).ToString("") + " Slow";
            case 17:
                return "Deal " + BlindingLightDamage(level).ToString("") + " Damage\nApply " + BlindingLightDaze(level).ToString("") + " Daze & " + BlindingLightWeak(level).ToString("") + " Weak";
            case 18:
                return "Deal " + ConsecrationDamage(level).ToString("") + " Damage\nApply " + ConsecrationSlow(level).ToString("") + " Slow\nto All Enemies\n Gain " + ConsecrationValor(level).ToString("") + " Valor";
            case 19:
                return "Gain " + HolyLightBlock(level).ToString("") + " Block\nRestore " + HolyLightHeal(level).ToString("") + " Health";
            case 20:
                return "Deal " + ChastiseDamage(level).ToString("") + " Damage\nApply " + ChastiseDaze(level).ToString("") + " Daze & " + ChastiseSlow(level).ToString("") + " Slow";
            case 21:
                return "Deal " + HolyBoltDamage(level).ToString("") + " Damage";
            case 22:
                return "Gain " + LayOnHandsValor(level).ToString("") + " Valor\nRestore " + LayOnHandsHeal(level).ToString("") + " Health";
            case 23:
                return "Deal " + CounterAttackDamage(level).ToString("") + " Damage";
            case 24:
                return "Gain " + SurgeOfLightValor(level).ToString("") + " Valor\nGain 1 Valor at the Start of Each Turn";
            case 25:
                return "Deal " + PatientStrikeDamage(level).ToString("") + " Damage";
            case 26:
                return "Deal " + CrushingBlowDamage(level).ToString("") + " Damage\nApply " + CrushingBlowDaze(level).ToString("") + " Daze\n" + CrushingBlowSlow(level).ToString("") + " Slow\n" + CrushingBlowWeak(level).ToString("") + " Weak\n& 1 Vulnerable";
            case 27:
                return "Gain " + HeavyArmorBlock(level).ToString("") + " Block";
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
        tempi = 9 + effect[0];
        tempi += 3 * level;
        return tempi;
    }

    void Defend(int level) // ID 1
    {
        GainBlock(DefendBlock(level));
    }

    int DefendBlock(int level)
    {
        tempi = 8 + effect[1];
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
        tempi = 11 + effect[0];
        tempi += 3 * level;
        return tempi;
    }

    void Judgement(int level) // ID 3
    {
        CombatScript.Enemy[CombatScript.targetedEnemy].TakeDamage(JudgementDamage(level));
    }

    int JudgementDamage(int level)
    {
        tempi = 12 + effect[0];
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
        tempi = 10 + effect[0];
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
        tempi = 7 + effect[0];
        tempi += 2 * level;
        return tempi;
    }

    int CripplingStrikeBleed(int level)
    {
        tempi = 4;
        tempi += 2 * level;
        return tempi;
    }

    void Fortify(int level) // ID 6
    {
        effect[3] += FortifyBlock(level);
        GainBlock(FortifyBlock(level));
    }

    int FortifyBlock(int level)
    {
        tempi = 8 + effect[1];
        tempi += 3 * level;
        return tempi;
    }

    void Empower(int level) // ID 7
    {
        GainStrength(EmpowerStrength(level));
    }

    int EmpowerStrength(int level)
    {
        tempi = 2;
        tempi += level;
        return tempi;
    }

    void Inspire(int level) // ID 8
    {
        Cards.Draw(InspireCardDraw(level));
        GainBlock(InspireBlock(level));
    }

    int InspireCardDraw(int level)
    {
        tempi = 2;
        tempi += level;
        return tempi;
    }

    int InspireBlock(int level)
    {
        tempi = 4 + effect[1];
        tempi += 2 * level;
        return tempi;
    }

    void ShieldBash(int level) // ID 9
    {
        if (level > 0)
            GainBlock(ShieldBashBlock(level));
        CombatScript.Enemy[CombatScript.targetedEnemy].TakeDamage(ShieldBashDamage(level));
    }

    int ShieldBashBlock(int level)
    {
        tempi = effect[1];
        tempi += 3 * level;
        return tempi;
    }

    int ShieldBashDamage(int level)
    {
        tempi = block + effect[0];
        return tempi;
    }

    void DesperateStand(int level) // ID 10
    {
        GainBlock(DesperateStandBlock(level));
        //if (HealthProcentage() < 0.6f)
        //    Cards.DestroyCard();
    }

    int DesperateStandBlock(int level)
    {
        tempi = 12 + effect[1];
        tempi += 4 * level;
        if (HealthProcentage() < 0.6f)
            tempi += 8 + 4 * level;
        return tempi;
    }

    void DecisiveStrike(int level) // ID 11
    {
        CombatScript.Enemy[CombatScript.targetedEnemy].TakeDamage(DecisiveStrikeDamage(level));
        GainValor(DecisiveStrikeValor(level));
    }

    int DecisiveStrikeDamage(int level)
    {
        tempi = 12 + effect[0];
        tempi += 2 * level;
        tempi += effect[4];
        return tempi;
    }

    int DecisiveStrikeValor(int level)
    {
        tempi = 1;
        tempi += level;
        return tempi;
    }

    void BulwarkOfLight(int level) // ID 12
    {
        GainBlock(BulwarkOfLightBlock(level));
        GainValor(BulwarkOfLightValor(level));
    }

    int BulwarkOfLightBlock(int level)
    {
        tempi = 10 + effect[1];
        tempi += 2 * level;
        tempi += effect[4];
        return tempi;
    }

    int BulwarkOfLightValor(int level)
    {
        tempi = 1;
        tempi += level;
        return tempi;
    }

    void GoldenAegis(int level) // ID 13
    {
        GainBlock(GoldenAegisBlock(level));
        for (int i = 0; i < CombatScript.enemyAlive.Length; i++)
        {
            if (CombatScript.enemyAlive[i])
                CombatScript.Enemy[CombatScript.targetedEnemy].GainSlow(GoldenAegisSlow(level));
        }
    }

    int GoldenAegisBlock(int level)
    {
        tempi = 12 + effect[1];
        tempi += 3 * level;
        return tempi;
    }

    int GoldenAegisSlow(int level)
    {
        tempi = 1;
        tempi += level;
        return tempi;
    }

    void ShieldWall(int level) // ID 14
    {
        GainBlock(ShieldWallBlock(level));
        GainResistance(ShieldWallResistance(level));
    }

    int ShieldWallBlock(int level)
    {
        tempi = 10 + effect[1];
        tempi += 4 * level;
        return tempi;
    }

    int ShieldWallResistance(int level)
    {
        tempi = 1;
        return tempi;
    }

    void ShieldGlare(int level) // ID 15
    {
        GainBlock(ShieldWallBlock(level));
        CombatScript.Enemy[CombatScript.targetedEnemy].GainWeak(ShieldGlareWeak(level));
    }

    int ShieldGlareBlock(int level)
    {
        tempi = 12 + effect[1];
        tempi += level;
        return tempi;
    }

    int ShieldGlareWeak(int level)
    {
        tempi = 1;
        tempi += level;
        return tempi;
    }

    void Smite(int level) // ID 16
    {
        CombatScript.Enemy[CombatScript.targetedEnemy].TakeDamage(SmiteDamage(level));
        CombatScript.Enemy[CombatScript.targetedEnemy].GainSlow(SmiteSlow(level));
    }

    int SmiteDamage(int level)
    {
        tempi = 7 + effect[0];
        tempi += 2 * level;
        return tempi;
    }

    int SmiteSlow(int level)
    {
        tempi = 1;
        tempi += level;
        return tempi;
    }

    void BlindingLight(int level) // ID 17
    {
        CombatScript.Enemy[CombatScript.targetedEnemy].TakeDamage(BlindingLightDamage(level));
        CombatScript.Enemy[CombatScript.targetedEnemy].GainDaze(BlindingLightDaze(level));
        CombatScript.Enemy[CombatScript.targetedEnemy].GainWeak(BlindingLightWeak(level));
    }

    int BlindingLightDamage(int level)
    {
        tempi = 19 + effect[0];
        tempi += 6 * level;
        return tempi;
    }

    int BlindingLightWeak(int level)
    {
        tempi = 2;
        tempi += level;
        return tempi;
    }

    int BlindingLightDaze(int level)
    {
        tempi = 3;
        tempi += 2 * level;
        return tempi;
    }

    void Consecration(int level) // ID 18
    {
        for (int i = 0; i < CombatScript.enemyAlive.Length; i++)
        {
            if (CombatScript.enemyAlive[i])
            {
                CombatScript.Enemy[CombatScript.targetedEnemy].TakeDamage(ConsecrationDamage(level));
                CombatScript.Enemy[CombatScript.targetedEnemy].GainSlow(ConsecrationSlow(level));
            }
        }
        GainValor(ConsecrationValor(level));
    }

    int ConsecrationDamage(int level)
    {
        tempi = 18 + effect[0];
        tempi += 6 * level;
        return tempi;
    }

    int ConsecrationSlow(int level)
    {
        tempi = 1;
        tempi += level;
        return tempi;
    }

    int ConsecrationValor(int level)
    {
        tempi = 2;
        tempi += level;
        return tempi;
    }

    void HolyLight(int level) // ID 19
    {
        GainBlock(HolyLightBlock(level));
        RestoreHealth(HolyLightHeal(level));
    }

    int HolyLightBlock(int level)
    {
        tempi = 6 + effect[1];
        tempi += 3 * level;
        return tempi;
    }

    int HolyLightHeal(int level)
    {
        tempi = 5;
        tempi += 2 * level;
        return tempi;
    }

    void Chastise(int level) // ID 20
    {
        CombatScript.Enemy[CombatScript.targetedEnemy].TakeDamage(ChastiseDamage(level));
        CombatScript.Enemy[CombatScript.targetedEnemy].GainDaze(ChastiseDaze(level));
        CombatScript.Enemy[CombatScript.targetedEnemy].GainSlow(ChastiseSlow(level));
    }

    int ChastiseDamage(int level)
    {
        tempi = 10 + effect[0];
        tempi += 5 * level;
        tempi += CombatScript.Enemy[CombatScript.targetedEnemy].effect[2];
        return tempi;
    }

    int ChastiseSlow(int level)
    {
        tempi = 4;
        tempi += level;
        return tempi;
    }

    int ChastiseDaze(int level)
    {
        tempi = 6;
        tempi += 2 * level;
        return tempi;
    }

    void HolyBolt(int level) // ID 21
    {
        CombatScript.Enemy[CombatScript.targetedEnemy].TakeDamage(HolyBoltDamage(level));
    }

    int HolyBoltDamage(int level)
    {
        tempi = 15 + effect[0];
        tempi += 5 * level;
        tempi += (3 + level) * effect[4];
        return tempi;
    }

    void LayOnHands(int level) // ID 22
    {
        GainValor(LayOnHandsValor(level));
        RestoreHealth(LayOnHandsHeal(level));
    }

    int LayOnHandsValor(int level)
    {
        tempi = 2;
        tempi += level;
        return tempi;
    }

    int LayOnHandsHeal(int level)
    {
        tempi = 3;
        tempi += level;
        return tempi;
    }

    void CounterAttack(int level) // ID 23
    {
        CombatScript.Enemy[CombatScript.targetedEnemy].TakeDamage(CounterAttackDamage(level));
    }

    int CounterAttackDamage(int level)
    {
        tempi = 3 + effect[0];
        tempi += 5 * level;
        if (CombatScript.Enemy[CombatScript.targetedEnemy].IntentToAttack())
            tempi += CombatScript.Enemy[CombatScript.targetedEnemy].AttackDamage();
        return tempi;
    }

    void SurgeOfLight(int level) // ID 24
    {
        GainValor(SurgeOfLightValor(level));
        effect[5]++;
        Display(1, effectSprite[5]);
    }

    int SurgeOfLightValor(int level)
    {
        tempi = 1;
        tempi += 2 * level;
        return tempi;
    }

    void PatientStrike(int level) // ID 25
    {
        CombatScript.Enemy[CombatScript.targetedEnemy].TakeDamage(PatientStrikeDamage(level));
    }

    int PatientStrikeDamage(int level)
    {
        tempi = 10 + effect[0];
        tempi += 3 * level;
        tempi += (2 + level) * CombatScript.turn;
        return tempi;
    }

    void CrushingBlow(int level) // ID 26
    {
        CombatScript.Enemy[CombatScript.targetedEnemy].TakeDamage(CrushingBlowDamage(level));
        CombatScript.Enemy[CombatScript.targetedEnemy].GainDaze(CrushingBlowDaze(level));
        CombatScript.Enemy[CombatScript.targetedEnemy].GainSlow(CrushingBlowSlow(level));
        CombatScript.Enemy[CombatScript.targetedEnemy].GainWeak(CrushingBlowWeak(level));
        //CombatScript.Enemy[CombatScript.targetedEnemy].GainVulnerable(1);
    }

    int CrushingBlowDamage(int level)
    {
        tempi = 22 + effect[0];
        tempi += 4 * level;
        return tempi;
    }

    int CrushingBlowSlow(int level)
    {
        tempi = 1;
        tempi += level;
        return tempi;
    }

    int CrushingBlowWeak(int level)
    {
        tempi = 1;
        tempi += level;
        return tempi;
    }

    int CrushingBlowDaze(int level)
    {
        tempi = 2;
        tempi += 2 * level;
        return tempi;
    }

    void HeavyArmor(int level) // ID 27
    {
        GainBlock(HeavyArmorBlock(level));
    }

    int HeavyArmorBlock(int level)
    {
        tempi = 11 + 3 * effect[1];
        tempi += (3 + effect[1]) * level;
        return tempi;
    }
}