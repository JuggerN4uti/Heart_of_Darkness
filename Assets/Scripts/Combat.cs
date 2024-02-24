using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Combat : MonoBehaviour
{
    [Header("Scripts")]
    public PlayerCombat Player;
    public EnemyCombat[] Enemy;
    public SceneChange Fade;
    public CardPick CardChoice;

    [Header("Stats")]
    public bool[] enemyAlive;
    public int targetedEnemy, turn, enemiesAlive;
    int whichEnemy;
    float temp;

    [Header("UI")]
    public Button EndTurnButton;
    public TMPro.TextMeshProUGUI TurnCounter, EffectTooltip;
    public GameObject CombatScene, Hand, CardPickObject;
    // public string/image[] playerEffects, enemyEffects; mo¿e potem zamieniæ na premade tooltipy

    public void Start()
    {
        //EffectTooltip.text = Player.Cards.Library.Cards[2].CardTooltip[0];
    }

    public void SetEnemy(int enemyID)
    {
        Enemy[0].Unit.SetActive(true);
        Enemy[0].SetUnit(enemyID);
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
        if (enemiesAlive == 0)
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

    void WonCombat()
    {
        Fade.StartFastDarken();
        Player.Set();
        Invoke("ReturnToMap", 0.25f);
    }

    void ReturnToMap()
    {
        CardChoice.RollCards();
        CardPickObject.SetActive(true);
        CombatScene.SetActive(false);
        Hand.SetActive(false);
    }

    // Display
    public void EffectHovered(bool player, int enemy, int effect)
    {
        if (player)
            DisplayPlayerEffect(effect);
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
