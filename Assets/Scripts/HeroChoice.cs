using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroChoice : MonoBehaviour
{
    [Header("Scripts")]
    public Player PlayerScript;

    [Header("UI")]
    public GameObject HeroChoiceScene;
    public GameObject MapScene, PlayerHUD;

    [Header("Stats")]
    public int[] LightStats;

    public void ChooseHero(int which)
    {
        switch (which)
        {
            case 0:
                for (int i = 0; i < LightStats.Length; i++)
                {
                    PlayerScript.StatValues[i] += LightStats[i];
                }
                break;
        }
        PlayerScript.GainStats();
        HeroChoiceScene.SetActive(false);
        MapScene.SetActive(true);
        PlayerHUD.SetActive(true);
    }
}
