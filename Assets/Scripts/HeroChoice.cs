using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroChoice : MonoBehaviour
{
    [Header("Scripts")]
    public Player PlayerScript;
    public Deck DeckScript;

    [Header("UI")]
    public GameObject HeroChoiceScene;
    public GameObject EquipmentChoiceScene;
    public GameObject MapScene, PlayerHUD, ComboDisplay;
    public Image HeroImage;

    [Header("Stats")]
    public int[] LightStats;
    public int[] WaterStats;
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
                DeckScript.CardID[4] = 26;
                DeckScript.CardID[9] = 13;
                break;
            case 1:
                for (int i = 0; i < LightStats.Length; i++)
                {
                    PlayerScript.StatValues[i] += WaterStats[i];
                }
                ComboDisplay.SetActive(true);
                DeckScript.CardID[4] = 46;
                DeckScript.CardID[9] = 63;
                break;
        }
        PlayerScript.Class = which;
        HeroImage.sprite = HeroSprites[which];
        PlayerScript.WeaponSprite = WeaponSprite[which];
        PlayerScript.weaponDamage = weaponDamage[which];
        PlayerScript.weaponStrengthBonus = weaponStrengthBonus[which];
        PlayerScript.weaponEnergyRequirement = weaponEnergyRequirement[which];
        PlayerScript.weaponName = weaponName[which];
        PlayerScript.GainStats();
        HeroChoiceScene.SetActive(false);
        EquipmentChoiceScene.SetActive(true);
        //MapScene.SetActive(true);
        PlayerHUD.SetActive(true);
    }
}
