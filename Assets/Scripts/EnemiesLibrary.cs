using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesLibrary : MonoBehaviour
{
    public Enemy[] Enemies;
    public int[] basicID, eliteID;
    int roll;

    public int BasicRoll()
    {
        roll = Random.Range(0, basicID.Length);
        return basicID[roll];
    }

    public int EliteRoll()
    {
        roll = Random.Range(0, eliteID.Length);
        return eliteID[roll];
    }
}
