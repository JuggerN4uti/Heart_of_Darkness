using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Scripts")]
    public Deck DeckScript;
    public UnitChoice[] Units;

    [Header("Stats")]
    public int[] StatValues;
    public int[] DrawbackValues;
    public int unitUnderCommand;

    public void GainUnitsStats()
    {
        for (int j = 0; j < unitUnderCommand; j++)
        {
            for (int i = 0; i < Units[j].AbilitiesAmount; i++)
            {
                DeckScript.AddACard(Units[j].Abilities[i], Units[j].AbilitiesLevel[i]);
            }
            for (int i = 0; i < Units[j].PerksAmount; i++)
            {
                StatValues[Units[j].Perks[i]] += Units[j].PerksValue[i];
            }
            for (int i = 0; i < Units[j].FlawsAmount; i++)
            {
                DrawbackValues[Units[j].Flaws[i]] += Units[j].FlawsValue[i];
            }
        }
    }
}
