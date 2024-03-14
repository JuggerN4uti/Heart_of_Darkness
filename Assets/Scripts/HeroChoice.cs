using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroChoice : MonoBehaviour
{
    [Header("Scripts")]
    public Player PlayerScript;

    [Header("UI")]
    public GameObject HeroChoiceScene;
    public GameObject MapScene, PlayerHUD;
    public Image HeroImage;

    [Header("Stats")]
    public int[] LightStats;
    public int[] WaterStats;

    [Header("Sprites")]
    public Sprite[] HeroSprites;

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
            case 1:
                for (int i = 0; i < LightStats.Length; i++)
                {
                    PlayerScript.StatValues[i] += WaterStats[i];
                }
                break;
        }
        PlayerScript.Class = which;
        HeroImage.sprite = HeroSprites[which];
        PlayerScript.GainStats();
        HeroChoiceScene.SetActive(false);
        MapScene.SetActive(true);
        PlayerHUD.SetActive(true);
    }
}
