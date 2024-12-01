using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Combat : MonoBehaviour
{
    [Header("Scripts")]
    public PlayerCombat Player;
    public EnemyCombat[] Enemy;
    public BossLoot BossLootScript;
    public Story StoryScript;
    public Maps MapsScript;
    public Map MapScript;
    public SceneChange Fade;
    public AdventureResults AdventureScript;

    [Header("Stats")]
    public bool[] enemyAlive;
    public int targetedEnemy, turn, enemiesAlive, bossDefeated;
    public bool elite, boss;
    int whichEnemy;
    float temp;
    int tempi;

    [Header("UI")]
    public Button EndTurnButton;
    public TMPro.TextMeshProUGUI TurnCounter, EffectTooltip;
    public GameObject CombatScene, Hand, PlayerHUD, Maps, StoryScene, ResultsScene;
    public GameObject[] TargetedObject;
    // public string/image[] playerEffects, enemyEffects; mo¿e potem zamieniæ na premade tooltipy

    [Header("Effects")]
    public EffectsLibrary ELibrary;

    [Header("Loot")]
    public LootChoice LootEvent;
    public float mapDanger;

    public void Start()
    {
        //EffectTooltip.text = Player.Cards.Library.Cards[2].CardTooltip[0];
    }

    public void SetEnemy(int enemyID, int enemyLevel)
    {
        Enemy[0].Unit.SetActive(true);
        Enemy[0].SetUnit(enemyID, enemyLevel);
        enemyAlive[0] = true;
        targetedEnemy = 0;
        enemiesAlive = 1;
        Enemy[1].Unit.SetActive(false);
        Enemy[2].Unit.SetActive(false);

        ResetCombat();
    }

    public void Set2Enemies(int enemyID, int enemyID2, int enemyLevel)
    {
        Enemy[0].Unit.SetActive(true);
        Enemy[0].SetUnit(enemyID, enemyLevel);
        enemyAlive[0] = true;
        Enemy[1].Unit.SetActive(true);
        Enemy[1].SetUnit(enemyID2, enemyLevel);
        enemyAlive[1] = true;
        targetedEnemy = 0;
        enemiesAlive = 2;
        Enemy[2].Unit.SetActive(false);

        ResetCombat();
    }

    public void Set3Enemies(int enemyID, int enemyID2, int enemyID3, int enemyLevel)
    {
        Enemy[0].Unit.SetActive(true);
        Enemy[0].SetUnit(enemyID, enemyLevel);
        enemyAlive[0] = true;
        Enemy[1].Unit.SetActive(true);
        Enemy[1].SetUnit(enemyID2, enemyLevel);
        enemyAlive[1] = true;
        Enemy[2].Unit.SetActive(true);
        Enemy[2].SetUnit(enemyID3, enemyLevel);
        enemyAlive[2] = true;
        targetedEnemy = 0;
        enemiesAlive = 3;

        ResetCombat();
    }

    public void SetLordOfAgony()
    {
        Enemy[0].Unit.SetActive(true);
        Enemy[0].SetUnit(5, 2);
        enemyAlive[0] = true;
        Enemy[1].Unit.SetActive(true);
        Enemy[1].SetUnit(20, 0);
        enemyAlive[1] = true;
        Enemy[2].Unit.SetActive(true);
        Enemy[2].SetUnit(13, 0);
        enemyAlive[2] = true;
        targetedEnemy = 0;
        enemiesAlive = 3;

        ResetCombat();
    }

    public void SetTheBog()
    {
        Enemy[0].Unit.SetActive(true);
        Enemy[0].SetUnit(18, 4);
        enemyAlive[0] = true;
        Enemy[1].Unit.SetActive(true);
        Enemy[1].SetUnit(21, 0);
        enemyAlive[1] = true;
        Enemy[2].Unit.SetActive(true);
        Enemy[2].SetUnit(18, 4);
        enemyAlive[2] = true;
        targetedEnemy = 0;
        enemiesAlive = 1;

        ResetCombat();
    }

    void ResetCombat()
    {
        RemoveTargets();
        turn = 1;
        Player.Reset();
        TurnCounter.text = turn.ToString("");
        EndTurnButton.interactable = true;
        TargetedObject[targetedEnemy].SetActive(true);
    }

    void RemoveTargets()
    {
        for (int i = 0; i < 3; i++)
        {
            TargetedObject[i].SetActive(false);
        }
    }

    public void ChooseTarget(int target)
    {
        TargetedObject[targetedEnemy].SetActive(false);
        targetedEnemy = target;
        TargetedObject[targetedEnemy].SetActive(true);
    }

    public void EndTurn()
    {
        Player.EndTurn();

        for (int i = 0; i < Enemy.Length; i++)
        {
            if (enemyAlive[i])
            {
                Enemy[i].EndTurn();
            }
        }

        Invoke("EnemyTurns", 0.5f);
    }

    void EnemyTurns()
    {
        temp = 0.2f;
        whichEnemy = 0;
        for (int i = 0; i < Enemy.Length; i++)
        {
            if (enemyAlive[i])
            {
                Invoke("EnemyTurn", temp);
                temp += 0.4f;
            }
        }
        Invoke("StartTurn", temp);
    }

    void StartTurn()
    {
        turn++;
        EndTurnButton.interactable = true;
        TurnCounter.text = turn.ToString("");

        Player.StartTurn();
        for (int i = 0; i < Enemy.Length; i++)
        {
            if (enemyAlive[i])
            {
                Enemy[i].StartTurn();
            }
        }
    }

    void EnemyTurn()
    {
        while (!enemyAlive[whichEnemy])
        {
            whichEnemy++;
        }
        Enemy[whichEnemy].Move();
        whichEnemy++;
    }

    public void EnemyDefeated(int which)
    {
        enemyAlive[which] = false;
        enemiesAlive--;
        if (enemiesAlive <= 0)
            WonCombat();
        else
        {
            RemoveTargets();
            targetedEnemy = 0;
            while (!enemyAlive[targetedEnemy])
            {
                targetedEnemy++;
            }
            TargetedObject[targetedEnemy].SetActive(true);
        }
    }

    public void CardPlayed()
    {
        for (int i = 0; i < Enemy.Length; i++)
        {
            if (enemyAlive[i])
            {
                if (Enemy[i].effect[14] > 0)
                    Enemy[i].GainLink(0, true);
            }
        }
    }

    void WonCombat()
    {
        Player.ItemsScript.ResetText();
        for (int i = 0; i < 3; i++)
        {
            enemyAlive[i] = false;
        }
        Fade.StartDarken();
        Player.Set();
        if (boss)
            Invoke("LevelCompleted", 0.4f);
        else Invoke("ReturnToMap", 0.4f);
    }

    public void HeroesDefeated()
    {
        Fade.StartDarken();
        Invoke("ReturnToCamp", 0.4f);
    }

    void ReturnToMap()
    {
        LootEvent.SetRewards(mapDanger, elite);
        MapScript.experience += mapDanger * 0.25f;
        CombatScene.SetActive(false);
        Hand.SetActive(false);
    }

    void LevelCompleted()
    {
        LootEvent.SetRewards(mapDanger, elite);
        BossLootScript.Open(2 + bossDefeated);
        bossDefeated++;
        MapScript.experience += mapDanger * 0.25f;
        MapsScript.MapCompleted();
        CombatScene.SetActive(false);
        Hand.SetActive(false);
    }

    void ReturnToCamp()
    {
        CombatScene.SetActive(false);
        Hand.SetActive(false);
        PlayerHUD.SetActive(false);
        Maps.SetActive(false);
        if (StoryScript.StoryChapter == 4)
        {
            AdventureScript.AdventureComplete(true, mapDanger);
            StoryScript.NewDialogue();
            StoryScene.SetActive(true);
            //ResultsScene.SetActive(true);
        }
        else
        {
            PlayerPrefs.SetInt("End", 0);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            //AdventureScript.AdventureComplete(false, mapDanger);
            //ResultsScene.SetActive(true);
        }
    }

    // Display
    public void EffectHovered(bool player, bool curse, int enemy, int effect)
    {
        if (player)
        {
            if (curse)
                DisplayCurse(effect);
            else DisplayPlayerEffect(effect);
        }
        else
            DisplayEnemyEffect(enemy, effect);
    }

    void DisplayPlayerEffect(int effect)
    {
        switch (Player.effectsActive[effect])
        {
            case 0:
                EffectTooltip.text = "Strength:\nIncrease Damage Dealt by " + Player.effect[Player.effectsActive[effect]].ToString("0");
                break;
            case 1:
                EffectTooltip.text = "Resistance:\nIncrease Block Gained by " + Player.effect[Player.effectsActive[effect]].ToString("0");
                break;
            case 2:
                EffectTooltip.text = "Dexterity:\nIncrease Energy Gained by " + Player.effect[Player.effectsActive[effect]].ToString("0");
                break;
            case 3:
                EffectTooltip.text = "Stored Block:\nGain " + Player.effect[Player.effectsActive[effect]].ToString("0") +" Block at the start of next Turn";
                break;
            case 4:
                EffectTooltip.text = "Thorns:\nDeal " + Player.effect[Player.effectsActive[effect]].ToString("0") + " Damage to Enemies that attack you";
                break;
            case 5:
                EffectTooltip.text = "Hammer of Wrath:\nWeapon Attack Apply " + Player.effect[Player.effectsActive[effect]].ToString("0") + " Daze & Slow";
                break;
            case 6:
                EffectTooltip.text = "Armor:\nGain " + Player.effect[Player.effectsActive[effect]].ToString("0") + " Block at the end of every Turn";
                break;
            case 7:
                EffectTooltip.text = "Slow:\nGain 1 less Mana at the start of Turn for " + Player.effect[Player.effectsActive[effect]].ToString("0") + " Turn/s";
                break;
            case 8:
                EffectTooltip.text = "Bleed:\nTake " + Player.effect[Player.effectsActive[effect]].ToString("0") + " Damage at the end of every Turn";
                break;
            case 9:
                EffectTooltip.text = "Weak:\nReduce Damage Dealt by 25%\nlasts " + Player.effect[Player.effectsActive[effect]].ToString("0") + " Turn/s";
                break;
            case 10:
                EffectTooltip.text = "Frail:\nReduce Blcok Gained by 25%\nlasts " + Player.effect[Player.effectsActive[effect]].ToString("0") + " Turn/s";
                break;
            case 11:
                EffectTooltip.text = "Barricade:\nBlock is retained for next " + Player.effect[Player.effectsActive[effect]].ToString("0") + " Turn/s";
                break;
            case 12:
                EffectTooltip.text = "Terror:\nDraw 1 less Card at the start of Turn for " + Player.effect[Player.effectsActive[effect]].ToString("0") + " Turn/s";
                break;
            case 13:
                EffectTooltip.text = "Guardian Angel:\nBlock gained from Armor is increased by " + Player.effect[Player.effectsActive[effect]].ToString("0") + " per Resistance";
                break;
            case 14:
                EffectTooltip.text = "Swift:\nEvery Card played gives " + Player.effect[Player.effectsActive[effect]].ToString("0") + " Block";
                break;
            case 15:
                EffectTooltip.text = "Serrated Blade:\nEvery Attack applies " + Player.effect[Player.effectsActive[effect]].ToString("0") + " Bleed to target";
                break;
            case 16:
                EffectTooltip.text = "Vulnerable:\nTake " + (10 * Player.effect[Player.effectsActive[effect]]).ToString("0") + "% more Damage";
                break;
            case 17:
                EffectTooltip.text = "Trident of Storms:\nWeapon Attack Give " + Player.effect[Player.effectsActive[effect]].ToString("0") + " Storm Charge/s";
                break;
            case 18:
                EffectTooltip.text = "Storm Charge:\nUpon reaching 9 Charges summon Lighting at random Enemy";
                break;
            case 19:
                EffectTooltip.text = "Prepared:\nUpon reaching 5x Combo Gain " + Player.effect[Player.effectsActive[effect]].ToString("0") + " Block";
                break;
            case 20:
                EffectTooltip.text = "Poison:\nTake " + Player.effect[Player.effectsActive[effect]].ToString("0") + " Magic Damage at the end of every Turn";
                break;
            case 21:
                EffectTooltip.text = "Riptides:\nNext Turn, Cast Riptide " + Player.effect[Player.effectsActive[effect]].ToString("0") + " Time/s";
                break;
            case 22:
                EffectTooltip.text = "Stored Mana:\nGain " + Player.effect[Player.effectsActive[effect]].ToString("0") + " Mana at the start of next Turn";
                break;
            case 23:
                EffectTooltip.text = "Stored Cards:\nDraw " + Player.effect[Player.effectsActive[effect]].ToString("0") + " Card/s at the start of next Turn";
                break;
            case 24:
                EffectTooltip.text = "Deflect:\nGain " + Player.effect[Player.effectsActive[effect]].ToString("0") + " Stored Block & Deal " + Player.effect[Player.effectsActive[effect]].ToString("0") + " Damage when being attacked this Turn";
                break;
            case 25:
                EffectTooltip.text = "Stored Energy:\nGain " + Player.effect[Player.effectsActive[effect]].ToString("0") + " Energy at the start of next Turn";
                break;
            case 26:
                EffectTooltip.text = "Juggernaut:\nGain " + Player.effect[Player.effectsActive[effect]].ToString("0") + " Shield for every Health Lost";
                break;
            case 27:
                EffectTooltip.text = "Battle Stance:\nAttacks Give " + Player.effect[Player.effectsActive[effect]].ToString("0") + " Block, Tripled for Weapon";
                break;
        }
    }

    void DisplayEnemyEffect(int enemy, int effect)
    {
        switch (Enemy[enemy].effectsActive[effect])
        {
            case 0:
                EffectTooltip.text = "Weak:\nReduce Damage Dealt by 25%\nlasts " + Enemy[enemy].effect[Enemy[enemy].effectsActive[effect]].ToString("0") + " Turn/s";
                break;
            case 1:
                EffectTooltip.text = "Bleed:\nTake " + Enemy[enemy].effect[Enemy[enemy].effectsActive[effect]].ToString("0") + " Damage at the start of every Turn";
                break;
            case 2:
                EffectTooltip.text = "Daze:\nTake " + Enemy[enemy].effect[Enemy[enemy].effectsActive[effect]].ToString("0") + " Damage whenever getting Stunned";
                break;
            case 3:
                EffectTooltip.text = "Strength:\nIncrease Damage Dealt by " + Enemy[enemy].effect[Enemy[enemy].effectsActive[effect]].ToString("0");
                break;
            case 4:
                EffectTooltip.text = "Bone Claws:\nEvery Attack applies " + Enemy[enemy].effect[Enemy[enemy].effectsActive[effect]].ToString("0") + " Bleed to target";
                break;
            case 5:
                EffectTooltip.text = "Fly Nest:\nEvery Turn deal " + Enemy[enemy].effect[Enemy[enemy].effectsActive[effect]].ToString("0") + " Magic Damage to Player";
                break;
            case 6:
                EffectTooltip.text = "Dreadful Aura:\nEvery Turn reduce Player sanity by " + Enemy[enemy].effect[Enemy[enemy].effectsActive[effect]].ToString("0");
                break;
            case 7:
                EffectTooltip.text = "Rot:\nEvery Turn Gain " + (2 * Enemy[enemy].effect[Enemy[enemy].effectsActive[effect]]).ToString("0") + " Block, " + Enemy[enemy].effect[Enemy[enemy].effectsActive[effect]].ToString("0") + " Strength & 1 Slow";
                break;
            case 8:
                EffectTooltip.text = "Enormous:\nGain " + Enemy[enemy].effect[Enemy[enemy].effectsActive[effect]].ToString("0") + " more Tenacity when Stunned";
                break;
            case 9:
                EffectTooltip.text = "Vulnerable:\nTake " + (10 * Enemy[enemy].effect[Enemy[enemy].effectsActive[effect]]).ToString("0") + "% more Damage";
                break;
            case 10:
                EffectTooltip.text = "Chemfuel:\nDeal " + (20 + 2 * Enemy[enemy].effect[Enemy[enemy].effectsActive[effect]]).ToString("0") + "% more Damage for next " + Enemy[enemy].effect[Enemy[enemy].effectsActive[effect]].ToString("0") + " Turn/s";
                break;
            case 11:
                EffectTooltip.text = "Dark Blade:\nAfter breaking through target Block, Deal " + Enemy[enemy].effect[Enemy[enemy].effectsActive[effect]].ToString("0") + " Magic Damage to them";
                break;
            case 12:
                EffectTooltip.text = "Hollow:\nTake 40% less Damage, but gain 1 Bleed when taking Damage";
                break;
            case 13:
                EffectTooltip.text = "Armor:\nGain " + Enemy[enemy].effect[Enemy[enemy].effectsActive[effect]].ToString("0") + " Block at the end of every Turn";
                break;
            case 14:
                EffectTooltip.text = "Chain Tether:\nWhenever Card is played gain " + Enemy[enemy].effect[Enemy[enemy].effectsActive[effect]].ToString("0") + " Link, 10 Links grant 2 Strength";
                break;
            case 15:
                EffectTooltip.text = "Chain Link:\nUpon gaining 10 Links, gain 2 Strength";
                break;
            case 16:
                EffectTooltip.text = "Unstoppable:\nCan't be Stunned, take 5% Max Health as Damage instead";
                break;
            case 17:
                EffectTooltip.text = "More Flesh!:\nGain Max Health equal to unblocked Damage Dealt";
                break;
            case 18:
                EffectTooltip.text = "Monster Within:\nTransform into Monstrosity after " + Enemy[enemy].effect[Enemy[enemy].effectsActive[effect]].ToString("0") + " Turn/s, Stun delays by 1 Turn instead";
                break;
            case 19:
                EffectTooltip.text = "Soul Harvest:\nWhen you end Turn with 0 Mana, gain " + Enemy[enemy].effect[Enemy[enemy].effectsActive[effect]].ToString("0") + " Strength\n & " + (3 * Enemy[enemy].effect[Enemy[enemy].effectsActive[effect]]).ToString("0") + " Shield";
                break;
            case 20:
                EffectTooltip.text = "Reaper of Souls:\nWhen you end Turn with unspent Mana, gain 2 Strength & Deal 12 Damage per Mana left";
                break;
            case 21:
                EffectTooltip.text = "Unstable Power:\nGain " + Enemy[enemy].effect[Enemy[enemy].effectsActive[effect]].ToString("0") + " more Mana each Turn";
                break;
            case 22:
                EffectTooltip.text = "One with the Swamp:\nDeal " + Enemy[enemy].effect[Enemy[enemy].effectsActive[effect]].ToString("0") + " to Mud Piles every Turn, Mud Piles Killed by Bog-Thing gives 1 Level";
                break;
            case 23:
                EffectTooltip.text = "Just Mud:\nCan not be Killed, instead Deal " + Enemy[enemy].BogThingDamage() + " Damage to Bog-Thing & Respawns with " + Enemy[enemy].effect[Enemy[enemy].effectsActive[effect]].ToString("0") + " more Health";
                break;
        }
    }

    public void DisplayCurse(int curse)
    {
        switch (curse)
        {
            case 0:
                EffectTooltip.text = "Doubt:\nGain " + (2 * Player.PlayerScript.CurseValue[curse]).ToString("0") + " Weak. Weak is more effective";
                break;
            case 1:
                EffectTooltip.text = "Madness:\nAt the end of each Turn take " + (4 * Player.PlayerScript.CurseValue[curse]).ToString("0") + " Damage for every Card left in your hand";
                break;
            case 2:
                EffectTooltip.text = "Pride:\nEnemies gain " + (3 * Player.PlayerScript.CurseValue[curse]).ToString("0") + " Strength. Every 2 Turns enemies gain " + Player.PlayerScript.CurseValue[curse].ToString("0") + " Strength";
                break;
            case 3:
                EffectTooltip.text = "Fear:\nGain " + (20 * Player.PlayerScript.CurseValue[curse]).ToString("0") + "% Card draw skip. Taking unblocked Damage also reduces Sanity";
                break;
            case 4:
                EffectTooltip.text = "Frailty:\nGain " + (2 * Player.PlayerScript.CurseValue[curse]).ToString("0") + " Frail. Frail is more effective";
                break;
        }
    }

    public void ClassEffectHovered(int classEffect)
    {
        switch (classEffect)
        {
            case 0:
                EffectTooltip.text = "Valor:\n Gained & Used by certain Cards for empowered Effects";
                break;
            case 1:
                EffectTooltip.text = "Combo:\n Gained by playing Cards, resets each Turn";
                break;
            case 2:
                EffectTooltip.text = "Blossom:\n Gained each Turn to empower certain Cards";
                break;
            case 3:
                EffectTooltip.text = "Wrath:\n Gained by dealing & taking Damage. Used by certain Cards";
                break;
        }
    }

    public void Effect(bool player, int effect, bool vertical, int enemy = 0)
    {
        if (vertical)
            temp = Random.Range(-24f, 24f);
        else temp = Random.Range(0f, 360f);
        if (player)
            Player.Effect(ELibrary.EffectPrefab[effect], temp);
        else
            Enemy[enemy].Effect(ELibrary.EffectPrefab[effect], temp);
    }

    public void Unhovered()
    {
        EffectTooltip.text = "";
    }

    public void EnemiesGainStrength(int value)
    {
        for (int i = 0; i < Enemy.Length; i++)
        {
            if (enemyAlive[i])
            {
                Enemy[i].GainStrength(value);
            }
        }
    }

    public int RandomEnemy()
    {
        do
        {
            tempi = Random.Range(0, enemyAlive.Length);
        } while (!enemyAlive[tempi]);
        return tempi;
    }
}
