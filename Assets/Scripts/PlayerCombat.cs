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
    int tempi, tempi2, tempi3;
    float temp;

    [Header("Ability Stats")]
    int valor, spentValor, combo, permanentCombo, flurry, lightningDamage, blossom, totalManaSpent, manaSpentTurn, savageryIncrease, wrath, wrathCharges, rampageIncrease;

    [Header("Item Stats")]
    public int turns;
    public int attacks, drink, extraWrathTick;
    public bool resistanceRing;

    [Header("Weapon")]
    public GameObject TheWeapon;
    public int baseDamage, strengthScaling, energyCost;
    public Image TheWeaponIcon;
    public TMPro.TextMeshProUGUI TheWeaponName, TheWeaponCost, TheWeaponEffect;

    [Header("Equipment")]
    public int equipmentAmount;
    public int[] eqID;
    public EquipmentLibrary ELibrary;
    public int[] eqEnergyCost, uses, cooldown, gain, maxCooldown;
    public Image[] EquipmentIcon, EquipmentFill;
    public Button[] EquipmentUseButton;
    public TMPro.TextMeshProUGUI[] equipmentCost, equipmentUses, equipmentCooldown;

    [Header("UI")]
    public GameObject ShieldDisplay;
    public GameObject BlockDisplay;
    public Image HealthBarFill, SanityBarFill, EnergyBarFill;
    public Button WeaponUseButton;
    public TMPro.TextMeshProUGUI HealthValue, ShieldValue, BlockValue, SanityValue, EnergyValue, ValorValue, ComboValue, BlossomValue, WrathValue, WeaponCost, ManaValue;
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
    public Sprite DamageSprite, MagicDamageSprite, HealthSprite, RestoreSprite, ShieldSprite, BlockSprite, InsanitySprite, SanitySprite, ValorSprite;
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
        block = 0;
        mana = 0;
        energy = 0;
        valor = 0;
        spentValor = 0;
        combo = 0;
        permanentCombo = 0;
        if (PlayerScript.Item[44])
            permanentCombo++;
        flurry = 0;
        blossom = 0;
        if (PlayerScript.Item[49])
            blossom += 3;
        totalManaSpent = 0;
        savageryIncrease = 0;
        wrath = 0;
        wrathCharges = 0;
        rampageIncrease = 0;
        attacks = 0;
        drink = 0;
        if (PlayerScript.Item[52])
            extraWrathTick = 1;
        lightningDamage = 36;
        if (PlayerScript.Item[46])
            lightningDamage += 11;
        if (PlayerScript.Item[53] && PlayerScript.CursesCount > 0)
        {
            GainStrength(PlayerScript.CursesCount);
            GainShield(PlayerScript.CursesCount * 3);
        }
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

        for (int i = 0; i < CurseObject.Length; i++)
        {
            CurseObject[i].SetActive(false);
        }
        for (int i = 0; i < PlayerScript.CurseValue.Length; i++)
        {
            if (PlayerScript.CurseValue[i] > 0)
                GainCurseEffect(i, PlayerScript.CurseValue[i]);
        }

        if (PlayerScript.Item[7])
            effect[4] += 2;
        if (PlayerScript.Item[39])
            effect[0] += (PlayerScript.MaxHealth - PlayerScript.BaseHealth) / 20;
        if (PlayerScript.Item[48])
            GainBlock(effect[0] * 4);

        if (PlayerScript.secondEquipment)
            equipmentAmount = 2;
        else equipmentAmount = 1;
        for (int i = 0; i < equipmentAmount; i++)
        {
            eqID[i] = PlayerScript.equipment[i];
            if (PlayerScript.Item[38])
                eqEnergyCost[i] = 0;
            else eqEnergyCost[i] = ELibrary.Equipments[eqID[i]].Cost;
            uses[i] = ELibrary.Equipments[eqID[i]].Uses;
            maxCooldown[i] = ELibrary.Equipments[eqID[i]].Cooldown;
            gain[i] = ELibrary.Equipments[eqID[i]].Gain;
            cooldown[i] = 0;
            EquipmentIcon[i].sprite = ELibrary.Equipments[eqID[i]].EquipmentSprite;
        }

        ItemsScript.SetText();
        StartTurn();
    }

    public void Set()
    {
        ItemsScript.ResetText();
        if (PlayerScript.Item[16])
            RestoreHealth((maxHealth - health) * 6 / 100);
        PlayerScript.MaxHealth = maxHealth;
        PlayerScript.Health = health;
        PlayerScript.MaxSanity = maxSanity;
        PlayerScript.Sanity = sanity;
        PlayerScript.UpdateInfo();
    }

    public void StartTurn()
    {
        combo = permanentCombo;
        GainBlossom(1);
        manaSpentTurn = 0;
        resistanceRing = true;
        if (effect[11] > 0)
            effect[11]--;
        else
        {
            if (PlayerScript.Item[0])
            {
                if (block > 13)
                    block = 13;
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
        if (effect[25] > 0)
        {
            GainEnergy(effect[25]);
            effect[25] = 0;
        }
        mana = 0;
        if (effect[7] > 0)
            GainMana(manaGain - 1);
        else GainMana(manaGain);
        if (effect[22] > 0)
        {
            GainMana(effect[22]);
            effect[22] = 0;
        }
        if (CombatScript.Enemy[0].effect[21] > 0 || CombatScript.Enemy[1].effect[21] > 0)
            GainMana(CombatScript.Enemy[0].effect[21] + CombatScript.Enemy[1].effect[21]);
        if (PlayerScript.Item[5] && CombatScript.turn < 3)
            GainMana(1);
        if (PlayerScript.Item[8])
        {
            turns++;
            if (turns % 3 == 0)
            {
                GainMana(1);
                Cards.Draw(1);
            }
        }
        if (effect[12] > 0)
            Cards.Draw(cardDraw - 1);
        else Cards.Draw(cardDraw);
        if (effect[23] > 0)
        {
            Cards.Draw(effect[23]);
            effect[23] = 0;
        }
        if (effect[21] > 0)
        {
            for (int i = 0; i < effect[21]; i++)
            {
                Riptide(0, false);
            }
            effect[21] = 0;
        }
        effect[24] = 0;
        if (PlayerScript.Item[23] && CombatScript.turn % 2 == 1)
            Cards.Draw(1);
        if (PlayerScript.CurseValue[2] > 0 && CombatScript.turn % 2 == 0)
            CombatScript.EnemiesGainStrength(PlayerScript.CurseValue[2]);
        EquipmentCooldown(1);
        if (PlayerScript.Item[23] && CombatScript.turn % 2 == 0)
            EquipmentCooldown(2);
        if (PlayerScript.Item[41])
            GainValor(1);
        if (PlayerScript.Item[46])
            GainStormCharge(1);
        if (PlayerScript.Item[47])
            GainBlock(effect[2]);
        if (PlayerScript.Item[52])
            GainWrath(2, 50);
    }

    public void UpdateInfo()
    {
        HealthBarFill.fillAmount = (health * 1f) / (maxHealth * 1f);
        SanityBarFill.fillAmount = (sanity * 1f) / (maxSanity * 1f);
        EnergyBarFill.fillAmount = (energy * 1f) / (energyCost * 1f);
        HealthValue.text = health.ToString("") + "/" + maxHealth.ToString("");
        SanityValue.text = sanity.ToString("") + "/" + maxSanity.ToString("");
        WeaponCost.text = energy.ToString("") + "/" + energyCost.ToString("");
        for (int i = 0; i < equipmentAmount; i++)
        {
            if (eqEnergyCost[i] != 0)
                EquipmentFill[i].fillAmount = (energy * 1f) / (eqEnergyCost[i] * 1f);
            equipmentCost[i].text = energy.ToString("") + "/" + eqEnergyCost[i].ToString("");
            equipmentUses[i].text = uses[i].ToString("");
            equipmentCooldown[i].text = cooldown[i].ToString("") + "/" + maxCooldown[i].ToString("");
        }
        ManaValue.text = mana.ToString("");
        EnergyValue.text = energy.ToString("");
        ValorValue.text = valor.ToString("");
        ComboValue.text = combo.ToString("");
        BlossomValue.text = blossom.ToString("");
        WrathValue.text = wrath.ToString("");
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
        for (int i = 0; i < equipmentAmount; i++)
        {
            if (energy >= eqEnergyCost[i] && uses[i] > 0)
                EquipmentUseButton[i].interactable = true;
            else EquipmentUseButton[i].interactable = false;
        }

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

    public void Effect(GameObject effect, float rotation)
    {
        Origin.rotation = Quaternion.Euler(Origin.rotation.x, Origin.rotation.y, Body.rotation + rotation);
        GameObject display = Instantiate(effect, Origin.position, Origin.rotation);
    }

    public void EndTurn()
    {
        if (energy > 10)
            energy = 10;
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
        if (effect[20] > 0)
            TakeMagicDamage(effect[20]);
        if (CombatScript.Enemy[0].effect[20] > 0 && mana > 0)
            TakeDamage(mana * 12);
        if (CombatScript.Enemy[1].effect[20] > 0 && mana > 0)
            TakeDamage(mana * 12);
        if (PlayerScript.CurseValue[1] > 0 && Cards.CardsInHand > 0)
            TakeDamage(PlayerScript.CurseValue[1] * Cards.CardsInHand * 4);
        if (PlayerScript.Item[6])
            TakeDamage(2 + CombatScript.turn * 1);
        if (PlayerScript.Item[16] && effect[8] > 0)
            effect[8]--;
        if (PlayerScript.Item[40] && mana > 0)
        {
            GainStrength(1);
            GainBlock(5);
        }
        //mana = 0;
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
            maxSanity += 8 + maxSanity / 24;
            sanity += maxSanity;
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
        temp = Random.Range(CombatScript.turn * 0.243f - 0.502f, CombatScript.turn * 0.485f - 0.468f);
        tempi = 0;
        for (float i = 1f; i < temp; i += 1f)
        {
            tempi++;
        }
        return tempi;
    }

    public void EquipmentCooldown(int amount)
    {
        for (int i = 0; i < equipmentAmount; i++)
        {
            cooldown[i] += amount;
            while (cooldown[i] >= maxCooldown[i])
            {
                cooldown[i] -= maxCooldown[i];
                uses[i] += gain[i];
            }
        }
        UpdateInfo();
    }

    public void GainHealth(int amount)
    {
        maxHealth += amount;
        health += amount;
        Display(amount, HealthSprite);
        UpdateInfo();
    }

    public void GainBlock(int amount)
    {
        if (PlayerScript.Item[31] && amount >= 15)
        {
            if (PlayerScript.Item[33])
                amount += 6;
            else amount += 3;
        }
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
        Display(amount, RestoreSprite);
        if (health > maxHealth)
            health = maxHealth;
        UpdateInfo();
    }

    public void GainEnergy(int amount)
    {
        if (PlayerScript.Item[32] && amount >= 3)
        {
            if (PlayerScript.Item[33])
                amount += 2;
            else amount += 1;
        }
        energy += amount;
        if (PlayerScript.Item[34])
        {
            drink += amount;
            while (drink >= 23)
            {
                drink -= 23;
                EquipmentCooldown(1);
            }
        }
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
        manaSpentTurn += amount;
        totalManaSpent += amount;
        UpdateInfo();
    }

    void GainValor(int amount)
    {
        valor += amount;
        Display(amount, ValorSprite);
        if (PlayerScript.Item[41])
            GainBlock(amount * 3);
        UpdateInfo();
    }

    void SpendValor(int amount)
    {
        valor -= amount;
        spentValor += amount;
        UpdateInfo();
    }

    void GainBlossom(int amount)
    {
        blossom += amount;
        //Display(amount, ValorSprite);
        UpdateInfo();
    }

    void GainStrength(int amount)
    {
        effect[0] += amount;
        Display(amount, effectSprite[0]);
        if (PlayerScript.Item[48])
            GainBlock(amount * 4);
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
        while (effect[18] >= 9)
        {
            effect[18] -= 9;
            if (CombatScript.enemiesAlive > 0)
            {
                tempi = CombatScript.RandomEnemy();
                CombatScript.Effect(false, 11, true, tempi);
                CombatScript.Enemy[tempi].TakeDamage(lightningDamage);
                CombatScript.Enemy[tempi].GainSlow((8 + lightningDamage) / 18);
                lightningDamage += 2;
            }
            if (PlayerScript.Item[45])
            {
                GainDexterity(1);
                GainBlock(9);
            }
        }
        //Display(amount, effectSprite[18]);
        UpdateInfo();
    }

    public void GainPoison(int amount)
    {
        effect[20] += amount;
        Display(amount, effectSprite[20]);
        UpdateInfo();
    }

    public void WeaponHovered(bool equipment, int equipmentOrder)
    {
        if (!equipment)
        {
            TheWeapon.SetActive(true);
            TheWeaponIcon.sprite = PlayerScript.WeaponSprite;
            TheWeaponCost.text = energyCost.ToString("");
            TheWeaponName.text = PlayerScript.weaponName;
            TheWeaponEffect.text = "Deal " + WeaponDamage().ToString("") + " Damage";
        }
        else
        {
            TheWeapon.SetActive(true);
            TheWeaponIcon.sprite = ELibrary.Equipments[eqID[equipmentOrder]].EquipmentSprite;
            TheWeaponCost.text = eqEnergyCost[equipmentOrder].ToString("");
            TheWeaponName.text = ELibrary.Equipments[eqID[equipmentOrder]].EquipmentName;
            switch (eqID[equipmentOrder])
            {
                case 0:
                    TheWeaponEffect.text = "Gain " + BucklerBlock().ToString("0") + " Block";
                    break;
                case 1:
                    TheWeaponEffect.text = "Deal " + DaggerDamage().ToString("0") + " Damage";
                    break;
                case 2:
                    TheWeaponEffect.text = "Draw 1 Card";
                    break;
                case 3:
                    TheWeaponEffect.text = "Gain 1 Mana";
                    break;
                case 4:
                    TheWeaponEffect.text = "Deal " + MaceDamage().ToString("0") + " Damage\nApply 1 Slow";
                    break;
            }
        }
    }

    public void Unhovered()
    {
        TheWeapon.SetActive(false);
    }

    public void UseWeapon()
    {
        CombatScript.Effect(false, 0, false, CombatScript.targetedEnemy);
        CombatScript.Enemy[CombatScript.targetedEnemy].TakeDamage(WeaponDamage());
        OnHit(true);
        SpendEnergy(energyCost);
        UpdateInfo();
    }

    public void UseEquipment(int order)
    {
        SpendEnergy(eqEnergyCost[order]);
        uses[order]--;
        switch (eqID[order])
        {
            case 0:
                Buckler();
                break;
            case 1:
                Dagger();
                break;
            case 2:
                Backpack();
                break;
            case 3:
                Shovel();
                break;
            case 4:
                Mace();
                break;
        }
        UpdateInfo();
    }

    void OnHit(bool weapon = false)
    {
        attacks++;
        if (effect[5] > 0 && weapon)
        {
            CombatScript.Enemy[CombatScript.targetedEnemy].GainDaze(effect[5]);
            CombatScript.Enemy[CombatScript.targetedEnemy].GainSlow(effect[5]);
        }
        if (effect[15] > 0)
            CombatScript.Enemy[CombatScript.targetedEnemy].GainBleed(effect[15]);
        if (effect[17] > 0 && weapon)
            GainStormCharge(effect[17]);
        if (effect[27] > 0)
        {
            if (weapon)
                GainBlock(effect[27] * 3);
            else GainBlock(effect[27]);
        }
        if (PlayerScript.Item[9] && attacks % 6 == 0)
            GainStrength(1);
        if (PlayerScript.Item[10] && attacks % 6 == 0)
            GainResistance(1);
        if (PlayerScript.Item[11] && attacks % 3 == 0)
            GainBlock(4);
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
                value *= 11;
                value /= 13 + PlayerScript.CurseValue[0];
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
                value *= 11;
                value /= 13 + PlayerScript.CurseValue[4];
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
        if (PlayerScript.Item[36])
            amount--;
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
        LoseHealth(amount);
        if (PlayerScript.CurseValue[3] > 0 && amount > 0)
            LoseSanity(Random.Range(PlayerScript.CurseValue[3] * 2, PlayerScript.CurseValue[3] * 3 + 1));
    }

    public void TakeMagicDamage(int amount)
    {
        Display(amount, MagicDamageSprite);
        LoseHealth(amount);
    }

    void LoseHealth(int amount)
    {
        health -= amount;
        GainWrath(amount, 28);
        if (effect[26] > 0 && amount > 0)
            GainShield(amount * effect[26]);
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
        if (PlayerScript.Item[37] && level >= 2)
            GainBlock(10);
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
                if (which < Library.waterCards)
                    UseWaterAbility(which, level);
                else
                {
                    which -= Library.waterCards;
                    if (which < Library.natureCards)
                        UseNatureAbility(which, level);
                    else
                    {
                        which -= Library.natureCards;
                        UseBloodAbility(which, level);
                    }
                }
            }
        }
        GainCombo();
    }

    void GainCombo()
    {
        combo++;
        if (combo % 5 == 0)
        {
            if (effect[19] > 0)
                GainBlock(effect[19]);
            if (PlayerScript.Item[44])
            {
                GainEnergy(4);
                Cards.Draw(1);
            }
        }
        UpdateInfo();
    }

    public void GainWrath(int amount, int multiplyer)
    {
        wrathCharges += amount * multiplyer;
        while (wrathCharges >= 50)
        {
            wrathCharges -= 50;
            wrath++;
        }
        UpdateInfo();
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
            case 35:
                HolyFire(level);
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
            case 30:
                FrozenTouch(level);
                break;
            case 31:
                EyeOfTheStorm(level);
                break;
            case 32:
                Acclimation(level);
                break;
            case 33:
                Torrent(level);
                break;
            case 34:
                Riptide(level, true);
                break;
        }
    }

    public void UseNatureAbility(int which, int level)
    {
        switch (which)
        {
            case 0:
                SapMagic(level);
                break;
            case 1:
                Innervate(level);
                break;
            case 2:
                Barkskin(level);
                break;
            case 3:
                CycleOfLife(level);
                break;
            case 4:
                ForceOfNature(level);
                break;
            case 5:
                Meditate(level);
                break;
            case 6:
                CleanCut(level);
                break;
            case 7:
                WildGrowth(level);
                break;
            case 8:
                EntanglingRoots(level);
                break;
            case 9:
                Deflect(level);
                break;
            case 10:
                EarthenMight(level);
                break;
            case 11:
                ManaWell();
                break;
            case 12:
                EarthenHide(level);
                break;
            case 13:
                PowerOfTheWild(level);
                break;
            case 14:
                PinDown(level);
                break;
            case 15:
                Rejuvenation(level);
                break;
            case 16:
                Volley(level);
                break;
            case 17:
                Savagery(level);
                break;
        }
    }

    public void UseBloodAbility(int which, int level)
    {
        switch (which)
        {
            case 0:
                Rampage(level);
                break;
            case 1:
                Drain(level);
                break;
            case 2:
                Carve(level);
                break;
            case 3:
                Cleave(level);
                break;
            case 4:
                SiphonStrength(level);
                break;
            case 5:
                Juggernaut(level);
                break;
            case 6:
                Execute(level);
                break;
            case 7:
                Reform(level);
                break;
            case 8:
                SiphonLife(level);
                break;
            case 9:
                BloodBoil(level);
                break;
            case 10:
                BattleStance(level);
                break;
            case 11:
                Enrage(level);
                break;
            case 12:
                Bloodletting(level);
                break;
            case 13:
                Onslaught(level);
                break;
            case 14:
                SeeingRed(level);
                break;
            case 15:
                Decimate(level);
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
                        return "Gain " + EmpowerStrength(level).ToString("") + " Strength\n& " + EmpowerEnergy(level).ToString("") + " Energy\nDestroy";
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
                        if (valor >= DecisiveStrikeValorReq(level))
                            return "Deal " + DecisiveStrikeDamage(level).ToString("") + " Damage\nGain " + DecisiveStrikeStrength(level).ToString("") + " Strength\nSpend " + DecisiveStrikeValorReq(level).ToString("") + " Valor";
                        else return "Deal " + DecisiveStrikeDamage(level).ToString("") + " Damage\nGain " + DecisiveStrikeValor(level).ToString("") + " Valor\n(" + valor.ToString("") + "/" + DecisiveStrikeValorReq(level).ToString("") + ") Valor";
                    case 10:
                        if (valor >= DecisiveStrikeValorReq(level))
                            return "Gain " + BulwarkOfLightBlock(level).ToString("") + " Block\nGain " + DecisiveStrikeStrength(level).ToString("") + " Resistance\nSpend " + DecisiveStrikeValorReq(level).ToString("") + " Valor";
                        else return "Gain " + BulwarkOfLightBlock(level).ToString("") + " Block\nGain " + DecisiveStrikeValor(level).ToString("") + " Valor\n(" + valor.ToString("") + "/" + DecisiveStrikeValorReq(level).ToString("") + ") Valor";
                    case 11:
                        return "Gain " + GoldenAegisBlock(level).ToString("") + " Block\nApply " + GoldenAegisSlow(level).ToString("") + " Slow\nto All Enemies";
                    case 12:
                        return "Gain " + ShieldWallBlock(level).ToString("") + " Block";
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
                        if (valor >= HolyBoltValorReq(level))
                            return "Deal " + HolyBoltDamage(level).ToString("") + " Damage Twice\nSpend " + HolyBoltValorReq(level).ToString("") + " Valor";
                        else return "Deal " + HolyBoltDamage(level).ToString("") + " Damage\nGain 1 Valor\n(" + valor.ToString("") + "/" + HolyBoltValorReq(level).ToString("") + ") Valor";
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
                        return "Gain " + HeavyArmorBlock(level).ToString("") + " Block\n& " + HeavyArmorArmor(level).ToString("") + " Armor\nDestroy";
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
                        return "Deal " + RighteousHammerDamage(level).ToString("") + " Damage\nApply " + RighteousHammerDaze(level).ToString("") + " Daze\n& " + RighteousHammerSlow(level).ToString("") + " Slow";
                    case 31:
                        return "Gain " + LightsChosenStrength(level).ToString("") + " Strength\n" + LightsChosenResistance(level).ToString("") + " Resistance\n& " + LightsChosenDexterity(level).ToString("") + " Dexterity\nDestroy";
                    case 32:
                        return "Deal " + PenanceDamage(level).ToString("") + " Damage\nGain " + PenanceBlock(level).ToString("") + " Block";
                    case 33:
                        return "Gain " + GuardianAngelResistance(level).ToString("") + " Resistance\n" + GuardianAngelArmor(level).ToString("") + " Armor\nBlock gained from Armor is affected by Resistance\nDestroy";
                    case 34:
                        if (CombatScript.Enemy[CombatScript.targetedEnemy].IntentToAttack())
                            return "Deal " + VengeanceDamage(level).ToString("") + " Damage, Gain " + VengeanceStrength(level).ToString("") + " Strength\n& " + VengeanceEnergy(level).ToString("") + " Energy";
                        else return "Deal " + VengeanceDamage(level).ToString("") + " Damage";
                    case 35:
                        return "Deal " + HolyFireDamage(level).ToString("") + " Damage";
                }
            }
            else
            {
                which -= Library.lightCards;
                if (which < Library.waterCards)
                {
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
                            return "Gain " + PreparationBlock(level).ToString("") + " Block\n& 1 permanent Combo,\nEvery 5x Combo reached, gain " + PreparationStacks(level).ToString("") + " Block\nDestroy";
                        case 19:
                            return "Deal " + StaggeringBlowDamage(level).ToString("") + " Damage\nreduce Targets\nTenacity by 1\n& Apply " + StaggeringBlowSlow(level).ToString("") + " Slow\nDestroy";
                        case 20:
                            return "Deal " + DredgeLineDamage(level).ToString("") + " Damage\nApply " + DredgeLineSlow(level).ToString("") + " Slow\nGain " + DredgeLineEnergy(level).ToString("") + " Energy";
                        case 21:
                            return "Deal " + ShredDamage(level).ToString("") + " Damage\nBreak up to " + ShredBreak(level).ToString("") + " Shield\nGain that much Block";
                        case 22:
                            return "Gain " + StrengthOfTheDepthsStrength(level).ToString("") + " Strength\n& " + StrengthOfTheDepthsBlock(level).ToString("") + " Block";
                        case 23:
                            return "Every Weapon Attack Gives " + TridentOfStormsStacks(level).ToString("") + " Storm Charges\nDestroy";
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
                        case 30:
                            return "Apply " + FrozenTouchBleed(level).ToString("") + " Bleed\n& " + FrozenTouchSlow(level).ToString("") + " Slow";
                        case 31:
                            if (combo <= 1)
                                return "Deal " + EyeOfTheStormDamage(level).ToString("") + " Damage\n(" + combo.ToString("") + "/" + EyeOfTheStormCombo(level).ToString("") + " Combo)";
                            else if (combo < EyeOfTheStormCombo(level))
                                return "Deal " + EyeOfTheStormDamage(level).ToString("") + " Damage\nGain " + EyeOfTheStormCharges(level).ToString("") + " Storm Charges\n(" + combo.ToString("") + "/" + EyeOfTheStormCombo(level).ToString("") + " Combo)";
                            else return "Deal " + EyeOfTheStormDamage(level).ToString("") + " Damage\nGain " + EyeOfTheStormCharges(level).ToString("") + " Storm Charges";
                        case 32:
                            return "Gain " + AcclimationBlock(level).ToString("") + " Block\nApply " + AcclimationSlow(level).ToString("") + " Slow";
                        case 33:
                            return "Deal " + TorrentDamage(level).ToString("") + " Damage\nGain " + TorrentCharges(level).ToString("") + " Storm Charges";
                        case 34:
                            return "Deal " + RiptideDamage().ToString("") + " Damage\n& Apply 2 Slow\nto all Enemies\nRepeat " + RiptideRecasts(level).ToString("") + " Time/s next Turn";
                    }
                }
                else
                {
                    which -= Library.waterCards;
                    if (which < Library.natureCards)
                    {
                        switch (which)
                        {
                            case 0:
                                return "Deal " + SapMagicDamage(level).ToString("") + " Damage\nGain 1 Mana next Turn";
                            case 1:
                                return "Gain " + InnervateMana(level).ToString("") + " Mana\n& Draw 1 Card";
                            case 2:
                                return "Gain " + BarkskinBlock(level).ToString("") + " Block";
                            case 3:
                                if (level == 0)
                                    return "Draw 1 Card\nGain " + CycleOfLifeBlock(level, 0).ToString("") + " Block";
                                return "Draw 2 Cards\nGain " + CycleOfLifeBlock(level, 1).ToString("") + " Block";
                            case 4:
                                return "Deal " + ForceOfNatureDamage(level).ToString("") + " Damage\nApply " + ForceOfNatureSlow(level).ToString("") + " Slow";
                            case 5:
                                if (level == 0)
                                    return "Gain " + MeditateBlock(level).ToString("") + " Block\nGain " + MeditateMana(level).ToString("") + " Mana\n& Draw 1 Card\nNext Turn";
                                return "Gain " + MeditateBlock(level).ToString("") + " Block\nGain " + MeditateMana(level).ToString("") + " Mana\n& Draw 2 Cards\nNext Turn";
                            case 6:
                                return "Deal " + CleanCutDamage(level).ToString("") + " Damage";
                            case 7:
                                return "Gain " + WildGrowthBlock(level).ToString("") + " Block\n& 1 Blossom";
                            case 8:
                                if (EntanglingRootsTargets(level) == 0)
                                    return "Deal " + EntanglingRootsDamage(level).ToString("") + " Damage\nApply " + EntanglingRootsSlow(level).ToString("") + " Slow\nto all Enemies";
                                else if (EntanglingRootsTargets(level) == 1)
                                    return "Deal " + EntanglingRootsDamage(level).ToString("") + " Damage\nApply " + EntanglingRootsSlow(level).ToString("") + " Slow\nto all Enemies\n & at random 1 Time";
                                else return "Deal " + EntanglingRootsDamage(level).ToString("") + " Damage\nApply " + EntanglingRootsSlow(level).ToString("") + " Slow\nto all Enemies\n & at random " + EntanglingRootsTargets(level).ToString("") + " Times";
                            case 9:
                                return "Gain " + DeflectBlock(level).ToString("") + " Block\nGain " + DeflectStacks(level).ToString("") + " Stored Block & Deal " + DeflectStacks(level).ToString("") + " Damage when being attacked this Turn";
                            case 10:
                                if (blossom < 5)
                                    return "Gain " + EarthenMightBlock(level).ToString("") + " Block\n(" + blossom.ToString("") + "/5 Blossom)";
                                else return "Gain " + EarthenMightBlock(level).ToString("") + " Block\nGain 1 Strength";
                            case 11:
                                return "Gain 1 Max Mana. Destroy";
                            case 12:
                                if (blossom < EarthenHideReq(level))
                                    return "Gain " + EarthenHideBlock(level).ToString("") + " Block\n(" + blossom.ToString("") + "/" + EarthenHideReq(level).ToString("") + " Blossom)";
                                else return "Gain " + EarthenHideBlock(level).ToString("") + " Block\nGain 1 Mana";
                            case 13:
                                return "Deal " + PowerOfTheWildDamage(level).ToString("") + " Damage\nGain " + PowerOfTheWildBlossom(level).ToString("") + " Blossom";
                            case 14:
                                return "Deal " + PinDownDamage(level).ToString("") + " Damage\nApply " + PinDownSlow(level).ToString("") + " Slow";
                            case 15:
                                return "Restore " + RejuvenationRestore(level).ToString("") + " Health. Destroy";
                            case 16:
                                return "Deal " + VolleyDamage(level).ToString("") + " Damage\nto random Enemy " + VolleyTargets(level).ToString("") + " Times";
                            case 17:
                                if (blossom < SavageryReq(level))
                                    return "Gain " + SavageryStrength(level, false).ToString("") + " Strength\n& " + SavageryBlock(level) + " Block\n(" + blossom.ToString("") + "/" + SavageryReq(level).ToString("") + " Blossom)";
                                else return "Gain " + SavageryStrength(level, true).ToString("") + " Strength\n& " + SavageryBlock(level) + " Block\nincrease Blossom required by " + SavageryBlossomInc(level).ToString("");
                        }
                    }
                    else
                    {
                        which -= Library.natureCards;
                        switch (which)
                        {
                            case 0:
                                return "Deal " + RampageDamage(level).ToString("") + " Damage\nIncrease Rampage Damage by " + RampageInc(level).ToString("");
                            case 1:
                                return "Deal " + DrainDamage(level).ToString("") + " Damage\nRestore " + DrainRestore(level).ToString("") + " Health\nDestroy";
                            case 2:
                                return "Deal " + CarveDamage(level).ToString("") + " Damage\nGain " + CarveBlock(level).ToString("") + " Block";
                            case 3:
                                return "Deal " + CleaveDamage(level).ToString("") + " Damage\nto all Enemies";
                            case 4:
                                return "Deal " + SiphonStrengthDamage(level).ToString("") + " Damage\nGain " + SiphonStrengthGain(level).ToString("") + " Strength";
                            case 5:
                                return "Gain " + JuggernautBlock(level).ToString("") + " Block\nGain Shield equal to Health Lost this Combat\nDestroy";
                            case 6:
                                return "Deal " + ExecuteDamage(level).ToString("") + " Damage";
                            case 7:
                                return "Lose 2 Health, Gain " + ReformBlock(level).ToString("") + " Block\n& " + ReformShield(level).ToString("") + " Shield";
                            case 8:
                                return "Deal " + SiphonLifeDamage(level).ToString("") + " Damage\nIncrease Max Health by " + ThickSkinHealth(level).ToString("") + " permamently\nDestroy";
                            case 9:
                                return "Lose 3 Health, Gain " + BloodBoilBonus(level, 0).ToString("") + " Strength\n" + BloodBoilBonus(level, 1).ToString("") + " Dexterity\n&" + BloodBoilBonus(level, 3).ToString("") + " Energy\nDestroy";
                            case 10:
                                return "Gain " + BattleStanceBlock(level).ToString("") + " Block\nAttacks give " + BattleStanceStacks(level).ToString("") + " Block this Combat, Tripled for Weapon attacks\nDestroy";
                            case 11:
                                if (EnrageBuff(level, EnrageWrath(level)) > 0)
                                    return "Gain " + EnrageWrath(level).ToString("") + " Wrath\nGain " + EnrageBuff(level, EnrageWrath(level)).ToString("") + " Strength & Energy";
                                else return "Gain " + EnrageWrath(level).ToString("") + " Wrath";
                            case 12:
                                return "Lose " + BloodlettingLoss(level).ToString("") + " Health\nGain " + BloodlettingEnergy(level).ToString("") + " Energy\n1 Mana\n& Draw 1 Card";
                            case 13:
                                return "Deal " + OnslaughtDamage(level).ToString("") + " Damage";
                            case 14:
                                if (PlayerScript.CursesCount > 0)
                                    return "Gain 1 Max Mana\n& " + SeeingRedStrength(level).ToString("") + " Strength\nLose " + SeeingRedSanity(level).ToString("") + " Sanity\nDestroy";
                                else return "Gain 1 Max Mana\nLose " + SeeingRedSanity(level).ToString("") + " Sanity\nDestroy";
                            case 15:
                                if (DecimateBuff(level) > 0)
                                    return "Deal " + DecimateDamage(level).ToString("") + " Damage\nRestore " + DecimateBuff(level).ToString("") + " Health";
                                else return "Deal " + DecimateDamage(level).ToString("") + " Damage";
                        }
                    }
                }
            }
        }
        return "";
    }

    // EQUIPMENT
    void Buckler() // 0
    {
        GainBlock(BucklerBlock());
    }

    int BucklerBlock()
    {
        tempi = 6 + effect[1];
        return BlockGainedModifier(tempi);
    }

    void Dagger() // 1
    {
        CombatScript.Effect(false, 1, false, CombatScript.targetedEnemy);
        CombatScript.Enemy[CombatScript.targetedEnemy].TakeDamage(DaggerDamage());
        OnHit();
    }

    int DaggerDamage()
    {
        tempi = 4 + effect[0];
        return DamageDealtModifier(tempi);
    }

    void Backpack() // 2
    {
        Cards.Draw(1);
    }

    void Shovel() // 3
    {
        GainMana(1);
    }

    void Mace() // 4
    {
        CombatScript.Effect(false, 0, false, CombatScript.targetedEnemy);
        CombatScript.Enemy[CombatScript.targetedEnemy].TakeDamage(MaceDamage());
        OnHit();
        CombatScript.Enemy[CombatScript.targetedEnemy].GainSlow(1);
    }

    int MaceDamage()
    {
        tempi = 6 + effect[0];
        return DamageDealtModifier(tempi);
    }

    // ABILITIES
    // NEUTRAL
    void Strike(int level) // ID N 0
    {
        CombatScript.Effect(false, 1, false, CombatScript.targetedEnemy);
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
        CombatScript.Effect(false, 2, true, CombatScript.targetedEnemy);
        CombatScript.Enemy[CombatScript.targetedEnemy].BreakShield(SpearThrustBreak(level));
        CombatScript.Enemy[CombatScript.targetedEnemy].TakeDamage(SpearThrustDamage(level));
        OnHit();
    }

    int SpearThrustBreak(int level)
    {
        tempi = 8;
        tempi += 5 * level;
        return tempi;
    }

    int SpearThrustDamage(int level)
    {
        tempi = 10 + effect[0];
        tempi += 2 * level;
        return DamageDealtModifier(tempi);
    }

    void Judgement(int level) // ID L 1
    {
        CombatScript.Effect(false, 1, false, CombatScript.targetedEnemy);
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
        CombatScript.Effect(false, 3, true, CombatScript.targetedEnemy);
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
        CombatScript.Effect(false, 2, false, CombatScript.targetedEnemy);
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
        CombatScript.Effect(false, 0, false, CombatScript.targetedEnemy);
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
        CombatScript.Effect(false, 1, false, CombatScript.targetedEnemy);
        CombatScript.Enemy[CombatScript.targetedEnemy].TakeDamage(DecisiveStrikeDamage(level));
        OnHit();
        if (valor >= DecisiveStrikeValorReq(level))
        {
            GainStrength(DecisiveStrikeStrength(level));
            SpendValor(DecisiveStrikeValorReq(level));
        }
        else GainValor(DecisiveStrikeValor(level));
    }

    int DecisiveStrikeDamage(int level)
    {
        tempi = 12 + effect[0];
        tempi += 2 * level;
        return DamageDealtModifier(tempi);
    }

    int DecisiveStrikeValorReq(int level)
    {
        tempi = 4;
        tempi -= level;
        tempi += (level / 2) * 3;
        return tempi;
    }

    int DecisiveStrikeStrength(int level)
    {
        tempi = 1;
        tempi += level / 2;
        return tempi;
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
        if (valor >= DecisiveStrikeValorReq(level))
        {
            GainResistance(DecisiveStrikeStrength(level));
            SpendValor(DecisiveStrikeValorReq(level));
        }
        else GainValor(DecisiveStrikeValor(level));
    }

    int BulwarkOfLightBlock(int level)
    {
        tempi = 11 + effect[1];
        tempi += 2 * level;
        return BlockGainedModifier(tempi);
    }

    void GoldenAegis(int level) // ID L 11
    {
        GainBlock(GoldenAegisBlock(level));
        for (int i = 0; i < CombatScript.enemyAlive.Length; i++)
        {
            if (CombatScript.enemyAlive[i])
            {
                CombatScript.Effect(false, 4, true, i);
                CombatScript.Enemy[i].GainSlow(GoldenAegisSlow(level));
            }
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
    }

    int ShieldWallBlock(int level)
    {
        tempi = 10 + 2 * effect[1];
        tempi += (2 + effect[1]) * level;
        return BlockGainedModifier(tempi);
    }

    void ShieldGlare(int level) // ID L 13
    {
        GainBlock(ShieldGlareBlock(level));
        CombatScript.Effect(false, 5, true, CombatScript.targetedEnemy);
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
        CombatScript.Effect(false, 6, true, CombatScript.targetedEnemy);
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
        CombatScript.Effect(false, 5, true, CombatScript.targetedEnemy);
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
                CombatScript.Effect(false, 4, true, CombatScript.targetedEnemy);
                CombatScript.Enemy[i].TakeDamage(ConsecrationDamage(level));
                CombatScript.Enemy[i].GainSlow(ConsecrationSlow(level));
            }
        }
        OnHit();
        GainValor(ConsecrationValor(level));
    }

    int ConsecrationDamage(int level)
    {
        tempi = 16 + effect[0];
        tempi += 6 * level;
        return DamageDealtModifier(tempi);
    }

    int ConsecrationSlow(int level)
    {
        tempi = 2;
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
        CombatScript.Effect(false, 4, true, CombatScript.targetedEnemy);
        CombatScript.Enemy[CombatScript.targetedEnemy].TakeDamage(ChastiseDamage(level));
        OnHit();
        CombatScript.Enemy[CombatScript.targetedEnemy].GainDaze(ChastiseDaze(level));
        CombatScript.Enemy[CombatScript.targetedEnemy].GainSlow(ChastiseSlow(level));
    }

    int ChastiseDamage(int level)
    {
        tempi = 11 + effect[0];
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
        tempi = 4;
        tempi += 2 * level;
        return tempi;
    }

    void HolyBolt(int level) // ID L 19
    {
        CombatScript.Effect(false, 7, true, CombatScript.targetedEnemy);
        CombatScript.Enemy[CombatScript.targetedEnemy].TakeDamage(HolyBoltDamage(level));
        OnHit();
        if (valor >= HolyBoltValorReq(level))
        {
            CombatScript.Enemy[CombatScript.targetedEnemy].TakeDamage(HolyBoltDamage(level));
            OnHit();
            SpendValor(HolyBoltValorReq(level));
        }
        else GainValor(1);
    }

    int HolyBoltDamage(int level)
    {
        tempi = 6 + effect[0];
        tempi += 3 * level;
        tempi -= (level / 2) * 2;
        return DamageDealtModifier(tempi);
    }

    int HolyBoltValorReq(int level)
    {
        tempi = 5;
        tempi -= level / 2;
        return tempi;
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
        tempi = 3;
        tempi += 2 * level;
        return tempi;
    }

    void CounterAttack(int level) // ID L 21
    {
        CombatScript.Effect(false, 1, false, CombatScript.targetedEnemy);
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
        tempi = 3;
        tempi += 1 * level;
        return tempi;
    }

    int SurgeOfLightEnergy(int level)
    {
        tempi = 1;
        tempi += 3 * level;
        return tempi;
    }

    void PatientStrike(int level) // ID L 23
    {
        CombatScript.Effect(false, 0, false, CombatScript.targetedEnemy);
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
        CombatScript.Effect(false, 0, false, CombatScript.targetedEnemy);
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
        GainArmor(HeavyArmorArmor(level));
    }

    int HeavyArmorBlock(int level)
    {
        tempi = 12 + effect[1];
        return BlockGainedModifier(tempi);
    }

    int HeavyArmorArmor(int level)
    {
        tempi = 2 + level;
        return tempi;
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
                {
                    CombatScript.Effect(false, 4, true, i);
                    CombatScript.Enemy[i].GainSlow(ALightInTheDarknessSlow(level));
                }
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
        CombatScript.Effect(false, 0, false, CombatScript.targetedEnemy);
        CombatScript.Enemy[CombatScript.targetedEnemy].TakeDamage(RighteousHammerDamage(level));
        OnHit();
        CombatScript.Enemy[CombatScript.targetedEnemy].GainDaze(RighteousHammerDaze(level));
        CombatScript.Enemy[CombatScript.targetedEnemy].GainSlow(RighteousHammerSlow(level));
    }

    int RighteousHammerDamage(int level)
    {
        tempi = 7 + effect[0];
        tempi += 1 * level;
        tempi += level / 2;
        return DamageDealtModifier(tempi);
    }

    int RighteousHammerDaze(int level)
    {
        tempi = RighteousHammerDamage(level) / 2;
        return tempi;
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
        tempi += (1 + level) /2;
        return tempi;
    }

    int LightsChosenResistance(int level)
    {
        tempi = 1;
        tempi -= level / 2;
        return tempi;
    }

    int LightsChosenDexterity(int level)
    {
        tempi = 1;
        tempi += level;
        return tempi;
    }

    void Penance(int level) // ID L 32
    {
        CombatScript.Effect(false, 6, true, CombatScript.targetedEnemy);
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
        CombatScript.Effect(false, 2, true, CombatScript.targetedEnemy);
        CombatScript.Enemy[CombatScript.targetedEnemy].TakeDamage(VengeanceDamage(level));
        OnHit();
        if (CombatScript.Enemy[CombatScript.targetedEnemy].IntentToAttack())
        {
            GainStrength(VengeanceStrength(level));
            GainEnergy(VengeanceEnergy(level));
        }
    }

    int VengeanceDamage(int level)
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

    void HolyFire(int level)
    {
        CombatScript.Effect(false, 5, true, CombatScript.targetedEnemy);
        CombatScript.Enemy[CombatScript.targetedEnemy].TakeDamage(HolyFireDamage(level));
        OnHit();
    }

    int HolyFireDamage(int level)
    {
        tempi = 11 + effect[0];
        tempi += 2 * level;
        if (level > 0)
            tempi += (3 + level) * (spentValor / 3);
        else tempi += spentValor;
        return DamageDealtModifier(tempi);
    }

    // WATER
    void QuickCut(int level) // ID W 0
    {
        CombatScript.Effect(false, 1, false, CombatScript.targetedEnemy);
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
        CombatScript.Effect(false, 1, false, CombatScript.targetedEnemy);
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
        CombatScript.Effect(false, 1, false, CombatScript.targetedEnemy);
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
        CombatScript.Effect(false, 8, true, CombatScript.targetedEnemy);
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
        CombatScript.Effect(false, 2, false, CombatScript.targetedEnemy);
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
        CombatScript.Effect(false, 2, false, CombatScript.targetedEnemy);
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
        tempi = 3 + effect[1];
        tempi += 3 * level;
        /*tempi += level / 2;
        tempi *= (Cards.CardsInHand - 1);
        tempi += effect[1];*/
        return BlockGainedModifier(tempi);
    }

    void Impale(int level) // ID W 9
    {
        CombatScript.Effect(false, 2, true, CombatScript.targetedEnemy);
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
        CombatScript.Effect(false, 1, false, CombatScript.targetedEnemy);
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
            CombatScript.Effect(false, 1, false, CombatScript.targetedEnemy);
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
        CombatScript.Effect(false, 1, false, CombatScript.targetedEnemy);
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
        for (int i = 0; i < combo * 2; i += 2 + i / 3)
        {
            CombatScript.Effect(false, 1, false, CombatScript.targetedEnemy);
        }
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
        permanentCombo++;
        effect[19] += PreparationStacks(level);
        Display(PreparationStacks(level), effectSprite[19]);
    }

    int PreparationBlock(int level)
    {
        tempi = 3 + effect[1];
        tempi += 2 * level;
        return BlockGainedModifier(tempi);
    }

    int PreparationStacks(int level)
    {
        tempi = 6;
        tempi += 2 * level;
        return tempi;
    }

    void StaggeringBlow(int level) // ID W 19
    {
        CombatScript.Effect(false, 2, false, CombatScript.targetedEnemy);
        CombatScript.Effect(false, 0, false, CombatScript.targetedEnemy);
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
        CombatScript.Effect(false, 0, false, CombatScript.targetedEnemy);
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
        CombatScript.Effect(false, 1, false, CombatScript.targetedEnemy);
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
        effect[17] += TridentOfStormsStacks(level);
        Display(TridentOfStormsStacks(level), effectSprite[17]);
    }

    int TridentOfStormsStacks(int level)
    {
        tempi = 2;
        tempi += level;
        return tempi;
    }

    void Conduit(int level) // ID W 24
    {
        GainBlock(ConduitBlock(level));
        GainStormCharge(ConduitCharges(level));
    }

    int ConduitBlock(int level)
    {
        tempi = 12 + effect[1];
        tempi += 2 * level;
        return BlockGainedModifier(tempi);
    }

    int ConduitCharges(int level)
    {
        tempi = 2;
        tempi += level;
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
        CombatScript.Effect(false, 0, false, CombatScript.targetedEnemy);
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
            CombatScript.Effect(false, 2, true, CombatScript.targetedEnemy);
            CombatScript.Enemy[CombatScript.targetedEnemy].TakeDamage(RendDamage(level));
            OnHit();
            GainBlock(RendBlock(level));
        }
    }

    int RendDamage(int level)
    {
        tempi = 7 + effect[0];
        tempi += level / 2;
        return DamageDealtModifier(tempi);
    }

    int RendBlock(int level)
    {
        tempi = 5 + effect[1];
        tempi += (level + 1) / 2;
        return BlockGainedModifier(tempi);
    }

    int RendAmount(int level)
    {
        tempi = 3 + level;
        tempi += effect[2] / 4;
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

    void FrozenTouch(int level) // ID W 30
    {
        CombatScript.Effect(false, 9, false, CombatScript.targetedEnemy);
        CombatScript.Enemy[CombatScript.targetedEnemy].GainBleed(FrozenTouchBleed(level));
        CombatScript.Enemy[CombatScript.targetedEnemy].GainSlow(FrozenTouchSlow(level));
    }

    int FrozenTouchBleed(int level)
    {
        tempi = 4;
        tempi += level;
        return tempi;
    }

    int FrozenTouchSlow(int level)
    {
        tempi = 2;
        tempi += level;
        return tempi;
    }

    void EyeOfTheStorm(int level) // ID W 31
    {
        CombatScript.Effect(false, 10, false, CombatScript.targetedEnemy);
        CombatScript.Enemy[CombatScript.targetedEnemy].TakeDamage(EyeOfTheStormDamage(level));
        OnHit();
        GainStormCharge(EyeOfTheStormCharges(level));
    }

    int EyeOfTheStormDamage(int level)
    {
        tempi = 4 + effect[0];
        tempi += 2 * level;
        return DamageDealtModifier(tempi);
    }

    int EyeOfTheStormCharges(int level)
    {
        tempi2 = combo / 2;
        if (combo >= EyeOfTheStormCombo(level))
        {
            tempi2 += 3;
            tempi2 += level / 2;
        }
        return tempi2;
    }

    int EyeOfTheStormCombo(int level)
    {
        tempi = 4;
        tempi -= (1 + level) / 2;
        return tempi;
    }

    void Acclimation(int level) // ID W 32
    {
        GainBlock(AcclimationBlock(level));
        CombatScript.Effect(false, 9, false, CombatScript.targetedEnemy);
        CombatScript.Enemy[CombatScript.targetedEnemy].GainSlow(AcclimationSlow(level));
    }

    int AcclimationBlock(int level)
    {
        tempi = 2 + effect[1];
        tempi += 4 * level;
        tempi -= (level / 2) * 3;
        tempi += 2 * CombatScript.Enemy[CombatScript.targetedEnemy].tenacity;
        return BlockGainedModifier(tempi);
    }

    int AcclimationSlow(int level)
    {
        tempi = 1;
        tempi += level / 2;
        return tempi;
    }

    void Torrent(int level) // ID W 33
    {
        CombatScript.Effect(false, 10, false, CombatScript.targetedEnemy);
        CombatScript.Enemy[CombatScript.targetedEnemy].TakeDamage(TorrentDamage(level));
        OnHit();
        GainStormCharge(TorrentCharges(level));
    }

    int TorrentDamage(int level)
    {
        tempi = 10 + effect[0];
        tempi += 3 * level;
        return DamageDealtModifier(tempi);
    }

    int TorrentCharges(int level)
    {
        tempi = 3;
        tempi += level;
        return tempi;
    }

    void Riptide(int level, bool initial) // ID W 34
    {
        for (int i = 0; i < CombatScript.enemyAlive.Length; i++)
        {
            if (CombatScript.enemyAlive[i])
            {
                CombatScript.Effect(false, 12, true, i);
                CombatScript.Enemy[i].TakeDamage(RiptideDamage());
                CombatScript.Enemy[i].GainSlow(2);
            }
        }
        OnHit();
        if (initial)
            effect[21] += RiptideRecasts(level);
    }

    int RiptideDamage()
    {
        tempi = 10 + effect[0];
        return DamageDealtModifier(tempi);
    }

    int RiptideRecasts(int level)
    {
        tempi = 1 + level;
        return tempi;
    }

    // NATURE
    void SapMagic(int level) // ID N 0
    {
        CombatScript.Effect(false, 15, true, CombatScript.targetedEnemy);
        CombatScript.Enemy[CombatScript.targetedEnemy].TakeDamage(SapMagicDamage(level));
        OnHit();
        effect[22]++;
    }

    int SapMagicDamage(int level)
    {
        tempi = 11 + effect[0];
        tempi += 4 * level;
        return DamageDealtModifier(tempi);
    }

    void Innervate(int level) // ID N 1
    {
        GainMana(InnervateMana(level));
        Cards.Draw(1);
    }

    int InnervateMana(int level)
    {
        tempi = 1;
        tempi += level;
        return tempi;
    }

    void Barkskin(int level) // ID N 2
    {
        GainBlock(BarkskinBlock(level));
    }

    int BarkskinBlock(int level)
    {
        tempi = 6 + effect[1];
        tempi += 2 * level;
        tempi += (3 + level) * manaSpentTurn;
        return BlockGainedModifier(tempi);
    }

    void CycleOfLife(int level) // ID N 3
    {
        Cards.Draw(CycleOfLifeDraw(level));
        GainBlock(CycleOfLifeBlock(level, 0));
    }

    int CycleOfLifeDraw(int level)
    {
        tempi = 1;
        tempi += (1 + level) / 2;
        return tempi;
    }

    int CycleOfLifeBlock(int level, int additionalCards)
    {
        tempi = 2;
        tempi += level / 2;
        tempi *= (Cards.CardsInHand + additionalCards);
        tempi += effect[1];
        return BlockGainedModifier(tempi);
    }

    void ForceOfNature(int level) // ID N 4
    {
        CombatScript.Effect(false, 13, true, CombatScript.targetedEnemy);
        CombatScript.Enemy[CombatScript.targetedEnemy].TakeDamage(ForceOfNatureDamage(level));
        OnHit();
        CombatScript.Enemy[CombatScript.targetedEnemy].GainSlow(ForceOfNatureSlow(level));
    }

    int ForceOfNatureDamage(int level)
    {
        tempi = 18 + effect[0];
        tempi += 4 * level;
        tempi += (3 + level) * (totalManaSpent / (4 + level));
        return DamageDealtModifier(tempi);
    }

    int ForceOfNatureSlow(int level)
    {
        tempi2 = 11 - level;
        tempi2 = ForceOfNatureDamage(level) / tempi2;
        return tempi2;
    }

    void Meditate(int level) // ID N 5
    {
        GainBlock(MeditateBlock(level));
        effect[22] += MeditateMana(level);
        effect[23] += MeditateDraw(level);
    }

    int MeditateBlock(int level)
    {
        tempi = 7 + effect[1];
        tempi += 2 * level;
        temp -= level / 2;
        return BlockGainedModifier(tempi);
    }

    int MeditateMana(int level)
    {
        tempi = 1;
        tempi += level / 2;
        return tempi;
    }

    int MeditateDraw(int level)
    {
        tempi = 1;
        tempi += (1 + level) / 2;
        return tempi;
    }

    void CleanCut(int level) // ID N 6
    {
        CombatScript.Effect(false, 1, false, CombatScript.targetedEnemy);
        CombatScript.Enemy[CombatScript.targetedEnemy].TakeDamage(CleanCutDamage(level));
        OnHit();
    }

    int CleanCutDamage(int level)
    {
        tempi = 11 + effect[0];
        tempi += 3 * level;
        if (CombatScript.Enemy[CombatScript.targetedEnemy].TotalBlock() <= 0)
            tempi *= 2;
        return DamageDealtModifier(tempi);
    }

    void WildGrowth(int level) // ID N 7
    {
        GainBlock(WildGrowthBlock(level));
        GainBlossom(1);
    }

    int WildGrowthBlock(int level)
    {
        tempi = 8 + effect[1];
        tempi += 4 * level;
        return BlockGainedModifier(tempi);
    }

    void EntanglingRoots(int level) // ID N 8
    {
        for (int i = 0; i < CombatScript.enemyAlive.Length; i++)
        {
            if (CombatScript.enemyAlive[i])
            {
                CombatScript.Effect(false, 14, true, i);
                CombatScript.Enemy[i].TakeDamage(EntanglingRootsDamage(level));
                CombatScript.Enemy[i].GainSlow(EntanglingRootsSlow(level));
            }
        }
        if (EntanglingRootsTargets(level) > 0)
        {
            for (int i = 0; i < EntanglingRootsTargets(level); i++)
            {
                if (CombatScript.enemiesAlive > 0)
                {
                    tempi2 = CombatScript.RandomEnemy();
                    CombatScript.Effect(false, 14, true, tempi2);
                    CombatScript.Enemy[tempi2].TakeDamage(EntanglingRootsDamage(level));
                    CombatScript.Enemy[tempi2].GainSlow(EntanglingRootsSlow(level));
                }
            }
        }
        OnHit();
    }

    int EntanglingRootsDamage(int level)
    {
        tempi = 14 + effect[0];
        tempi += 4 * level;
        tempi += (level / 2) * 2;
        return DamageDealtModifier(tempi);
    }

    int EntanglingRootsSlow(int level)
    {
        tempi = 2;
        tempi += (1 + level) / 2;
        return tempi;
    }

    int EntanglingRootsTargets(int level)
    {
        tempi2 = 8 - (level / 2);
        tempi2 = blossom / tempi2;
        return tempi2;
    }

    void Deflect(int level) // ID N 9
    {
        GainBlock(DeflectBlock(level));
        effect[24] += DeflectStacks(level);
    }

    int DeflectBlock(int level)
    {
        tempi = 11 + effect[1];
        tempi += 2 * level;
        return BlockGainedModifier(tempi);
    }

    int DeflectStacks(int level)
    {
        tempi = 2;
        tempi += 2 * level;
        return tempi;
    }

    void EarthenMight(int level) // ID N 10
    {
        GainBlock(EarthenMightBlock(level));
        if (blossom >= 5)
            GainStrength(1);
    }

    int EarthenMightBlock(int level)
    {
        tempi = 9 + effect[1];
        tempi += 3 * level;
        if (blossom >= 5)
            tempi += 4 + 2 * level;
        return BlockGainedModifier(tempi);
    }

    void ManaWell() // ID N 11
    {
        manaGain++;
    }

    void EarthenHide(int level) // ID N 12
    {
        GainBlock(EarthenHideBlock(level));
        if (blossom >= EarthenHideReq(level))
            GainMana(1);
    }

    int EarthenHideBlock(int level)
    {
        tempi = 20 + effect[1];
        tempi += 4 * level;
        return BlockGainedModifier(tempi);
    }

    int EarthenHideReq(int level)
    {
        tempi = 7 - level;
        return tempi;
    }

    void PowerOfTheWild(int level) // ID N 13
    {
        CombatScript.Effect(false, 1, false, CombatScript.targetedEnemy);
        CombatScript.Enemy[CombatScript.targetedEnemy].TakeDamage(PowerOfTheWildDamage(level));
        GainBlossom(PowerOfTheWildBlossom(level));
        OnHit();
    }

    int PowerOfTheWildDamage(int level)
    {
        tempi = 9 + effect[0];
        tempi += 3 * level;
        return DamageDealtModifier(tempi);
    }

    int PowerOfTheWildBlossom(int level)
    {
        tempi = PowerOfTheWildDamage(level) / 8;
        return tempi;
    }

    void PinDown(int level) // ID N 14
    {
        CombatScript.Effect(false, 7, false, CombatScript.targetedEnemy);
        CombatScript.Enemy[CombatScript.targetedEnemy].TakeDamage(PinDownDamage(level));
        CombatScript.Enemy[CombatScript.targetedEnemy].GainSlow(PinDownSlow(level));
        OnHit();
    }

    int PinDownDamage(int level)
    {
        tempi = 7 + effect[0];
        tempi += 2 * level;
        return DamageDealtModifier(tempi);
    }

    int PinDownSlow(int level)
    {
        tempi = PinDownDamage(level) / 4;
        return tempi;
    }

    void Rejuvenation(int level) // ID N 15
    {
        RestoreHealth(RejuvenationRestore(level));
    }

    int RejuvenationRestore(int level)
    {
        tempi = 6;
        tempi += 3 * level;
        return tempi;
    }

    void Volley(int level) // ID N 16
    {
        for (int i = 0; i < VolleyTargets(level); i++)
        {
            if (CombatScript.enemiesAlive > 0)
            {
                tempi2 = CombatScript.RandomEnemy();
                CombatScript.Effect(false, 7, false, tempi2);
                CombatScript.Enemy[tempi2].TakeDamage(VolleyDamage(level));
            }
        }
        OnHit();
    }

    int VolleyDamage(int level)
    {
        tempi = 6 + effect[0];
        return DamageDealtModifier(tempi);
    }

    int VolleyTargets(int level)
    {
        tempi = 3;
        tempi += level;
        return tempi;
    }

    void Savagery(int level) // ID N 17
    {
        if (blossom >= SavageryReq(level))
        {
            GainStrength(SavageryStrength(level, true));
            savageryIncrease += SavageryBlossomInc(level);
        }
        else GainStrength(SavageryStrength(level, false));
        GainBlock(SavageryBlock(level));
    }

    int SavageryStrength(int level, bool empowered)
    {
        tempi = 1;
        if (empowered)
        {
            tempi += 1;
            tempi += level / 2;
        }
        return tempi;
    }

    int SavageryBlock(int level)
    {
        tempi = 3 + effect[1];
        tempi += 3 * level;
        return BlockGainedModifier(tempi);
    }

    int SavageryReq(int level)
    {
        tempi = 4 + savageryIncrease;
        tempi -= level;
        tempi += 4 * (level / 2);
        return tempi;
    }

    int SavageryBlossomInc(int level)
    {
        tempi = 4;
        tempi -= level;
        tempi += 3 * (level / 2);
        return tempi;
    }

    // BLOOD
    void Rampage(int level) // ID B 0
    {
        CombatScript.Effect(false, 1, false, CombatScript.targetedEnemy);
        CombatScript.Enemy[CombatScript.targetedEnemy].TakeDamage(RampageDamage(level));
        OnHit();
        rampageIncrease += RampageInc(level);
    }

    int RampageDamage(int level)
    {
        tempi = 10 + effect[0];
        tempi += 2 * level;
        tempi += rampageIncrease;
        return DamageDealtModifier(tempi);
    }

    int RampageInc(int level)
    {
        tempi = 4;
        tempi += 3 * level;
        return tempi;
    }

    void Drain(int level) // ID B 1
    {
        CombatScript.Effect(false, 16, true, CombatScript.targetedEnemy);
        CombatScript.Enemy[CombatScript.targetedEnemy].TakeDamage(DrainDamage(level));
        OnHit();
        RestoreHealth(DrainRestore(level));
    }

    int DrainDamage(int level)
    {
        tempi = 12 + effect[0];
        tempi += 3 * level;
        return DamageDealtModifier(tempi);
    }

    int DrainRestore(int level)
    {
        tempi = 5;
        tempi += 4 * level;
        return tempi;
    }

    void Carve(int level) // ID B 2
    {
        CombatScript.Effect(false, 1, false, CombatScript.targetedEnemy);
        CombatScript.Effect(false, 1, false, CombatScript.targetedEnemy);
        CombatScript.Enemy[CombatScript.targetedEnemy].TakeDamage(CarveDamage(level));
        OnHit();
        GainBlock(CarveBlock(level));
    }

    int CarveDamage(int level)
    {
        tempi = 15 + effect[0];
        tempi += 5 * level;
        return DamageDealtModifier(tempi);
    }

    int CarveBlock(int level)
    {
        tempi = CarveDamage(level) + effect[1];
        return BlockGainedModifier(tempi);
    }

    void Cleave(int level) // ID B 3
    {
        for (int i = 0; i < CombatScript.enemyAlive.Length; i++)
        {
            if (CombatScript.enemyAlive[i])
            {
                CombatScript.Effect(false, 1, false, i);
                CombatScript.Enemy[i].TakeDamage(CleaveDamage(level));
            }
        }
        OnHit();
    }

    int CleaveDamage(int level)
    {
        tempi = 11 + effect[0];
        tempi += 4 * level;
        return DamageDealtModifier(tempi);
    }

    void SiphonStrength(int level) // ID B 4
    {
        CombatScript.Effect(false, 17, true, CombatScript.targetedEnemy);
        CombatScript.Enemy[CombatScript.targetedEnemy].TakeDamage(SiphonStrengthDamage(level));
        OnHit();
        GainStrength(SiphonStrengthGain(level));
    }

    int SiphonStrengthDamage(int level)
    {
        tempi = 17 + effect[0];
        tempi += 4 * level;
        tempi += 2 * (level / 2);
        return DamageDealtModifier(tempi);
    }

    int SiphonStrengthGain(int level)
    {
        tempi2 = 13;
        tempi2 -= (level + 1) / 2;
        tempi2 = SiphonStrengthDamage(level) / tempi2;
        return tempi2;
    }

    void Juggernaut(int level) // ID B 5
    {
        effect[26] += 1;
        Display(1, effectSprite[26]);
        GainBlock(JuggernautBlock(level));
    }

    int JuggernautBlock(int level)
    {
        tempi = 2 + effect[1];
        tempi += 3 * level;
        return BlockGainedModifier(tempi);
    }

    void Execute(int level) // ID B 6
    {
        CombatScript.Effect(false, 1, false, CombatScript.targetedEnemy);
        if (CombatScript.Enemy[CombatScript.targetedEnemy].HealthPercent() < ExecuteThreshhold(level))
            CombatScript.Effect(false, 1, false, CombatScript.targetedEnemy);
        CombatScript.Enemy[CombatScript.targetedEnemy].TakeDamage(ExecuteDamage(level));
        OnHit();
    }

    int ExecuteDamage(int level)
    {
        tempi = 13 + effect[0];
        tempi += 2 * level;
        if (CombatScript.Enemy[CombatScript.targetedEnemy].HealthPercent() < ExecuteThreshhold(level))
            tempi *= 2;
        return DamageDealtModifier(tempi);
    }

    float ExecuteThreshhold(int level)
    {
        temp = 0.4f;
        temp += 0.1f * level;
        return temp;
    }

    void Reform(int level) // ID B 7
    {
        LoseHealth(2);
        if (PlayerScript.Item[51])
            RestoreHealth(1);
        GainBlock(ReformBlock(level));
        GainShield(ReformShield(level));
    }

    int ReformBlock(int level)
    {
        tempi = 13 + effect[1];
        tempi += 4 * level;
        return BlockGainedModifier(tempi);
    }

    int ReformShield(int level)
    {
        tempi = 3;
        tempi += 2 * level;
        return tempi;
    }

    void SiphonLife(int level) // ID B 8
    {
        CombatScript.Effect(false, 16, true, CombatScript.targetedEnemy);
        CombatScript.Enemy[CombatScript.targetedEnemy].TakeDamage(SiphonLifeDamage(level));
        OnHit();
        GainHealth(ThickSkinHealth(level));
    }

    int SiphonLifeDamage(int level)
    {
        tempi2 = 9 - level;
        tempi = (maxHealth / tempi2) + effect[0];
        return DamageDealtModifier(tempi);
    }

    void BloodBoil(int level) // ID B 9
    {
        LoseHealth(3);
        if (PlayerScript.Item[51])
            RestoreHealth(1);
        GainStrength(BloodBoilBonus(level, 0));
        GainDexterity(BloodBoilBonus(level, 1));
        GainEnergy(BloodBoilBonus(level, 3));
    }

    int BloodBoilBonus(int level, int bonus)
    {
        tempi = 1;
        tempi += level;
        tempi += bonus;
        return tempi;
    }

    void BattleStance(int level) // ID B 10
    {
        GainBlock(BattleStanceBlock(level));
        effect[27] += BattleStanceStacks(level);
        Display(BattleStanceStacks(level), effectSprite[27]);
    }

    int BattleStanceBlock(int level)
    {
        tempi = 6 + effect[1];
        tempi += 2 * level;
        return BlockGainedModifier(tempi);
    }

    int BattleStanceStacks(int level)
    {
        tempi = 2;
        tempi += level;
        return tempi;
    }

    void Enrage(int level) // ID B 11
    {
        GainWrath(EnrageWrath(level), 50);
        if (EnrageBuff(level, 0) > 0)
        {
            GainStrength(EnrageBuff(level, 0));
            GainEnergy(EnrageBuff(level, 0));
        }
    }

    int EnrageWrath(int level)
    {
        tempi = 6;
        tempi += 3 * level;
        return tempi;
    }

    int EnrageReq(int level)
    {
        tempi2 = 26;
        tempi2 -= 2 * level;
        return tempi2;
    }

    int EnrageBuff(int level, int bonus = 0)
    {
        tempi3 = 0;
        for (int i = 1; i < 4 + level + extraWrathTick; i++)
        {
            if (wrath + bonus >= EnrageReq(level) * i)
                tempi3++;
        }
        return tempi3;
    }

    void Bloodletting(int level) // ID B 12
    {
        LoseHealth(BloodlettingLoss(level));
        if (PlayerScript.Item[51])
            RestoreHealth(1);
        GainEnergy(BloodlettingEnergy(level));
        GainMana(1);
        Cards.Draw(1);
    }

    int BloodlettingLoss(int level)
    {
        tempi = 5;
        tempi -= level;
        return tempi;
    }

    int BloodlettingEnergy(int level)
    {
        tempi = 3;
        tempi += level;
        return tempi;
    }

    void Onslaught(int level) // ID B 13
    {
        CombatScript.Effect(false, 1, false, CombatScript.targetedEnemy);
        CombatScript.Enemy[CombatScript.targetedEnemy].TakeDamage(OnslaughtDamage(level));
        OnHit();
        GainWrath(OnslaughtDamage(level), OnslaughtWrath(level));
    }

    int OnslaughtDamage(int level)
    {
        tempi = 13 + effect[0];
        tempi += 2 * level;
        return DamageDealtModifier(tempi);
    }

     int OnslaughtWrath(int level)
    {
        tempi = 6;
        tempi += 2 * level;
        return tempi;
    }

    void SeeingRed(int level) // ID B 14
    {
        manaGain++;
        if (PlayerScript.CursesCount > 0)
            GainStrength(SeeingRedStrength(level));
        LoseSanity(SeeingRedSanity(level));
    }

    int SeeingRedStrength(int level)
    {
        tempi = 1;
        tempi += level / 2;
        tempi *= PlayerScript.CursesCount;
        return tempi;
    }

    int SeeingRedSanity(int level)
    {
        tempi = 9;
        tempi -= 3 * ((level + 1) / 2);
        return tempi;
    }

    void Decimate(int level) // ID B 15
    {
        if (DecimateBuff(level) > 0)
            RestoreHealth(DecimateBuff(level));
        CombatScript.Effect(false, 1, false, CombatScript.targetedEnemy);
        CombatScript.Enemy[CombatScript.targetedEnemy].TakeDamage(DecimateDamage(level));
        OnHit();
    }

    int DecimateDamage(int level)
    {
        tempi = 12 + effect[0];
        tempi += 4 * level;
        tempi += (4 + level) * DecimateBuff(level);
        return DamageDealtModifier(tempi);
    }

    int DecimateBuff(int level)
    {
        tempi3 = 0;
        for (int i = 1; i < 4 + level + extraWrathTick; i++)
        {
            if (wrath >= DecimateReq(level) * i)
                tempi3++;
        }
        return tempi3;
    }

    int DecimateReq(int level)
    {
        tempi2 = 26;
        tempi2 -= 2 * level;
        return tempi2;
    }

    // checks
    public int TotalBlock()
    {
        tempi = shield + block;
        return tempi;
    }

    public void RingOfResistance()
    {
        if (PlayerScript.Item[33])
            GainResistance(2);
        else GainResistance(1);
        resistanceRing = false;
    }
}
