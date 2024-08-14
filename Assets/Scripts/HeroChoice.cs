using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroChoice : MonoBehaviour
{
    [Header("Scripts")]
    public Player PlayerScript;
    public Deck DeckScript;
    public CardLibrary CLibrary;

    [Header("UI")]
    public GameObject HeroChoiceScene;
    public GameObject EquipmentChoiceScene;
    public GameObject MapScene, ValorDisplay, ComboDisplay, BlossomDisplay, WrathDisplay;
    public Image HeroImage, WeaponImage, WeaponImage2;

    [Header("Stats")]
    public int[] LightStats;
    public int[] WaterStats;
    public int[] NatureStats;
    public int[] BloodStats;
    public int[] weaponDamage, weaponStrengthBonus, weaponEnergyRequirement;
    public string[] weaponName;

    [Header("Sprites")]
    public Sprite[] HeroSprites;
    public Sprite[] WeaponSprite;

    public void ChooseHero(int which)
    {
        switch (which)
        {
            case 0:
                for (int i = 0; i < LightStats.Length; i++)
                {
                    PlayerScript.StatValues[i] += LightStats[i];
                }
                ValorDisplay.SetActive(true);
                DeckScript.CardID[4] = 24 + CLibrary.neutralCards;
                DeckScript.CardID[9] = 11 + CLibrary.neutralCards;
                break;
            case 1:
                for (int i = 0; i < LightStats.Length; i++)
                {
                    PlayerScript.StatValues[i] += WaterStats[i];
                }
                ComboDisplay.SetActive(true);
                DeckScript.CardID[4] = 9 + CLibrary.neutralCards + CLibrary.lightCards;
                DeckScript.CardID[9] = 26 + CLibrary.neutralCards + CLibrary.lightCards;
                break;
            case 2:
                for (int i = 0; i < LightStats.Length; i++)
                {
                    PlayerScript.StatValues[i] += NatureStats[i];
                }
                BlossomDisplay.SetActive(true);
                DeckScript.CardID[4] = 8 + CLibrary.neutralCards + CLibrary.lightCards + CLibrary.waterCards;
                DeckScript.CardID[9] = 2 + CLibrary.neutralCards + CLibrary.lightCards + CLibrary.waterCards;
                break;
            case 3:
                for (int i = 0; i < LightStats.Length; i++)
                {
                    PlayerScript.StatValues[i] += BloodStats[i];
                }
                WrathDisplay.SetActive(true);
                DeckScript.CardID[4] = 0 + CLibrary.neutralCards + CLibrary.lightCards + CLibrary.waterCards + CLibrary.natureCards;
                DeckScript.CardID[9] = 2 + CLibrary.neutralCards + CLibrary.lightCards + CLibrary.waterCards + CLibrary.natureCards;
                break;
        }
        PlayerScript.Class = which;
        HeroImage.sprite = HeroSprites[which];
        PlayerScript.WeaponSprite = WeaponSprite[which];
        WeaponImage.sprite = WeaponSprite[which];
        WeaponImage2.sprite = WeaponSprite[which];
        PlayerScript.weaponDamage = weaponDamage[which];
        PlayerScript.weaponStrengthBonus = weaponStrengthBonus[which];
        PlayerScript.weaponEnergyRequirement = weaponEnergyRequirement[which];
        PlayerScript.weaponName = weaponName[which];
        PlayerScript.GainStats();
        HeroChoiceScene.SetActive(false);
        EquipmentChoiceScene.SetActive(true);
        //MapScene.SetActive(true);
    }
}
