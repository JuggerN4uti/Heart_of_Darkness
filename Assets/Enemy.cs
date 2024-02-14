using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public string UnitName;
    public int UnitID, UnitHealth, UnitShield, MovesCount;
    public Sprite UnitSprite;
    public Sprite[] MovesSprite;
    public int[] MovesValues;
    public string[] additionalText;
    public bool[] attackIntention;
}
