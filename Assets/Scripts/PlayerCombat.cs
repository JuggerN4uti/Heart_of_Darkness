using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCombat : MonoBehaviour
{
    [Header("Scripts")]
    public Player PlayerScript;
    public Combat CombatScript;
    public Hand Cards;

    [Header("Stats")]
    public int maxHealth;
    public int health, shield, block, energy, mana, manaGain, cardDraw, sanity, maxSanity;
    public int[] effect;
    int tempi;
    float temp;

    [Header("Item Stats")]
    public int turns;
    public int attacks;

    [Header("Weapon")]
    public GameObject TheWeapon;
    public int baseDamage, strengthScaling, energyCost;
    public TMPro.TextMeshProUGUI TheWeaponCost, TheWeaponEffect;

    [Header("UI")]
    public GameObject ShieldDisplay;
    public GameObject BlockDisplay;
    public Image HealthBarFill, SanityBarFill, EnergyBarFill;
    public Button WeaponUseButton;
    public TMPro.TextMeshProUGUI HealthValue, ShieldValue, BlockValue, SanityValue, EnergyValue, ManaValue;
    public TMPro.TextMeshProUGUI[] CurseText;
    public GameObject[] UnitObject, CurseObject;
    public Image[] UnitSprite;

    [Header("Display")]
    public GameObject DisplayObject;
    public GameObject[] StatusObjects;
    public Image[] StatusImages;
    public TMPro.TextMeshProUGUI[] StatusValues;
    public Rigidbody2D Body;
    public Transform Origin;
    public Display Displayed;
    public Sprite DamageSprite, MagicDamageSprite, HealthSprite, ShieldSprite, BlockSprite, InsanitySprite, SanitySprite;
    public Sprite[] effectSprite;
    public int[] effectsActive;
    int statusCount;

    void Start()
    {
        //Cards.CardDraw.SetDeck();
        //StartTurn();
    }

    public void Reset()
    {
        Cards.CardsInHand = 0;
        Cards.CardDraw.SetDeck();

        manaGain = PlayerScript.BaseMana;
        cardDraw = PlayerScript.BaseDraw;
        mana = 0;
        energy = 0;
        attacks = 0;

        health = PlayerScript.Health;
        maxHealth = PlayerScript.MaxHealth;
        shield = PlayerScript.StatValues[5];
        sanity = PlayerScript.Sanity;
        maxSanity = PlayerScript.MaxSanity;

        baseDamage = PlayerScript.weaponDamage;
        strengthScaling = PlayerScript.weaponStrengthBonus;
        energyCost = PlayerScript.weaponEnergyRequirement;

        for (int i = 0; i < effect.Length; i++)
        {
            effect[i] = 0;
        }
        effect[6] = PlayerScript.StatValues[6];
        effect[0] = PlayerScript.StatValues[8];
        effect[1] = PlayerScript.StatValues[7];
        effect[2] = PlayerScript.StatValues[9];

        for (int i = 0; i < PlayerScript.CurseValue.Length; i++)
        {
            if (PlayerScript.CurseValue[i] > 0)
                GainCurseEffect(i, PlayerScript.CurseValue[i]);
        }


        StartTurn();
    }

    public void Set()
    {
        if (PlayerScript.Item[16])
            RestoreHealth(maxHealth / 50);
        PlayerScript.Health = health;
        PlayerScript.Sanity = sanity;
        PlayerScript.MaxSanity = maxSanity;
        PlayerScript.UpdateInfo();
    }

    public void StartTurn()
    {
        if (effect[11] > 0)
            effect[11]--;
        else
        {
            if (PlayerScript.Item[0])
            {
                if (block > 12)
                    block = 12;
            }
            else block = 0;
        }
        if (effect[3] > 0)
        {
            GainBlock(effect[3]);
            effect[3] = 0;
        }
        if (effect[6] > 0)
            GainBlock(effect[6]);
        GainEnergy(8 + effect[2]);
        if (effect[7] > 0)
            GainMana(manaGain - 1);
        else GainMana(manaGain);
        if (PlayerScript.Item[5] && CombatScript.turn < 3)
            GainMana(1);
        if (PlayerScript.Item[8])
        {
            turns++;
            if (turns % 3 == 0)
                GainMana(1);
        }
        if (effect[12] > 0)
            Cards.Draw(cardDraw - 1);
        else Cards.Draw(cardDraw);
        if (PlayerScript.CurseValue[2] > 0)
            CombatScript.EnemiesGainStrength(PlayerScript.CurseValue[2]);
        UpdateInfo();
    }

    void UpdateInfo()
    {
        HealthBarFill.fillAmount = (health * 1f) / (maxHealth * 1f);
        SanityBarFill.fillAmount = (sanity * 1f) / (maxSanity * 1f);
        EnergyBarFill.fillAmount = (energy * 1f) / (energyCost * 1f);
        HealthValue.text = health.ToString("") + "/" + maxHealth.ToString("");
        SanityValue.text = sanity.ToString("") + "/" + maxSanity.ToString("");
        EnergyValue.text = energy.ToString("") + "/" + energyCost.ToString("");
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

    public void Display(int amount, Sprite sprite)
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
        if (effect[7] > 0)
            effect[7]--;
        if (effect[8] > 0)
            TakeDamage(effect[8]);
        if (effect[9] > 0)
            effect[9]--;
        if (effect[10] > 0)
            effect[10]--;
        if (effect[12] > 0)
            effect[12]--;
        if (PlayerScript.CurseValue[1] > 0 && Cards.CardsInHand > 0)
            TakeDamage(PlayerScript.CurseValue[1] * Cards.CardsInHand * 4);
        if (PlayerScript.Item[6])
            TakeDamage(CombatScript.turn);
        if (PlayerScript.Item[16] && effect[8] > 0)
            effect[8]--;
        LoseSanity(TurnSanity());
        UpdateInfo();
        Cards.ShuffleHand();
    }

    public void LoseSanity(int amount)
    {
        if (amount > 0)
        {
            sanity -= amount;
            Display(amount, InsanitySprite);
        }
        PlayerScript.LoseSanity(amount, true);
        if (sanity < 1)
        {
            sanity += maxSanity;
            maxSanity += 8 + maxSanity / 25;
            GainCurse();
        }
        UpdateInfo();
    }

    public void GainSanity(int amount)
    {
        sanity += amount;
        Display(amount, SanitySprite);
        UpdateInfo();
    }

    void GainCurse()
    {
        tempi = Random.Range(0, PlayerScript.CurseValue.Length);
        PlayerScript.CurseValue[tempi]++;
        PlayerScript.CursesCount++;

        Origin.rotation = Quaternion.Euler(Origin.rotation.x, Origin.rotation.y, Body.rotation + Random.Range(-5f, 5f));
        GameObject display = Instantiate(DisplayObject, Origin.position, transform.rotation);
        Displayed = display.GetComponent(typeof(Display)) as Display;
        Displayed.DisplayName(PlayerScript.CurseName[tempi], InsanitySprite);
        Rigidbody2D display_body = display.GetComponent<Rigidbody2D>();
        display_body.AddForce(Origin.up * Random.Range(2f, 2.4f), ForceMode2D.Impulse);

        GainCurseEffect(tempi, 1);
    }

    void GainCurseEffect(int which, int level)
    {
        CurseObject[which].SetActive(true);
        if (PlayerScript.CurseValue[which] > 1)
            CurseText[which].text = PlayerScript.CurseValue[which].ToString("0");
        else CurseText[which].text = "";
        switch (which)
        {
            case 0:
                GainWeak(2 * level);
                break;
            case 2:
                CombatScript.EnemiesGainStrength(2 * level);
                break;
            case 4:
                GainFrail(2 * level);
                break;
        }
    }

    int TurnSanity()
    {
        temp = Random.Range(CombatScript.turn * 0.5f - 0.4f, CombatScript.turn * 0.75f - 0.3f);
        tempi = 0;
        for (float i = 1f; i < temp; i += 1f)
        {
            tempi++;
        }
        return tempi;
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

    public void GainEnergy(int amount)
    {
        energy += amount;
        UpdateInfo();
    }

    void SpendEnergy(int amount)
    {
        energy -= amount;
        UpdateInfo();
    }

    public void GainMana(int amount)
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

    void GainDexterity(int amount)
    {
        effect[2] += amount;
        Display(amount, effectSprite[2]);
        UpdateInfo();
    }

    void GainValor(int amount)
    {
        effect[4] += amount;
        Display(amount, effectSprite[4]);
        UpdateInfo();
    }

    public void GainSlow(int amount)
    {
        effect[7] += amount;
        Display(amount, effectSprite[7]);
        UpdateInfo();
    }

    public void GainBleed(int amount)
    {
        effect[8] += amount;
        Display(amount, effectSprite[8]);
        UpdateInfo();
    }

    public void GainWeak(int amount)
    {
        effect[9] += amount;
        Display(amount, effectSprite[9]);
        UpdateInfo();
    }

    public void GainFrail(int amount)
    {
        effect[10] += amount;
        Display(amount, effectSprite[10]);
        UpdateInfo();
    }

    public void GainTerror(int amount)
    {
        effect[12] += amount;
        Display(amount, effectSprite[12]);
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
        OnHit();
        SpendEnergy(energyCost);
        if (effect[5] > 0)
            Cards.Draw(effect[5]);
        UpdateInfo();
    }

    void OnHit()
    {
        attacks++;
        if (PlayerScript.Item[9] && attacks % 6 == 0)
            GainStrength(1);
        if (PlayerScript.Item[10] && attacks % 6 == 0)
            GainResistance(1);
        if (PlayerScript.Item[11] && attacks % 3 == 0)
            GainBlock(4);
        if (PlayerScript.Item[12] && attacks % 8 == 0)
            GainMana(1);
        if (PlayerScript.Item[17])
            GainEnergy(1);
    }

    int WeaponDamage()
    {
        tempi = baseDamage + strengthScaling * effect[0];
        return DamageDealtModifier(tempi);
    }

    public float HealthProcentage()
    {
        temp = (health * 1f) / (maxHealth * 1f);
        return temp;
    }

    public int DamageDealtModifier(int value)
    {
        if (effect[9] > 0)
        {
            value *= 3;
            value /= 4 + PlayerScript.CurseValue[0];
        }
        return value;
    }

    public int BlockGainedModifier(int value)
    {
        if (effect[10] > 0)
        {
            value *= 3;
            value /= 4 + PlayerScript.CurseValue[4];
        }
        return value;
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
        if (PlayerScript.CurseValue[3] > 0 && amount > 0)
            LoseSanity(Random.Range(PlayerScript.CurseValue[3] * 2, PlayerScript.CurseValue[3] * 3 + 1));
        if (health <= 0)
            Lost();
        UpdateInfo();
    }

    public void TakeMagicDamage(int amount)
    {
        Display(amount, MagicDamageSprite);
        health -= amount;
        if (health <= 0)
            Lost();
        UpdateInfo();
    }

    void Lost()
    {
        CombatScript.HeroesDefeated();
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
            case 28:
                ShieldOfHope(level);
                break;
            case 29:
                HammerOfWrath(level);
                break;
            case 30:
                ALightInTheDarkness(level);
                break;
            case 31:
                Barricade(level);
                break;
            case 32:
                RighteousHammer(level);
                break;
            case 33:
                LightsChosen(level);
                break;
            case 34:
                Penance(level);
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
                if (CombatScript.Enemy[CombatScript.targetedEnemy].IntentToAttack())
                    return "Deal " + JudgementDamage(level).ToString("") + " Damage, Apply " + JudgementVulnerable(level).ToString("") + " Vulnerable";
                else return "Deal " + JudgementDamage(level).ToString("") + " Damage";
            case 4:
                return "Deal " + BolaShotDamage(level).ToString("") + " Damage\nApply " + BolaShotSlow(level).ToString("") + " Slow";
            case 5:
                return "Deal " + CripplingStrikeDamage(level).ToString("") + " Damage\nApply " + CripplingStrikeBleed(level).ToString("") + " Bleed";
            case 6:
                return "Gain " + FortifyBlock(level).ToString("") + " Block\nGain " + FortifyBlock(level).ToString("") + " Block Next Trun";
            case 7:
                return "Gain " + EmpowerStrength(level).ToString("") + " Strength\n and " + EmpowerEnergy(level).ToString("") + " Energy";
            case 8:
                return "Draw " + InspireCardDraw(level).ToString("") + " Cards\nGain " + InspireBlock(level).ToString("") + " Block";
            case 9:
                if (level > 0)
                    return "Gain " + ShieldBashBlock(level).ToString("") + " Block\nDeal " + (ShieldBashBlock(level) + ShieldBashDamage(level)).ToString("") + " Damage";
                else return "Deal " + ShieldBashDamage(level).ToString("") + " Damage";
            case 10:
                if (HealthProcentage() < 0.5f)
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
                return "Gain " + HolyLightBlock(level).ToString("") + " Block\nRestore " + HolyLightHeal(level).ToString("") + " Health\nDestroy";
            case 20:
                return "Deal " + ChastiseDamage(level).ToString("") + " Damage\nApply " + ChastiseDaze(level).ToString("") + " Daze & " + ChastiseSlow(level).ToString("") + " Slow";
            case 21:
                return "Deal " + HolyBoltDamage(level).ToString("") + " Damage";
            case 22:
                return "Gain " + LayOnHandsValor(level).ToString("") + " Valor\nRestore " + LayOnHandsHeal(level).ToString("") + " Health\nDestroy";
            case 23:
                return "Deal " + CounterAttackDamage(level).ToString("") + " Damage";
            case 24:
                return "Gain " + SurgeOfLightDexterity(level).ToString("") + " Dexterity\n& " + SurgeOfLightEnergy(level).ToString("") + " Energy\nDestroy";
            case 25:
                return "Deal " + PatientStrikeDamage(level).ToString("") + " Damage";
            case 26:
                return "Deal " + CrushingBlowDamage(level).ToString("") + " Damage\nApply " + CrushingBlowDaze(level).ToString("") + " Daze\n" + CrushingBlowSlow(level).ToString("") + " Slow\n" + CrushingBlowWeak(level).ToString("") + " Weak\n& 1 Vulnerable";
            case 27:
                return "Gain " + HeavyArmorBlock(level).ToString("") + " Block";
            case 28:
                return "Gain " + ShieldOfHopeBlock(level).ToString("") + " Block\nDestroy";
            case 29:
                return "Increase your Weapon Damage by " + HammerOfWrathDamage(level).ToString("") + "\nEvery Weapon Attack Draws 1 Card\nDestroy";
            case 30:
                if (PlayerScript.CursesCount > 0)
                    return "Restore " + ALightInTheDarknessSanity(level).ToString("") + " Sanity\nGain " + ALightInTheDarknessBlock(level).ToString("") + " Block\nApply " + ALightInTheDarknessSlow(level).ToString("") + " Slow to all Enemies\nDestroy";
                else return "Restore " + ALightInTheDarknessSanity(level).ToString("") + " Sanity\nGain " + ALightInTheDarknessBlock(level).ToString("") + " Block\nDestroy";
            case 31:
                return "Gain " + BarricadeBlock(level).ToString("") + " Block\nBlock is retained for 1 Turn";
            case 32:
                return "Deal " + RighteousHammerDamage(level).ToString("") + " Damage\nApply " + RighteousHammerDamage(level).ToString("") + " Daze\n& " + RighteousHammerSlow(level).ToString("") + " Slow";
            case 33:
                return "Gain " + LightsChosenBuff(level).ToString("") + " Strength, Resistance & Dexterity";
            case 34:
                return "Deal " + PenanceDamage(level).ToString("") + " Damage\nGain " + PenanceBlock(level).ToString("") + " Block";
        }
        return "";
    }

    // Abilities ---
    void Strike(int level) // ID 0
    {
        CombatScript.Enemy[CombatScript.targetedEnemy].TakeDamage(StrikeDamage(level));
        OnHit();
    }

    int StrikeDamage(int level)
    {
        tempi = 9 + effect[0];
        tempi += 3 * level;
        return DamageDealtModifier(tempi);
    }

    void Defend(int level) // ID 1
    {
        GainBlock(DefendBlock(level));
    }

    int DefendBlock(int level)
    {
        tempi = 8 + effect[1];
        tempi += 3 * level;
        return BlockGainedModifier(tempi);
    }

    void SpearThrust(int level) // ID 2
    {
        CombatScript.Enemy[CombatScript.targetedEnemy].BreakShield(SpearThrustBreak(level));
        CombatScript.Enemy[CombatScript.targetedEnemy].TakeDamage(SpearThrustDamage(level));
        OnHit();
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
        return DamageDealtModifier(tempi);
    }

    void Judgement(int level) // ID 3
    {
        CombatScript.Enemy[CombatScript.targetedEnemy].TakeDamage(JudgementDamage(level));
        OnHit();
        if (CombatScript.Enemy[CombatScript.targetedEnemy].IntentToAttack())
            CombatScript.Enemy[CombatScript.targetedEnemy].GainVulnerable(JudgementVulnerable(level));
    }

    int JudgementDamage(int level)
    {
        tempi = 12 + effect[0];
        tempi += 4 * level;
        return DamageDealtModifier(tempi);
    }

    int JudgementVulnerable(int level)
    {
        tempi = 1;
        tempi += level;
        return tempi;
    }

    void BolaShot(int level) // ID 4
    {
        CombatScript.Enemy[CombatScript.targetedEnemy].TakeDamage(BolaShotDamage(level));
        OnHit();
        CombatScript.Enemy[CombatScript.targetedEnemy].GainSlow(BolaShotSlow(level));
    }

    int BolaShotDamage(int level)
    {
        tempi = 10 + effect[0];
        tempi += 3 * level;
        return DamageDealtModifier(tempi);
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
        OnHit();
        CombatScript.Enemy[CombatScript.targetedEnemy].GainBleed(CripplingStrikeBleed(level));
    }

    int CripplingStrikeDamage(int level)
    {
        tempi = 7 + effect[0];
        tempi += 2 * level;
        return DamageDealtModifier(tempi);
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
        return BlockGainedModifier(tempi);
    }

    void Empower(int level) // ID 7
    {
        GainStrength(EmpowerStrength(level));
        GainEnergy(EmpowerEnergy(level));
    }

    int EmpowerStrength(int level)
    {
        tempi = 2;
        tempi += level;
        return tempi;
    }

    int EmpowerEnergy(int level)
    {
        tempi = 2;
        tempi += 2 * level;
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
        tempi = 5 + effect[1];
        tempi += 2 * level;
        return BlockGainedModifier(tempi);
    }

    void ShieldBash(int level) // ID 9
    {
        if (level > 0)
            GainBlock(ShieldBashBlock(level));
        CombatScript.Enemy[CombatScript.targetedEnemy].TakeDamage(ShieldBashDamage(level));
        OnHit();
    }

    int ShieldBashBlock(int level)
    {
        tempi = effect[1];
        tempi += 3 * level;
        return BlockGainedModifier(tempi);
    }

    int ShieldBashDamage(int level)
    {
        tempi = block + effect[0];
        return DamageDealtModifier(tempi);
    }

    void DesperateStand(int level) // ID 10
    {
        GainBlock(DesperateStandBlock(level));
    }

    int DesperateStandBlock(int level)
    {
        tempi = 12 + effect[1];
        tempi += 4 * level;
        if (HealthProcentage() < 0.5f)
            tempi += 9 + 4 * level;
        return BlockGainedModifier(tempi);
    }

    void DecisiveStrike(int level) // ID 11
    {
        CombatScript.Enemy[CombatScript.targetedEnemy].TakeDamage(DecisiveStrikeDamage(level));
        OnHit();
        GainValor(DecisiveStrikeValor(level));
    }

    int DecisiveStrikeDamage(int level)
    {
        tempi = 12 + effect[0];
        tempi += 2 * level;
        tempi += effect[4];
        return DamageDealtModifier(tempi);
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
        return BlockGainedModifier(tempi);
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
        return BlockGainedModifier(tempi);
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
        return BlockGainedModifier(tempi);
    }

    int ShieldWallResistance(int level)
    {
        tempi = 1;
        return tempi;
    }

    void ShieldGlare(int level) // ID 15
    {
        GainBlock(ShieldGlareBlock(level));
        CombatScript.Enemy[CombatScript.targetedEnemy].GainWeak(ShieldGlareWeak(level));
    }

    int ShieldGlareBlock(int level)
    {
        tempi = 12 + effect[1];
        tempi += level;
        return BlockGainedModifier(tempi);
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
        OnHit();
        CombatScript.Enemy[CombatScript.targetedEnemy].GainSlow(SmiteSlow(level));
    }

    int SmiteDamage(int level)
    {
        tempi = 7 + effect[0];
        tempi += 2 * level;
        return DamageDealtModifier(tempi);
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
        OnHit();
        CombatScript.Enemy[CombatScript.targetedEnemy].GainDaze(BlindingLightDaze(level));
        CombatScript.Enemy[CombatScript.targetedEnemy].GainWeak(BlindingLightWeak(level));
    }

    int BlindingLightDamage(int level)
    {
        tempi = 19 + effect[0];
        tempi += 6 * level;
        return DamageDealtModifier(tempi);
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
        OnHit();
        GainValor(ConsecrationValor(level));
    }

    int ConsecrationDamage(int level)
    {
        tempi = 18 + effect[0];
        tempi += 6 * level;
        tempi += effect[4];
        return DamageDealtModifier(tempi);
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
        tempi = 7 + effect[1];
        tempi += 3 * level;
        return BlockGainedModifier(tempi);
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
        OnHit();
        CombatScript.Enemy[CombatScript.targetedEnemy].GainDaze(ChastiseDaze(level));
        CombatScript.Enemy[CombatScript.targetedEnemy].GainSlow(ChastiseSlow(level));
    }

    int ChastiseDamage(int level)
    {
        tempi = 10 + effect[0];
        tempi += 5 * level;
        tempi += CombatScript.Enemy[CombatScript.targetedEnemy].effect[2];
        return DamageDealtModifier(tempi);
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
        OnHit();
    }

    int HolyBoltDamage(int level)
    {
        tempi = 15 + effect[0];
        tempi += 5 * level;
        tempi += (3 + level) * effect[4];
        return DamageDealtModifier(tempi);
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
        tempi += 2 * level;
        return tempi;
    }

    void CounterAttack(int level) // ID 23
    {
        CombatScript.Enemy[CombatScript.targetedEnemy].TakeDamage(CounterAttackDamage(level));
        OnHit();
    }

    int CounterAttackDamage(int level)
    {
        tempi = 3 + effect[0];
        tempi += 5 * level;
        if (CombatScript.Enemy[CombatScript.targetedEnemy].IntentToAttack())
            tempi += CombatScript.Enemy[CombatScript.targetedEnemy].AttackDamage();
        return DamageDealtModifier(tempi);
    }

    void SurgeOfLight(int level) // ID 24
    {
        GainDexterity(SurgeOfLightDexterity(level));
        GainEnergy(SurgeOfLightEnergy(level));
    }

    int SurgeOfLightDexterity(int level)
    {
        tempi = 2;
        tempi += 1 * level;
        return tempi;
    }

    int SurgeOfLightEnergy(int level)
    {
        tempi = 4;
        tempi += 2 * level;
        return tempi;
    }

    void PatientStrike(int level) // ID 25
    {
        CombatScript.Enemy[CombatScript.targetedEnemy].TakeDamage(PatientStrikeDamage(level));
        OnHit();
    }

    int PatientStrikeDamage(int level)
    {
        tempi = 8 + effect[0];
        tempi += 2 * level;
        tempi += (2 + level) * CombatScript.turn;
        return DamageDealtModifier(tempi);
    }

    void CrushingBlow(int level) // ID 26
    {
        CombatScript.Enemy[CombatScript.targetedEnemy].TakeDamage(CrushingBlowDamage(level));
        OnHit();
        CombatScript.Enemy[CombatScript.targetedEnemy].GainDaze(CrushingBlowDaze(level));
        CombatScript.Enemy[CombatScript.targetedEnemy].GainSlow(CrushingBlowSlow(level));
        CombatScript.Enemy[CombatScript.targetedEnemy].GainWeak(CrushingBlowWeak(level));
        CombatScript.Enemy[CombatScript.targetedEnemy].GainVulnerable(1);
    }

    int CrushingBlowDamage(int level)
    {
        tempi = 22 + effect[0];
        tempi += 4 * level;
        return DamageDealtModifier(tempi);
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
        return BlockGainedModifier(tempi);
    }

    void ShieldOfHope(int level) // ID 28
    {
        GainBlock(ShieldOfHopeBlock(level));
    }

    int ShieldOfHopeBlock(int level)
    {
        tempi = (maxHealth - health) + effect[1];
        return BlockGainedModifier(tempi);
    }

    void HammerOfWrath(int level) // ID 29
    {
        baseDamage += HammerOfWrathDamage(level);
        effect[5]++;
        Display(1, effectSprite[5]);
    }

    int HammerOfWrathDamage(int level)
    {
        tempi = 3;
        tempi += 2 * level;
        return BlockGainedModifier(tempi);
    }

    void ALightInTheDarkness(int level) // ID 30
    {
        GainSanity(ALightInTheDarknessSanity(level));
        GainBlock(ALightInTheDarknessBlock(level));
        if (PlayerScript.CursesCount > 0)
        {
            for (int i = 0; i < CombatScript.enemyAlive.Length; i++)
            {
                if (CombatScript.enemyAlive[i])
                    CombatScript.Enemy[CombatScript.targetedEnemy].GainSlow(ALightInTheDarknessSlow(level));
            }
        }
    }

    int ALightInTheDarknessSanity(int level)
    {
        tempi = 8;
        tempi += 2 * level;
        return tempi;
    }

    int ALightInTheDarknessBlock(int level)
    {
        tempi = 4 + effect[1];
        tempi += 2 * level;
        return BlockGainedModifier(tempi);
    }

    int ALightInTheDarknessSlow(int level)
    {
        tempi = 1;
        tempi += level;
        tempi *= PlayerScript.CursesCount;
        return tempi;
    }

    void Barricade(int level) // ID 31
    {
        GainBlock(BarricadeBlock(level));
        effect[11]++;
        Display(1, effectSprite[11]);
    }

    int BarricadeBlock(int level)
    {
        tempi = 12 + effect[1];
        tempi += 5 * level;
        return BlockGainedModifier(tempi);
    }

    void RighteousHammer(int level) // ID 32
    {
        CombatScript.Enemy[CombatScript.targetedEnemy].TakeDamage(RighteousHammerDamage(level));
        OnHit();
        CombatScript.Enemy[CombatScript.targetedEnemy].GainDaze(RighteousHammerDamage(level));
        CombatScript.Enemy[CombatScript.targetedEnemy].GainSlow(RighteousHammerSlow(level));
    }

    int RighteousHammerDamage(int level)
    {
        tempi = 8 + effect[0];
        tempi += 2 * level;
        return DamageDealtModifier(tempi);
    }

    int RighteousHammerSlow(int level)
    {
        tempi = 1;
        tempi += level;
        return tempi;
    }

    void LightsChosen(int level) // ID 33
    {
        GainStrength(LightsChosenBuff(level));
        GainResistance(LightsChosenBuff(level));
        GainDexterity(LightsChosenBuff(level));
    }

    int LightsChosenBuff(int level)
    {
        tempi = 1;
        tempi += level;
        return tempi;
    }

    void Penance(int level) // ID 34
    {
        CombatScript.Enemy[CombatScript.targetedEnemy].TakeDamage(PenanceDamage(level));
        OnHit();
        GainBlock(PenanceBlock(level));
    }

    int PenanceDamage(int level)
    {
        tempi = 8 + effect[0];
        tempi += 3 * level;
        return DamageDealtModifier(tempi);
    }

    int PenanceBlock(int level)
    {
        tempi = 7 + effect[1];
        tempi += 2 * level;
        return BlockGainedModifier(tempi);
    }

    // checks
    public int TotalBlock()
    {
        tempi = shield + block;
        return tempi;
    }
}
