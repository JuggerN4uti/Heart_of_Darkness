using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public string UnitName;
    public int UnitID, UnitHealth, UnitShield, UnitTenacity, MovesCount;
    public bool boss;
    public Sprite UnitSprite;
    public Sprite[] MovesSprite;
    public int[] MovesValues, MovesCooldowns, StartingEffects;
    public string[] additionalText;
    public bool[] attackIntention, normalAttack;
}
