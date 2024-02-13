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
    public TMPro.TextMeshProUGUI TurnCounter;

    public void EndTurn()
    {
        Player.EndTurn();

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
    }

    void EnemyTurn()
    {
        while (!enemyAlive[whichEnemy])
        {
            whichEnemy++;
        }
        Enemy[whichEnemy].EndTurn();
        whichEnemy++;
    }
}
