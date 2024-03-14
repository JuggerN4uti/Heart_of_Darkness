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
    public Story StoryScript;
    public Map MapScript;
    public SceneChange Fade;
    public AdventureResults AdventureScript;

    [Header("Stats")]
    public bool[] enemyAlive;
    public int targetedEnemy, turn, enemiesAlive;
    public bool elite;
    int whichEnemy;
    float temp;

    [Header("UI")]
    public Button EndTurnButton;
    public TMPro.TextMeshProUGUI TurnCounter, EffectTooltip;
    public GameObject CombatScene, Hand, StoryScene, ResultsScene;
    // public string/image[] playerEffects, enemyEffects; mo¿e potem zamieniæ na premade tooltipy

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

        ResetCombat();
    }

    void ResetCombat()
    {
        turn = 1;
        Player.Reset();
        TurnCounter.text = turn.ToString("");
        EndTurnButton.interactable = true;
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
            targetedEnemy = 0;
            while (!enemyAlive[targetedEnemy])
            {
                targetedEnemy++;
            }
        }
    }

    public void CardPlayed()
    {
        for (int i = 0; i < Enemy.Length; i++)
        {
            if (enemyAlive[i])
            {
                if (Enemy[i].effect[13] > 0)
                    Enemy[i].GainLink();
            }
        }
    }

    void WonCombat()
    {
        Fade.StartDarken();
        Player.Set();
        Invoke("ReturnToMap", 0.4f);
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

    void ReturnToCamp()
    {
        CombatScene.SetActive(false);
        Hand.SetActive(false);
        if (StoryScript.StoryChapter == 4)
        {
            AdventureScript.AdventureComplete(true, mapDanger);
            StoryScript.NewDialogue();
            StoryScene.SetActive(true);
            //ResultsScene.SetActive(true);
        }
        else
        {
            AdventureScript.AdventureComplete(false, mapDanger);
            ResultsScene.SetActive(true);
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
                EffectTooltip.text = "Valor:\nEmpower effects of certain Cards by " + Player.effect[Player.effectsActive[effect]].ToString("0");
                break;
            case 5:
                EffectTooltip.text = "Hammer of Wrath:\nEvery Weapon Attack Draws " + Player.effect[Player.effectsActive[effect]].ToString("0") + " Card/s";
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
                EffectTooltip.text = "Rot:\nEvery Turn Gain " + (3 * Enemy[enemy].effect[Enemy[enemy].effectsActive[effect]]).ToString("0") + " Block, " + Enemy[enemy].effect[Enemy[enemy].effectsActive[effect]].ToString("0") + " Strength & 1 Slow";
                break;
            case 8:
                EffectTooltip.text = "Enormous:\nGain " + Enemy[enemy].effect[Enemy[enemy].effectsActive[effect]].ToString("0") + " more Tenacity when Stunned";
                break;
            case 9:
                EffectTooltip.text = "Vulnerable:\nTake " + (10 * Enemy[enemy].effect[Enemy[enemy].effectsActive[effect]]).ToString("0") + "% more Damage";
                break;
            case 10:
                EffectTooltip.text = "Chemfuel:\nDeal " + (28 + 4 * Enemy[enemy].effect[Enemy[enemy].effectsActive[effect]]).ToString("0") + "% more Damage for next " + Enemy[enemy].effect[Enemy[enemy].effectsActive[effect]].ToString("0") + " Turn/s";
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
                EffectTooltip.text = "Chain Link:\nUpon gaining 9 Links, gain 2 Strength";
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
                EffectTooltip.text = "Pride:\nEnemies gain " + (2 * Player.PlayerScript.CurseValue[curse]).ToString("0") + " Strength. Each Turn enemies gain " + Player.PlayerScript.CurseValue[curse].ToString("0") + " Strength";
                break;
            case 3:
                EffectTooltip.text = "Fear:\nGain " + (20 * Player.PlayerScript.CurseValue[curse]).ToString("0") + "% Card draw skip. Taking unblocked Damage also reduces Sanity";
                break;
            case 4:
                EffectTooltip.text = "Frailty:\nGain " + (2 * Player.PlayerScript.CurseValue[curse]).ToString("0") + " Frail. Frail is more effective";
                break;
        }
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
}
