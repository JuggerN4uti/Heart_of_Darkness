using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Combat : MonoBehaviour
{
    [Header("Scripts")]
    public PlayerCombat Player;
    public EnemyCombat[] Enemy;

    [Header("Stats")]
    public bool[] enemyAlive;
    public int targetedEnemy, turn;
    int whichEnemy;
    float temp;

    [Header("UI")]
    public Button EndTurnButton;
    public TMPro.TextMeshProUGUI TurnCounter, EffectTooltip;
    // public string/image[] playerEffects, enemyEffects; mo¿e potem zamieniæ na premade tooltipy

    public void Start()
    {
        //EffectTooltip.text = Player.Cards.Library.Cards[2].CardTooltip[0];
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
                EffectTooltip.text = "Light Surge:\nGain " + Player.effect[Player.effectsActive[effect]].ToString("0") + " Valor at the start of every Turn";
                break;
        }
    }

    void DisplayEnemyEffect(int enemy, int effect)
    {
        switch (Enemy[enemy].effectsActive[effect])
        {
            case 0:
                EffectTooltip.text = "Weak:\nReduce Damage Dealt by 25%\nlasts " + Enemy[enemy].effect[Enemy[enemy].effectsActive[effect]].ToString("0") + " Turns";
                break;
            case 1:
                EffectTooltip.text = "Bleed:\nTake " + Enemy[enemy].effect[Enemy[enemy].effectsActive[effect]].ToString("0") + " Damage at the start of every Turn";
                break;
            case 2:
                EffectTooltip.text = "Daze:\nTake " + Enemy[enemy].effect[Enemy[enemy].effectsActive[effect]].ToString("0") + " Damage whenever getting Stunned";
                break;
        }
    }

    public void Unhovered()
    {
        EffectTooltip.text = "";
    }
}
