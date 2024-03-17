using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCombat : MonoBehaviour
{
    [Header("Scripts")]
    public Player PlayerScript;
    public Combat CombatScript;
    public CardLibrary Library;
    public Hand Cards;
    public ItemsCollected ItemsScript;

    [Header("Stats")]
    public int maxHealth;
    public int health, shield, block, energy, mana, manaGain, cardDraw, sanity, maxSanity;
    public int[] effect;
    int tempi, tempi2;
    float temp;

    [Header("Ability Stats")]
    int combo, flurry, lightingDamage;

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
        combo = 0;
        flurry = 0;
        attacks = 0;
        lightingDamage = 30;

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

        ItemsScript.SetText();
        StartTurn();
    }

    public void Set()
    {
        if (PlayerScript.Item[16])
            RestoreHealth((maxHealth - health) / 20);
        PlayerScript.MaxHealth = maxHealth;
        PlayerScript.Health = health;
        PlayerScript.MaxSanity = maxSanity;
        PlayerScript.Sanity = sanity;
        PlayerScript.UpdateInfo();
    }

    public void StartTurn()
    {
        combo = 0;
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
            GainBlock(effect[6] + effect[1] * effect[13]);
        if (PlayerScript.Item[27] && shield > 0)
            GainBlock(5);
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
        if (PlayerScript.CurseValue[2] > 0 && CombatScript.turn % 2 == 0)
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

        ItemsScript.SetText();
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
            TakeDamage(CombatScript.turn * 2);
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
                CombatScript.EnemiesGainStrength(3 * level);
                break;
            case 4:
                GainFrail(2 * level);
                break;
        }
    }

    int TurnSanity()
    {
        temp = Random.Range(CombatScript.turn * 0.3f - 0.4f, CombatScript.turn * 0.55f - 0.35f);
        tempi = 0;
        for (float i = 1f; i < temp; i += 1f)
        {
            tempi++;
        }
        return tempi;
    }

    void GainHealth(int amount)
    {
        maxHealth += amount;
        health += amount;
        Display(amount, HealthSprite);
        UpdateInfo();
    }

    public void GainBlock(int amount)
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
        Cards.UpdateInfo();
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

    void GainArmor(int amount)
    {
        effect[6] += amount;
        Display(amount, effectSprite[6]);
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

    public void GainVulnerable(int amount)
    {
        effect[16] += amount;
        Display(amount, effectSprite[16]);
        UpdateInfo();
    }

    public void GainStormCharge(int amount)
    {
        effect[18] += amount;
        while (effect[18] >= 7)
        {
            effect[18] -= 7;
            if (CombatScript.enemiesAlive > 0)
            {
                tempi = CombatScript.RandomEnemy();
                CombatScript.Enemy[tempi].TakeDamage(lightingDamage);
                CombatScript.Enemy[tempi].GainSlow(3);
                lightingDamage += 5;
            }
        }
        //Display(amount, effectSprite[18]);
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
        {
            CombatScript.Enemy[CombatScript.targetedEnemy].GainDaze(effect[5]);
            CombatScript.Enemy[CombatScript.targetedEnemy].GainSlow(effect[5]);
        }
        if (effect[17] > 0)
            GainStormCharge(effect[17]);
        UpdateInfo();
    }

    void OnHit()
    {
        attacks++;
        if (effect[15] > 0)
            CombatScript.Enemy[CombatScript.targetedEnemy].GainBleed(effect[15]);
        if (PlayerScript.Item[9] && attacks % 7 == 0)
            GainStrength(1);
        if (PlayerScript.Item[10] && attacks % 7 == 0)
            GainResistance(1);
        if (PlayerScript.Item[11] && attacks % 4 == 0)
            GainBlock(5);
        if (PlayerScript.Item[12] && attacks % 9 == 0)
            GainMana(1);
        if (PlayerScript.Item[17] && attacks % 3 == 0)
            GainEnergy(2);
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
            if (PlayerScript.Item[28])
            {
                value *= 4;
                value /= 5 + PlayerScript.CurseValue[0];
            }
            else
            {
                value *= 3;
                value /= 4 + PlayerScript.CurseValue[0];
            }
        }
        return value;
    }

    public int BlockGainedModifier(int value)
    {
        if (effect[10] > 0)
        {
            if (PlayerScript.Item[29])
            {
                value *= 4;
                value /= 5 + PlayerScript.CurseValue[4];
            }
            else
            {
                value *= 3;
                value /= 4 + PlayerScript.CurseValue[4];
            }
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
        CombatScript.CardPlayed();
        if (effect[14] > 0)
            GainBlock(effect[14]);
        if (which < Library.neutralCards)
            UseNeutralAbility(which, level);
        else
        {
            which -= Library.neutralCards;
            if (which < Library.lightCards)
                UseLightAbility(which, level);
            else
            {
                which -= Library.lightCards;
                UseWaterAbility(which, level);
            }
            // potem dalej etc.
        }
        combo++;
    }

    public void UseNeutralAbility(int which, int level)
    {
        if (which == 0)
            Strike(level);
        else Defend(level);
    }

    public void UseLightAbility(int which, int level)
    {
        switch (which)
        {
            case 0:
                SpearThrust(level);
                break;
            case 1:
                Judgement(level);
                break;
            case 2:
                BolaShot(level);
                break;
            case 3:
                CripplingStrike(level);
                break;
            case 4:
                Fortify(level);
                break;
            case 5:
                Empower(level);
                break;
            case 6:
                Inspire(level);
                break;
            case 7:
                ShieldBash(level);
                break;
            case 8:
                DesperateStand(level);
                break;
            case 9:
                DecisiveStrike(level);
                break;
            case 10:
                BulwarkOfLight(level);
                break;
            case 11:
                GoldenAegis(level);
                break;
            case 12:
                ShieldWall(level);
                break;
            case 13:
                ShieldGlare(level);
                break;
            case 14:
                Smite(level);
                break;
            case 15:
                BlindingLight(level);
                break;
            case 16:
                Consecration(level);
                break;
            case 17:
                HolyLight(level);
                break;
            case 18:
                Chastise(level);
                break;
            case 19:
                HolyBolt(level);
                break;
            case 20:
                LayOnHands(level);
                break;
            case 21:
                CounterAttack(level);
                break;
            case 22:
                SurgeOfLight(level);
                break;
            case 23:
                PatientStrike(level);
                break;
            case 24:
                CrushingBlow(level);
                break;
            case 25:
                HeavyArmor(level);
                break;
            case 26:
                ShieldOfHope(level);
                break;
            case 27:
                HammerOfWrath(level);
                break;
            case 28:
                ALightInTheDarkness(level);
                break;
            case 29:
                Barricade(level);
                break;
            case 30:
                RighteousHammer(level);
                break;
            case 31:
                LightsChosen(level);
                break;
            case 32:
                Penance(level);
                break;
            case 33:
                GuardianAngel(level);
                break;
            case 34:
                Vengeance(level);
                break;
        }
    }

    public void UseWaterAbility(int which, int level)
    {
        switch (which)
        {
            case 0:
                QuickCut(level);
                break;
            case 1:
                HarpoonThrow(level);
                break;
            case 2:
                CutDown(level);
                break;
            case 3:
                Ensnare(level);
                break;
            case 4:
                ViciousSlash(level);
                break;
            case 5:
                Rupture(level);
                break;
            case 6:
                ProtectiveBubble(level);
                break;
            case 7:
                Swift(level);
                break;
            case 8:
                Hop(level);
                break;
            case 9:
                Impale(level);
                break;
            case 10:
                Flurry(level);
                break;
            case 11:
                SerratedBlade(level);
                break;
            case 12:
                ShellsUp(level);
                break;
            case 13:
                DeadlySwings(level);
                break;
            case 14:
                SteelOfSteal(level);
                break;
            case 15:
                Eviscerate(level);
                break;
            case 16:
                DoubleJump(level);
                break;
            case 17:
                FlowLikeWater(level);
                break;
            case 18:
                Preparation(level);
                break;
            case 19:
                StaggeringBlow(level);
                break;
            case 20:
                DredgeLine(level);
                break;
            case 21:
                Shred(level);
                break;
            case 22:
                StrengthOfTheDepths(level);
                break;
            case 23:
                TridentOfStorms(level);
                break;
            case 24:
                Conduit(level);
                break;
            case 25:
                Anchored(level);
                break;
            case 26:
                Nimble(level);
                break;
            case 27:
                Sink(level);
                break;
            case 28:
                Rend(level);
                break;
            case 29:
                ThickSkin(level);
                break;
        }
    }

    public string AbilityInfo(int which, int level)
    {
        if (which < Library.neutralCards)
        {
            switch (which)
            {
                case 0:
                    return "Deal " + StrikeDamage(level).ToString("") + " Damage";
                case 1:
                    return "Gain " + DefendBlock(level).ToString("") + " Block";
            }
        }
        else
        {
            which -= Library.neutralCards;
            if (which < Library.lightCards)
            {
                switch (which)
                {
                    case 0:
                        return "Break up to " + SpearThrustBreak(level).ToString("") + " Shield\nDeal " + SpearThrustDamage(level).ToString("") + " Damage";
                    case 1:
                        if (CombatScript.Enemy[CombatScript.targetedEnemy].IntentToAttack())
                            return "Deal " + JudgementDamage(level).ToString("") + " Damage, Apply " + JudgementVulnerable(level).ToString("") + " Vulnerable";
                        else return "Deal " + JudgementDamage(level).ToString("") + " Damage";
                    case 2:
                        return "Deal " + BolaShotDamage(level).ToString("") + " Damage\nApply " + BolaShotSlow(level).ToString("") + " Slow";
                    case 3:
                        return "Deal " + CripplingStrikeDamage(level).ToString("") + " Damage\nApply " + CripplingStrikeBleed(level).ToString("") + " Bleed";
                    case 4:
                        return "Gain " + FortifyBlock(level).ToString("") + " Block\nGain " + FortifyBlock(level).ToString("") + " Block Next Trun";
                    case 5:
                        return "Gain " + EmpowerBuff(level).ToString("") + " Strength & Energy";
                    case 6:
                        return "Draw " + InspireCardDraw(level).ToString("") + " Cards\nGain " + InspireBlock(level).ToString("") + " Block";
                    case 7:
                        if (level > 0)
                            return "Gain " + ShieldBashBlock(level).ToString("") + " Block\nDeal " + (ShieldBashBlock(level) + ShieldBashDamage(level)).ToString("") + " Damage";
                        else return "Deal " + ShieldBashDamage(level).ToString("") + " Damage";
                    case 8:
                        if (HealthProcentage() < 0.5f)
                            return "Gain " + DesperateStandBlock(level).ToString("") + " Block\nDestroy";
                        else return "Gain " + DesperateStandBlock(level).ToString("") + " Block";
                    case 9:
                        return "Deal " + DecisiveStrikeDamage(level).ToString("") + " Damage\nGain " + DecisiveStrikeValor(level).ToString("") + " Valor";
                    case 10:
                        return "Gain " + BulwarkOfLightBlock(level).ToString("") + " Block\nGain " + BulwarkOfLightValor(level).ToString("") + " Valor";
                    case 11:
                        return "Gain " + GoldenAegisBlock(level).ToString("") + " Block\nApply " + GoldenAegisSlow(level).ToString("") + " Slow\nto All Enemies";
                    case 12:
                        return "Gain " + ShieldWallBlock(level).ToString("") + " Block\nGain " + ShieldWallResistance(level).ToString("") + " Resistance";
                    case 13:
                        return "Gain " + ShieldGlareBlock(level).ToString("") + " Block\nApply " + ShieldGlareWeak(level).ToString("") + " Weak";
                    case 14:
                        return "Deal " + SmiteDamage(level).ToString("") + " Damage\nApply " + SmiteSlow(level).ToString("") + " Slow";
                    case 15:
                        return "Deal " + BlindingLightDamage(level).ToString("") + " Damage\nApply " + BlindingLightDaze(level).ToString("") + " Daze & " + BlindingLightWeak(level).ToString("") + " Weak";
                    case 16:
                        return "Deal " + ConsecrationDamage(level).ToString("") + " Damage\nApply " + ConsecrationSlow(level).ToString("") + " Slow\nto All Enemies\n Gain " + ConsecrationValor(level).ToString("") + " Valor";
                    case 17:
                        return "Gain " + HolyLightBlock(level).ToString("") + " Block\nRestore " + HolyLightHeal(level).ToString("") + " Health\nDestroy";
                    case 18:
                        return "Deal " + ChastiseDamage(level).ToString("") + " Damage\nApply " + ChastiseDaze(level).ToString("") + " Daze & " + ChastiseSlow(level).ToString("") + " Slow";
                    case 19:
                        return "Deal " + HolyBoltDamage(level).ToString("") + " Damage";
                    case 20:
                        return "Gain " + LayOnHandsValor(level).ToString("") + " Valor\nRestore " + LayOnHandsHeal(level).ToString("") + " Health\nDestroy";
                    case 21:
                        return "Deal " + CounterAttackDamage(level).ToString("") + " Damage";
                    case 22:
                        return "Gain " + SurgeOfLightDexterity(level).ToString("") + " Dexterity\n& " + SurgeOfLightEnergy(level).ToString("") + " Energy\nDestroy";
                    case 23:
                        return "Deal " + PatientStrikeDamage(level).ToString("") + " Damage";
                    case 24:
                        return "Deal " + CrushingBlowDamage(level).ToString("") + " Damage\nApply " + CrushingBlowSlow(level).ToString("")
                            + " Slow\n" + CrushingBlowWeak(level).ToString("") + " Weak\n& " + CrushingBlowVulnerable(level).ToString("") + " Vulnerable";
                    case 25:
                        return "Gain " + HeavyArmorBlock(level).ToString("") + " Block";
                    case 26:
                        return "Gain " + ShieldOfHopeBlock(level).ToString("") + " Block\nDestroy";
                    case 27:
                        return "Increase your Weapon Damage by " + HammerOfWrathDamage(level).ToString("") + "\nEvery Weapon Attack Draws 1 Card\nDestroy";
                    case 28:
                        if (PlayerScript.CursesCount > 0)
                            return "Gain " + ALightInTheDarknessSanityBlock(level).ToString("") + " Sanity & Block\nApply " + ALightInTheDarknessSlow(level).ToString("") + " Slow to all Enemies\nDestroy";
                        else return "Gain " + ALightInTheDarknessSanityBlock(level).ToString("") + " Sanity & Block\nDestroy";
                    case 29:
                        if (level > 1)
                            return "Gain " + BarricadeBlock(level).ToString("") + " Block\nBlock is retained for " + BarricadeRetain(level) + " Turns";
                        else return "Gain " + BarricadeBlock(level).ToString("") + " Block\nBlock is retained for 1 Turn";
                    case 30:
                        return "Deal " + RighteousHammerDamage(level).ToString("") + " Damage\nApply " + RighteousHammerDamage(level).ToString("") + " Daze\n& " + RighteousHammerSlow(level).ToString("") + " Slow";
                    case 31:
                        return "Gain " + LightsChosenStrength(level).ToString("") + " Strength\n" + LightsChosenResistance(level).ToString("") + " Resistance\n& " + LightsChosenDexterity(level).ToString("") + " Dexterity\nDestroy";
                    case 32:
                        return "Deal " + PenanceDamage(level).ToString("") + " Damage\nGain " + PenanceBlock(level).ToString("") + " Block";
                    case 33:
                        return "Gain " + GuardianAngelResistance(level).ToString("") + " Resistance\n" + GuardianAngelArmor(level).ToString("") + " Armor\nBlock gained from Armor is affected by Resistance\nDestroy";
                    case 34:
                        if (CombatScript.Enemy[CombatScript.targetedEnemy].IntentToAttack())
                            return "Deal " + VengeancekDamage(level).ToString("") + " Damage, Gain " + VengeanceStrength(level).ToString("") + " Strength\n& " + VengeanceEnergy(level).ToString("") + " Energy";
                        else return "Deal " + VengeancekDamage(level).ToString("") + " Damage";
                }
            }
            else
            {
                which -= Library.lightCards;
                switch (which)
                {
                    case 0:
                        return "Deal " + QuickCutDamage(level).ToString("") + " Damage";
                    case 1:
                        if (level == 0)
                            return "Deal " + HarpoonThrowDamage(level).ToString("") + " Damage\nDraw a Card";
                        else return "Deal " + HarpoonThrowDamage(level).ToString("") + " Damage\nDraw " + HarpoonThrowDraw(level).ToString("") + " Cards";
                    case 2:
                        return "Deal " + CutDownDamage(level).ToString("") + " Damage\nApply " + CutDownBleed(level).ToString("") + " Bleed";
                    case 3:
                        return "Apply " + EnsnareSlow(level).ToString("") + " Slow\nbut increase Targets\nTenacity by 1";
                    case 4:
                        return "Deal " + ViciousSlashDamage(level).ToString("") + " Damage\nApply " + ViciousSlashBleed(level).ToString("") + " Bleed";
                    case 5:
                        return "Deal " + RuptureDamage(level).ToString("") + " Damage";
                    case 6:
                        return "Gain " + ProtectiveBubbleBlock(level).ToString("") + " Block\n& " + ProtectiveBubbleShield(level).ToString("") + " Shield";
                    case 7:
                        return "Gain 1 Dexterity\nEvery Card playd gives " + SwiftBlock(level).ToString("") + " Block\nDestroy";
                    case 8:
                        return "Draw " + HopDraw(level).ToString("") + " Card\nGain " + HopBlock(level).ToString("") + " Block";
                    case 9:
                        return "Deal " + ImpaleDamage(level).ToString("") + " Damage\nApply " + ImpaleSlow(level).ToString("") + " Slow\n& gain that much Energy";
                    case 10:
                        return "Deal " + FlurryDamage(level).ToString("") + " Damage\nGain " + FlurryEnergy(level).ToString("") + " Energy\nIncrease future Flurry Energy gain by " + FlurryGain(level).ToString("");
                    case 11:
                        return "Gain 2 Strength\nEvery Attack applies " + SerratedBladeBleed(level).ToString("") + " Bleed\nDestroy";
                    case 12:
                        return "Gain " + ShellsUpBlock(level).ToString("") + " Block";
                    case 13:
                        return "Deal " + DeadlySwingsDamage(level).ToString("") + " Damage\n" + DeadlySwingsAmount(level).ToString("") + " Times";
                    case 14:
                        return "Deal " + SteelOfStealDamage(level).ToString("") + " Damage\nGain " + SteelOfStealSilver(level).ToString("") + " Silver\nDestroy";
                    case 15:
                        return "Deal " + EviscerateDamage(level).ToString("") + " Damage";
                    case 16:
                        if (combo < 3)
                            return "Gain " + DoubleJumpBlock(level).ToString("") + " Block\n(" + combo.ToString("") + "/3 Combo)";
                        else return "Gain " + DoubleJumpBlock(level).ToString("") + " Block Twice";
                    case 17:
                        if (combo < 4)
                            return combo.ToString("") + "/4 Combo)";
                        else if (level == 0) return "Draw a Card\nGain" + FlowLikeWaterMana(level).ToString("") + " Mana\n& " + FlowLikeWaterEnergy(level).ToString("") + " Energy";
                        else return "Draw " + FlowLikeWaterDraw(level).ToString("") + " Cards\nGain" + FlowLikeWaterMana(level).ToString("") + " Mana\n& " + FlowLikeWaterEnergy(level).ToString("") + " Energy";
                    case 18:
                        return "Gain " + PreparationBlock(level).ToString("") + " Block\n& +" + PreparationCombo(level).ToString("") + " Combo";
                    case 19:
                        return "Deal " + StaggeringBlowDamage(level).ToString("") + " Damage\nreduce Targets\nTenacity by 1\n& Apply " + StaggeringBlowSlow(level).ToString("") + " Slow\nDestroy";
                    case 20:
                        return "Deal " + DredgeLineDamage(level).ToString("") + " Damage\nApply " + DredgeLineSlow(level).ToString("") + " Slow\nGain " + DredgeLineEnergy(level).ToString("") + " Energy";
                    case 21:
                        return "Deal " + ShredDamage(level).ToString("") + " Damage\nBreak up to " + ShredBreak(level).ToString("") + " Shield\nGain that much Block";
                    case 22:
                        return "Gain " + StrengthOfTheDepthsStrength(level).ToString("") + " Strength\n& " + StrengthOfTheDepthsBlock(level).ToString("") + " Block";
                    case 23:
                        return "Increase your Weapon Damage by " + TridentOfStormsDamage(level).ToString("") + "\nEvery Weapon Attack Gives 1 Storm Charge\nDestroy";
                    case 24:
                        return "Gain " + ConduitBlock(level).ToString("") + " Block\n& " + ConduitCharges(level).ToString("") + " Storm Charges";
                    case 25:
                        if (energy >= 12)
                            return "Gain " + AnchoredBlock(level, true).ToString("") + " Block\n" + AnchoredResistance(level).ToString("") + " Resistance\nSpend 12 Energy";
                        else return "Gain " + AnchoredBlock(level, false).ToString("") + " Block\n(" + energy.ToString("") + "/12 Energy)";
                    case 26:
                        return "Gain " + NimbleEnergy(level).ToString("") + " Energy\n& " + NimbleBlock(level, NimbleEnergy(level)).ToString("") + " Block";
                    case 27:
                        if (level == 0)
                        {
                            if (combo < 2)
                                return "Deal " + SinkDamage(level).ToString("") + " Damage\n(" + combo.ToString("") + "/2 Combo)";
                            else return "Deal " + SinkDamage(level).ToString("") + " Damage\nGain 1 Strength";
                        }
                        else if (level == 1)
                        {
                            if (combo < 1)
                                return "Deal " + SinkDamage(level).ToString("") + " Damage\n(" + combo.ToString("") + "/1 Combo)";
                            else return "Deal " + SinkDamage(level).ToString("") + " Damage\nGain 1 Strength";
                        }
                        else
                        {
                            if (combo < 1)
                                return "Deal " + SinkDamage(level).ToString("") + " Damage\n(" + combo.ToString("") + "/1 Combo)\n(" + combo.ToString("") + "/3 Combo)";
                            else if (combo < 3)
                                return "Deal " + SinkDamage(level).ToString("") + " Damage\nGain 1 Strength\n(" + combo.ToString("") + "/3 Combo)";
                            else return "Deal " + SinkDamage(level).ToString("") + " Damage\nGain 2 Strength";
                        }
                    case 28:
                        return "Deal " + RendDamage(level).ToString("") + " Damage\nGain " + RendBlock(level).ToString("") + " Block\n" + RendAmount(level).ToString("") + " Times";
                    case 29:
                        return "Gain " + ThickSkinBlock(level).ToString("") + " Block\nIncrease Max Health by " + ThickSkinHealth(level).ToString("") + " permamently\nDestroy";
                }
            }
        }

        return "";
    }

    // ABILITIES
    // NEUTRAL
    void Strike(int level) // ID N 0
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

    void Defend(int level) // ID N 1
    {
        GainBlock(DefendBlock(level));
    }

    int DefendBlock(int level)
    {
        tempi = 8 + effect[1];
        tempi += 3 * level;
        return BlockGainedModifier(tempi);
    }

    // LIGHT
    void SpearThrust(int level) // ID L 0
    {
        CombatScript.Enemy[CombatScript.targetedEnemy].BreakShield(SpearThrustBreak(level));
        CombatScript.Enemy[CombatScript.targetedEnemy].TakeDamage(SpearThrustDamage(level));
        OnHit();
    }

    int SpearThrustBreak(int level)
    {
        tempi = 7;
        tempi += 3 * level;
        return tempi;
    }

    int SpearThrustDamage(int level)
    {
        tempi = 10 + effect[0];
        tempi += 3 * level;
        return DamageDealtModifier(tempi);
    }

    void Judgement(int level) // ID L 1
    {
        CombatScript.Enemy[CombatScript.targetedEnemy].TakeDamage(JudgementDamage(level));
        OnHit();
        if (CombatScript.Enemy[CombatScript.targetedEnemy].IntentToAttack())
            CombatScript.Enemy[CombatScript.targetedEnemy].GainVulnerable(JudgementVulnerable(level));
    }

    int JudgementDamage(int level)
    {
        tempi = 11 + effect[0];
        tempi += 3 * level;
        if (level > 1)
            tempi += 4;
        return DamageDealtModifier(tempi);
    }

    int JudgementVulnerable(int level)
    {
        tempi = 1;
        tempi += level;
        tempi -= level / 2;
        return tempi;
    }

    void BolaShot(int level) // ID L 2
    {
        CombatScript.Enemy[CombatScript.targetedEnemy].TakeDamage(BolaShotDamage(level));
        OnHit();
        CombatScript.Enemy[CombatScript.targetedEnemy].GainSlow(BolaShotSlow(level));
    }

    int BolaShotDamage(int level)
    {
        tempi = 8 + effect[0];
        tempi += 2 * level;
        return DamageDealtModifier(tempi);
    }

    int BolaShotSlow(int level)
    {
        tempi = 2;
        tempi += 1 * level;
        return tempi;
    }

    void CripplingStrike(int level) // ID L 3
    {
        CombatScript.Enemy[CombatScript.targetedEnemy].TakeDamage(CripplingStrikeDamage(level));
        OnHit();
        CombatScript.Enemy[CombatScript.targetedEnemy].GainBleed(CripplingStrikeBleed(level));
    }

    int CripplingStrikeDamage(int level)
    {
        tempi = 7 + effect[0];
        tempi += 1 * level;
        return DamageDealtModifier(tempi);
    }

    int CripplingStrikeBleed(int level)
    {
        tempi = 3;
        tempi += 2 * level;
        return tempi;
    }

    void Fortify(int level) // ID L 4
    {
        effect[3] += FortifyBlock(level);
        GainBlock(FortifyBlock(level));
    }

    int FortifyBlock(int level)
    {
        tempi = 7 + effect[1];
        tempi += 2 * level;
        return BlockGainedModifier(tempi);
    }

    void Empower(int level) // ID L 5
    {
        GainStrength(EmpowerBuff(level));
        GainEnergy(EmpowerBuff(level));
    }

    int EmpowerBuff(int level)
    {
        tempi = 2;
        tempi += level;
        return tempi;
    }

    void Inspire(int level) // ID L 6
    {
        Cards.Draw(InspireCardDraw(level));
        GainBlock(InspireBlock(level));
    }

    int InspireCardDraw(int level)
    {
        tempi = 2;
        tempi += level;
        tempi -= level / 2;
        return tempi;
    }

    int InspireBlock(int level)
    {
        tempi = 6 + effect[1];
        tempi += 2 * level;
        if (level > 1)
            tempi += 4;
        return BlockGainedModifier(tempi);
    }

    void ShieldBash(int level) // ID L 7
    {
        if (level > 0)
            GainBlock(ShieldBashBlock(level));
        CombatScript.Enemy[CombatScript.targetedEnemy].TakeDamage(ShieldBashDamage(level));
        OnHit();
    }

    int ShieldBashBlock(int level)
    {
        tempi = effect[1];
        tempi += 2 * level;
        return BlockGainedModifier(tempi);
    }

    int ShieldBashDamage(int level)
    {
        tempi = block + effect[0];
        return DamageDealtModifier(tempi);
    }

    void DesperateStand(int level) // ID L 8
    {
        GainBlock(DesperateStandBlock(level));
    }

    int DesperateStandBlock(int level)
    {
        tempi = 12 + effect[1];
        tempi += 4 * level;
        if (HealthProcentage() < 0.5f)
            tempi += 8 + 4 * level;
        return BlockGainedModifier(tempi);
    }

    void DecisiveStrike(int level) // ID L 9
    {
        CombatScript.Enemy[CombatScript.targetedEnemy].TakeDamage(DecisiveStrikeDamage(level));
        OnHit();
        GainValor(DecisiveStrikeValor(level));
    }

    int DecisiveStrikeDamage(int level)
    {
        tempi = 10 + effect[0];
        tempi += 1 * level;
        tempi += effect[4];
        return DamageDealtModifier(tempi);
    }

    int DecisiveStrikeValor(int level)
    {
        tempi = 1;
        tempi += level;
        return tempi;
    }

    void BulwarkOfLight(int level) // ID L 10
    {
        GainBlock(BulwarkOfLightBlock(level));
        GainValor(BulwarkOfLightValor(level));
    }

    int BulwarkOfLightBlock(int level)
    {
        tempi = 9 + effect[1];
        tempi += 1 * level;
        tempi += effect[4];
        return BlockGainedModifier(tempi);
    }

    int BulwarkOfLightValor(int level)
    {
        tempi = 1;
        tempi += level;
        return tempi;
    }

    void GoldenAegis(int level) // ID L 11
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
        tempi = 11 + effect[1];
        tempi += 2 * level;
        return BlockGainedModifier(tempi);
    }

    int GoldenAegisSlow(int level)
    {
        tempi = 1;
        tempi += level;
        return tempi;
    }

    void ShieldWall(int level) // ID L 12
    {
        GainBlock(ShieldWallBlock(level));
        GainResistance(ShieldWallResistance(level));
    }

    int ShieldWallBlock(int level)
    {
        tempi = 8 + effect[1];
        tempi += 4 * level;
        if (level > 1)
            tempi -= 3;
        return BlockGainedModifier(tempi);
    }

    int ShieldWallResistance(int level)
    {
        tempi = 1;
        tempi += level / 2;
        return tempi;
    }

    void ShieldGlare(int level) // ID L 13
    {
        GainBlock(ShieldGlareBlock(level));
        CombatScript.Enemy[CombatScript.targetedEnemy].GainWeak(ShieldGlareWeak(level));
    }

    int ShieldGlareBlock(int level)
    {
        tempi = 10 + effect[1];
        tempi += level;
        return BlockGainedModifier(tempi);
    }

    int ShieldGlareWeak(int level)
    {
        tempi = 1;
        tempi += level;
        return tempi;
    }

    void Smite(int level) // ID L 14
    {
        CombatScript.Enemy[CombatScript.targetedEnemy].TakeDamage(SmiteDamage(level));
        OnHit();
        CombatScript.Enemy[CombatScript.targetedEnemy].GainSlow(SmiteSlow(level));
    }

    int SmiteDamage(int level)
    {
        tempi = 6 + effect[0];
        tempi += 1 * level;
        return DamageDealtModifier(tempi);
    }

    int SmiteSlow(int level)
    {
        tempi = 1;
        tempi += level;
        return tempi;
    }

    void BlindingLight(int level) // ID L 15
    {
        CombatScript.Enemy[CombatScript.targetedEnemy].TakeDamage(BlindingLightDamage(level));
        OnHit();
        CombatScript.Enemy[CombatScript.targetedEnemy].GainDaze(BlindingLightDaze(level));
        CombatScript.Enemy[CombatScript.targetedEnemy].GainWeak(BlindingLightWeak(level));
    }

    int BlindingLightDamage(int level)
    {
        tempi = 17 + effect[0];
        tempi += 4 * level;
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

    void Consecration(int level) // ID L 16
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
        tempi = 16 + effect[0];
        tempi += 5 * level;
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

    void HolyLight(int level) // ID L 17
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
        tempi = 4;
        tempi += 2 * level;
        return tempi;
    }

    void Chastise(int level) // ID L 18
    {
        CombatScript.Enemy[CombatScript.targetedEnemy].TakeDamage(ChastiseDamage(level));
        OnHit();
        CombatScript.Enemy[CombatScript.targetedEnemy].GainDaze(ChastiseDaze(level));
        CombatScript.Enemy[CombatScript.targetedEnemy].GainSlow(ChastiseSlow(level));
    }

    int ChastiseDamage(int level)
    {
        tempi = 9 + effect[0];
        tempi += 4 * level;
        tempi += CombatScript.Enemy[CombatScript.targetedEnemy].effect[2];
        return DamageDealtModifier(tempi);
    }

    int ChastiseSlow(int level)
    {
        tempi = 3;
        tempi += level;
        return tempi;
    }

    int ChastiseDaze(int level)
    {
        tempi = 6;
        tempi += 2 * level;
        return tempi;
    }

    void HolyBolt(int level) // ID L 19
    {
        CombatScript.Enemy[CombatScript.targetedEnemy].TakeDamage(HolyBoltDamage(level));
        OnHit();
    }

    int HolyBoltDamage(int level)
    {
        tempi = 7 + effect[0];
        tempi += 2 * level;
        tempi += (2 + level) * effect[4];
        return DamageDealtModifier(tempi);
    }

    void LayOnHands(int level) // ID L 20
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
        tempi = 2;
        tempi += 2 * level;
        return tempi;
    }

    void CounterAttack(int level) // ID L 21
    {
        CombatScript.Enemy[CombatScript.targetedEnemy].TakeDamage(CounterAttackDamage(level));
        OnHit();
    }

    int CounterAttackDamage(int level)
    {
        tempi = 2 + effect[0];
        tempi += 4 * level;
        if (CombatScript.Enemy[CombatScript.targetedEnemy].IntentToAttack())
            tempi += CombatScript.Enemy[CombatScript.targetedEnemy].AttackDamage();
        return DamageDealtModifier(tempi);
    }

    void SurgeOfLight(int level) // ID L 22
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
        tempi = 3;
        tempi += 2 * level;
        return tempi;
    }

    void PatientStrike(int level) // ID L 23
    {
        CombatScript.Enemy[CombatScript.targetedEnemy].TakeDamage(PatientStrikeDamage(level));
        OnHit();
    }

    int PatientStrikeDamage(int level)
    {
        tempi = 6 + effect[0];
        tempi += 1 * level;
        tempi += (2 + level) * CombatScript.turn;
        return DamageDealtModifier(tempi);
    }

    void CrushingBlow(int level) // ID L 24
    {
        CombatScript.Enemy[CombatScript.targetedEnemy].TakeDamage(CrushingBlowDamage(level));
        OnHit();
        CombatScript.Enemy[CombatScript.targetedEnemy].GainSlow(CrushingBlowSlow(level));
        CombatScript.Enemy[CombatScript.targetedEnemy].GainWeak(CrushingBlowWeak(level));
        CombatScript.Enemy[CombatScript.targetedEnemy].GainVulnerable(CrushingBlowVulnerable(level));
    }

    int CrushingBlowDamage(int level)
    {
        tempi = 20 + effect[0];
        tempi += 5 * level;
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
        tempi -= level / 2;
        return tempi;
    }

    int CrushingBlowVulnerable(int level)
    {
        tempi = 1;
        tempi += level / 2;
        return tempi;
    }

    void HeavyArmor(int level) // ID L 25
    {
        GainBlock(HeavyArmorBlock(level));
    }

    int HeavyArmorBlock(int level)
    {
        tempi = 10 + 2 * effect[1];
        tempi += (2 + effect[1]) * level;
        return BlockGainedModifier(tempi);
    }

    void ShieldOfHope(int level) // ID L 26
    {
        GainBlock(ShieldOfHopeBlock(level));
    }

    int ShieldOfHopeBlock(int level)
    {
        tempi = ((maxHealth - health) * (2 + level)) / 4 + effect[1];
        return BlockGainedModifier(tempi);
    }

    void HammerOfWrath(int level) // ID L 27
    {
        baseDamage += HammerOfWrathDamage(level);
        effect[5]++;
        Display(1, effectSprite[5]);
    }

    int HammerOfWrathDamage(int level)
    {
        tempi = 2;
        tempi += 2 * level;
        return BlockGainedModifier(tempi);
    }

    void ALightInTheDarkness(int level) // ID L 28
    {
        GainSanity(ALightInTheDarknessSanityBlock(level));
        GainBlock(ALightInTheDarknessSanityBlock(level));
        if (PlayerScript.CursesCount > 0)
        {
            for (int i = 0; i < CombatScript.enemyAlive.Length; i++)
            {
                if (CombatScript.enemyAlive[i])
                    CombatScript.Enemy[CombatScript.targetedEnemy].GainSlow(ALightInTheDarknessSlow(level));
            }
        }
    }

    int ALightInTheDarknessSanityBlock(int level)
    {
        tempi = 6;
        tempi += 2 * level;
        return tempi;
    }

    int ALightInTheDarknessSlow(int level)
    {
        tempi = 1;
        tempi += level;
        tempi *= PlayerScript.CursesCount;
        return tempi;
    }

    void Barricade(int level) // ID L 29
    {
        GainBlock(BarricadeBlock(level));
        effect[11] += BarricadeRetain(level);
        Display(BarricadeRetain(level), effectSprite[11]);
    }

    int BarricadeBlock(int level)
    {
        tempi = 11 + effect[1];
        tempi += 4 * level;
        if (tempi > 1)
            tempi -= 2;
        return BlockGainedModifier(tempi);
    }

    int BarricadeRetain(int level)
    {
        tempi = 1;
        tempi += level / 2;
        return tempi;
    }

    void RighteousHammer(int level) // ID L 30
    {
        CombatScript.Enemy[CombatScript.targetedEnemy].TakeDamage(RighteousHammerDamage(level));
        OnHit();
        CombatScript.Enemy[CombatScript.targetedEnemy].GainDaze(RighteousHammerDamage(level));
        CombatScript.Enemy[CombatScript.targetedEnemy].GainSlow(RighteousHammerSlow(level));
    }

    int RighteousHammerDamage(int level)
    {
        tempi = 6 + effect[0];
        tempi += 1 * level;
        return DamageDealtModifier(tempi);
    }

    int RighteousHammerSlow(int level)
    {
        tempi = 1;
        tempi += level;
        return tempi;
    }

    void LightsChosen(int level) // ID L 31
    {
        GainStrength(LightsChosenStrength(level));
        GainResistance(LightsChosenResistance(level));
        GainDexterity(LightsChosenDexterity(level));
    }

    int LightsChosenStrength(int level)
    {
        tempi = 1;
        tempi += level;
        return tempi;
    }

    int LightsChosenResistance(int level)
    {
        tempi = 1;
        tempi += level;
        tempi -= level / 2;
        return tempi;
    }

    int LightsChosenDexterity(int level)
    {
        tempi = 1;
        tempi += level / 2;
        return tempi;
    }

    void Penance(int level) // ID L 32
    {
        CombatScript.Enemy[CombatScript.targetedEnemy].TakeDamage(PenanceDamage(level));
        OnHit();
        GainBlock(PenanceBlock(level));
    }

    int PenanceDamage(int level)
    {
        tempi = 7 + effect[0];
        tempi += 2 * level;
        return DamageDealtModifier(tempi);
    }

    int PenanceBlock(int level)
    {
        tempi = 6 + effect[1];
        tempi += 2 * level;
        return BlockGainedModifier(tempi);
    }

    void GuardianAngel(int level) // ID L 33
    {
        GainResistance(GuardianAngelResistance(level));
        GainArmor(GuardianAngelArmor(level));
        effect[13] += 1;
        Display(1, effectSprite[13]);
    }

    int GuardianAngelResistance(int level)
    {
        tempi = 2;
        tempi += level / 2;
        return tempi;
    }

    int GuardianAngelArmor(int level)
    {
        tempi = 4;
        tempi += 3 * level;
        if (level > 1)
            tempi -= 2;
        return tempi;
    }

    void Vengeance(int level) // ID L 34
    {
        CombatScript.Enemy[CombatScript.targetedEnemy].TakeDamage(VengeancekDamage(level));
        OnHit();
        if (CombatScript.Enemy[CombatScript.targetedEnemy].IntentToAttack())
        {
            GainStrength(VengeanceStrength(level));
            GainEnergy(VengeanceEnergy(level));
        }
    }

    int VengeancekDamage(int level)
    {
        tempi = 11 + effect[0];
        tempi += 2 * level;
        return DamageDealtModifier(tempi);
    }

    int VengeanceStrength(int level)
    {
        tempi = 1;
        tempi += level;
        return tempi;
    }

    int VengeanceEnergy(int level)
    {
        tempi = 4;
        tempi += 2 * level;
        return tempi;
    }

    // WATER
    void QuickCut(int level) // ID W 0
    {
        CombatScript.Enemy[CombatScript.targetedEnemy].TakeDamage(QuickCutDamage(level));
        OnHit();
    }

    int QuickCutDamage(int level)
    {
        tempi = 8 + effect[0];
        tempi += 3 * level;
        return DamageDealtModifier(tempi);
    }

    void HarpoonThrow(int level) // ID W 1
    {
        CombatScript.Enemy[CombatScript.targetedEnemy].TakeDamage(HarpoonThrowDamage(level));
        OnHit();
        Cards.Draw(HarpoonThrowDraw(level));
    }

    int HarpoonThrowDamage(int level)
    {
        tempi = 11 + effect[0];
        tempi += 1 * level;
        if (level > 1)
            tempi += 3;
        return DamageDealtModifier(tempi);
    }

    int HarpoonThrowDraw(int level)
    {
        tempi = 1;
        tempi += (1 + level) / 2;
        return tempi;
    }

    void CutDown(int level) // ID W 2
    {
        CombatScript.Enemy[CombatScript.targetedEnemy].TakeDamage(CutDownDamage(level));
        OnHit();
        CombatScript.Enemy[CombatScript.targetedEnemy].GainBleed(CutDownBleed(level));
    }

    int CutDownDamage(int level)
    {
        tempi = 2 + effect[0];
        tempi += 1 * level;
        return DamageDealtModifier(tempi);
    }

    int CutDownBleed(int level)
    {
        tempi = 2;
        tempi += 2 * level;
        tempi += CombatScript.Enemy[CombatScript.targetedEnemy].tenacity;
        return tempi;
    }

    void Ensnare(int level) // ID W 3
    {
        CombatScript.Enemy[CombatScript.targetedEnemy].tenacity++;
        CombatScript.Enemy[CombatScript.targetedEnemy].GainSlow(EnsnareSlow(level));
    }

    int EnsnareSlow(int level)
    {
        tempi = 6;
        tempi += 3 * level;
        return tempi;
    }

    void ViciousSlash(int level) // ID W 4
    {
        CombatScript.Enemy[CombatScript.targetedEnemy].TakeDamage(ViciousSlashDamage(level));
        OnHit();
        CombatScript.Enemy[CombatScript.targetedEnemy].GainBleed(ViciousSlashBleed(level));
    }

    int ViciousSlashDamage(int level)
    {
        tempi = 6 + effect[0];
        tempi += 1 * level;
        return DamageDealtModifier(tempi);
    }

    int ViciousSlashBleed(int level)
    {
        tempi = 2 + effect[0];
        tempi += 2 * level;
        return tempi;
    }

    void Rupture(int level) // ID W 5
    {
        CombatScript.Enemy[CombatScript.targetedEnemy].TakeDamage(RuptureDamage(level));
        OnHit();
    }

    int RuptureDamage(int level)
    {
        tempi = 1 + effect[0];
        tempi += 4 * level;
        tempi += CombatScript.Enemy[CombatScript.targetedEnemy].effect[1];
        return DamageDealtModifier(tempi);
    }

    void ProtectiveBubble(int level) // ID W 6
    {
        GainBlock(ProtectiveBubbleBlock(level));
        GainShield(ProtectiveBubbleShield(level));
    }

    int ProtectiveBubbleBlock(int level)
    {
        tempi = 13 + effect[1];
        tempi += 2 * level;
        return BlockGainedModifier(tempi);
    }

    int ProtectiveBubbleShield(int level)
    {
        tempi = 5;
        tempi += 3 * level;
        return tempi;
    }

    void Swift(int level) // ID W 7
    {
        GainDexterity(1);
        effect[14] += SwiftBlock(level);
        Display(SwiftBlock(level), effectSprite[14]);
    }

    int SwiftBlock(int level)
    {
        tempi = 1 + level;
        return tempi;
    }

    void Hop(int level) // ID W 8
    {
        Cards.Draw(HopDraw(level));
        GainBlock(HopBlock(level));
    }

    int HopDraw(int level)
    {
        tempi = 1;
        return tempi;
    }

    int HopBlock(int level)
    {
        tempi = 3;
        tempi += 3 * level;
        /*tempi += level / 2;
        tempi *= (Cards.CardsInHand - 1);
        tempi += effect[1];*/
        return BlockGainedModifier(tempi);
    }

    void Impale(int level) // ID W 9
    {
        CombatScript.Enemy[CombatScript.targetedEnemy].TakeDamage(ImpaleDamage(level));
        OnHit();
        CombatScript.Enemy[CombatScript.targetedEnemy].GainSlow(ImpaleSlow(level));
        GainEnergy(ImpaleSlow(level));
    }

    int ImpaleDamage(int level)
    {
        tempi = 16 + effect[0];
        tempi += 5 * level;
        if (level > 1)
            tempi -= 3;
        return DamageDealtModifier(tempi);
    }

    int ImpaleSlow(int level)
    {
        tempi2 = 6;
        tempi2 -= level / 2;
        tempi = (ImpaleDamage(level) / tempi2);
        return tempi;
    }

    void Flurry(int level) // ID W 10
    {
        CombatScript.Enemy[CombatScript.targetedEnemy].TakeDamage(FlurryDamage(level));
        OnHit();
        GainEnergy(FlurryEnergy(level));
        flurry += FlurryGain(level);
    }

    int FlurryDamage(int level)
    {
        tempi = 9 + effect[0];
        tempi += 2 * level;
        return DamageDealtModifier(tempi);
    }

    int FlurryEnergy(int level)
    {
        tempi = 2;
        tempi += level;
        tempi += flurry;
        return tempi;
    }

    int FlurryGain(int level)
    {
        tempi = 1;
        tempi += level;
        return tempi;
    }

    void SerratedBlade(int level) // ID W 11
    {
        GainStrength(2);
        effect[15] += SerratedBladeBleed(level);
        Display(SerratedBladeBleed(level), effectSprite[15]);
    }

    int SerratedBladeBleed(int level)
    {
        tempi = 1;
        tempi += level / 2;
        return tempi;
    }

    void ShellsUp(int level) // ID W 12
    {
        GainBlock(ShellsUpBlock(level));
    }

    int ShellsUpBlock(int level)
    {
        tempi = 15 + effect[1];
        tempi += 4 * level;
        return BlockGainedModifier(tempi);
    }

    void DeadlySwings(int level) // ID W 13
    {
        for (int i = 0; i < DeadlySwingsAmount(level); i++)
        {
            CombatScript.Enemy[CombatScript.targetedEnemy].TakeDamage(DeadlySwingsDamage(level));
            OnHit();
        }
    }

    int DeadlySwingsDamage(int level)
    {
        tempi = 7 + effect[0];
        return DamageDealtModifier(tempi);
    }

    int DeadlySwingsAmount(int level)
    {
        tempi = 2 + level;
        return tempi;
    }

    void SteelOfSteal(int level) // ID W 14
    {
        CombatScript.Enemy[CombatScript.targetedEnemy].TakeDamage(SteelOfStealDamage(level));
        OnHit();
        PlayerScript.GainSilver(SteelOfStealSilver(level));
    }

    int SteelOfStealDamage(int level)
    {
        tempi = 9 + effect[0];
        tempi += 2 * level;
        return DamageDealtModifier(tempi);
    }

    int SteelOfStealSilver(int level)
    {
        tempi = 10 + 4 * level;
        return tempi;
    }

    void Eviscerate(int level) // ID W 15
    {
        CombatScript.Enemy[CombatScript.targetedEnemy].TakeDamage(EviscerateDamage(level));
        OnHit();
    }

    int EviscerateDamage(int level)
    {
        tempi = 8 + effect[0];
        tempi += 1 * level;
        tempi += combo * (4 + 2 * level);
        return DamageDealtModifier(tempi);
    }

    void DoubleJump(int level) // ID W 16
    {
        GainBlock(DoubleJumpBlock(level));
        if (combo >= 3)
            GainBlock(DoubleJumpBlock(level));
    }

    int DoubleJumpBlock(int level)
    {
        tempi = 11 + effect[1];
        tempi += 3 * level;
        return BlockGainedModifier(tempi);
    }

    void FlowLikeWater(int level) // ID W 17
    {
        if (combo >= 4)
        {
            GainMana(FlowLikeWaterMana(level));
            Cards.Draw(FlowLikeWaterDraw(level));
            GainEnergy(FlowLikeWaterMana(level));
        }
    }

    int FlowLikeWaterMana(int level)
    {
        tempi = 1;
        tempi += level / 2;
        return tempi;
    }

    int FlowLikeWaterDraw(int level)
    {
        tempi = 1;
        tempi += (1 + level) / 2;
        return tempi;
    }

    int FlowLikeWaterEnergy(int level)
    {
        tempi = 1;
        tempi += level;
        return tempi;
    }

    void Preparation(int level) // ID W 18
    {
        GainBlock(PreparationBlock(level));
        combo += PreparationCombo(level);
    }

    int PreparationBlock(int level)
    {
        tempi = 4 + effect[1];
        tempi += 3 * level;
        if (level > 1)
            tempi -= 2;
        return BlockGainedModifier(tempi);
    }

    int PreparationCombo(int level)
    {
        tempi = 1;
        tempi += level / 2;
        return tempi;
    }

    void StaggeringBlow(int level) // ID W 19
    {
        CombatScript.Enemy[CombatScript.targetedEnemy].TakeDamage(StaggeringBlowDamage(level));
        OnHit();
        CombatScript.Enemy[CombatScript.targetedEnemy].tenacity--;
        CombatScript.Enemy[CombatScript.targetedEnemy].GainSlow(StaggeringBlowSlow(level));
    }

    int StaggeringBlowDamage(int level)
    {
        tempi = 12 + effect[0];
        tempi += 6 * level;
        return DamageDealtModifier(tempi);
    }

    int StaggeringBlowSlow(int level)
    {
        tempi = 2;
        tempi += level;
        return tempi;
    }

    void DredgeLine(int level) // ID W 20
    {
        CombatScript.Enemy[CombatScript.targetedEnemy].TakeDamage(DredgeLineDamage(level));
        OnHit();
        CombatScript.Enemy[CombatScript.targetedEnemy].GainSlow(DredgeLineSlow(level));
        GainEnergy(DredgeLineEnergy(level));
    }

    int DredgeLineDamage(int level)
    {
        tempi = 9 + effect[0];
        tempi += 1 * level;
        return DamageDealtModifier(tempi);
    }

    int DredgeLineSlow(int level)
    {
        tempi = 1;
        tempi += level;
        return tempi;
    }

    int DredgeLineEnergy(int level)
    {
        tempi = 4;
        tempi += level;
        return tempi;
    }

    void Shred(int level) // ID W 21
    {
        if (CombatScript.Enemy[CombatScript.targetedEnemy].TotalBlock() > 0)
        {
            if (CombatScript.Enemy[CombatScript.targetedEnemy].TotalBlock() >= ShredBreak(level))
                GainBlock(ShredBreak(level));
            else GainBlock(CombatScript.Enemy[CombatScript.targetedEnemy].TotalBlock());
        }
        CombatScript.Enemy[CombatScript.targetedEnemy].BreakShield(ShredBreak(level));
        CombatScript.Enemy[CombatScript.targetedEnemy].TakeDamage(ShredDamage(level));
        OnHit();
    }

    int ShredDamage(int level)
    {
        tempi = 1 + effect[0];
        tempi += level;
        return DamageDealtModifier(tempi);
    }

    int ShredBreak(int level)
    {
        tempi = 8;
        tempi += 3 * level;
        return tempi;
    }

    void StrengthOfTheDepths(int level) // ID W 22
    {
        GainStrength(StrengthOfTheDepthsStrength(level));
        GainBlock(StrengthOfTheDepthsBlock(level));
    }

    int StrengthOfTheDepthsStrength(int level)
    {
        tempi = 1;
        tempi += level;
        return tempi;
    }

    int StrengthOfTheDepthsBlock(int level)
    {
        tempi = 12 + effect[1];
        tempi += 3 * level;
        return BlockGainedModifier(tempi);
    }

    void TridentOfStorms(int level) // ID W 23
    {
        baseDamage += TridentOfStormsDamage(level);
        effect[17]++;
        Display(1, effectSprite[17]);
    }

    int TridentOfStormsDamage(int level)
    {
        tempi = 2;
        tempi += 2 * level;
        return BlockGainedModifier(tempi);
    }

    void Conduit(int level) // ID W 24
    {
        GainBlock(ConduitBlock(level));
        GainStormCharge(ConduitCharges(level));
    }

    int ConduitBlock(int level)
    {
        tempi = 11 + effect[1];
        tempi += 4 * level;
        if (level > 1)
            tempi -= 2;
        return BlockGainedModifier(tempi);
    }

    int ConduitCharges(int level)
    {
        tempi = 2;
        tempi += level / 2;
        return tempi;
    }

    void Anchored(int level) // ID W 25
    {
        if (energy >= 12)
        {
            GainBlock(AnchoredBlock(level, true));
            GainResistance(AnchoredResistance(level));
            SpendEnergy(12);
        }
        else GainBlock(AnchoredBlock(level, false));
    }

    int AnchoredBlock(int level, bool empowered)
    {
        tempi = 11 + effect[1];
        tempi += 2 * level;
        tempi += level / 2;
        if (empowered)
        {
            tempi += 8;
            tempi += 2 * level;
            tempi += (level / 2) * 3;
        }
        return BlockGainedModifier(tempi);
    }

    int AnchoredResistance(int level)
    {
        tempi = 1;
        tempi += (level + 1) / 2;
        return tempi;
    }

    void Nimble(int level) // ID W 26
    {
        GainEnergy(NimbleEnergy(level));
        GainBlock(NimbleBlock(level, 0));
    }

    int NimbleEnergy(int level)
    {
        tempi = 3;
        tempi += 2 * level;
        return tempi;
    }

    int NimbleBlock(int level, int bonus)
    {
        tempi = energy + bonus + effect[1];
        return BlockGainedModifier(tempi);
    }

    void Sink(int level) // ID W 27
    {
        CombatScript.Enemy[CombatScript.targetedEnemy].TakeDamage(SinkDamage(level));
        OnHit();
        if (level == 0 && combo >= 2)
            GainStrength(SinkStrength(level, false));
        else if (level > 0 && combo >= 1)
        {
            if (level > 1 && combo >= 3)
                GainStrength(SinkStrength(level, true));
            else GainStrength(SinkStrength(level, false));
        }
    }

    int SinkDamage(int level)
    {
        tempi = 12 + effect[0];
        tempi += 3 * level;
        return DamageDealtModifier(tempi);
    }

    int SinkStrength(int level, bool empowered)
    {
        tempi = 1;
        if (empowered)
            tempi++;
        return tempi;
    }

    void Rend(int level) // ID W 28
    {
        for (int i = 0; i < RendAmount(level); i++)
        {
            CombatScript.Enemy[CombatScript.targetedEnemy].TakeDamage(RendDamage(level));
            OnHit();
            GainBlock(RendBlock(level));
        }
    }

    int RendDamage(int level)
    {
        tempi = 6 + effect[0] + effect[2];
        tempi += level / 2;
        return DamageDealtModifier(tempi);
    }

    int RendBlock(int level)
    {
        tempi = 4 + effect[1] + effect[2];
        tempi += (level + 1) / 2;
        return BlockGainedModifier(tempi);
    }

    int RendAmount(int level)
    {
        tempi = 3 + level;
        return tempi;
    }

    void ThickSkin(int level) // ID W 29
    {
        GainBlock(ThickSkinBlock(level));
        GainHealth(ThickSkinHealth(level));
    }

    int ThickSkinBlock(int level)
    {
        tempi2 = 10 - level;
        tempi = (maxHealth / tempi2) + effect[1];
        return BlockGainedModifier(tempi);
    }

    int ThickSkinHealth(int level)
    {
        tempi = 2 + 2 * level;
        return tempi;
    }

    // checks
    public int TotalBlock()
    {
        tempi = shield + block;
        return tempi;
    }
}
