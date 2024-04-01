using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [Header("Scripts")]
    public Deck DeckScript;
    public ItemsCollected ItemsScript;
    public UnitChoice[] Units;

    [Header("Stats")]
    public int Class;
    public int Health, MaxHealth, BaseHealth, Sanity, MaxSanity, Silver, Iron, SanityLost, BaseMana, BaseDraw;
    public int[] Gems;
    public int[] StatValues, EffectID, CurseID;
    public int unitUnderCommand;
    public bool opened, map;
    public bool[] Item; //Equipment; idk czy daæ mo¿liwe kilka 
    public int[] equipment;
    public int equipmentLevel;
    public bool secondEquipment;
    int roll;

    [Header("Curses")]
    public int CursesCount;
    public int[] CurseValue;
    public string[] CurseName;

    [Header("Weapon")]
    public int weaponLevel;
    public int weaponDamage, weaponStrengthBonus, weaponEnergyRequirement;
    public string weaponName;
    public Sprite WeaponSprite;

    [Header("Info")]
    public TMPro.TextMeshProUGUI[] WeaponInfoText;
    public GameObject[] EffectObject, CurseObject;
    public Image[] EffectIcon, CurseIcon;
    public TMPro.TextMeshProUGUI HoveredText;
    public TMPro.TextMeshProUGUI[] EffectValueText, CurseValueText;
    int count;

    [Header("UI")]
    public GameObject InfoObject;
    public GameObject DeckOpenButton, MapInfo, AddCommonButton, AddUncommonButton;
    public Image HealthFill, SanityFill;
    public TMPro.TextMeshProUGUI HealthText, SanityText, SilverText, IronText, CommonsText, UncommonsText, RaresText, CommonAddText, UncommonAddText;

    [Header("Sprites")]
    public Sprite[] EffectSprite;
    public Sprite[] CurseSprite;

    public void GainUnitsStats()
    {
        for (int j = 0; j < unitUnderCommand; j++)
        {
            for (int i = 0; i < StatValues.Length; i++)
            {
                StatValues[i] += Units[j].PerksValue[i];
            }
        }
        GainStats();
    }

    public void GainStats()
    {
        for (int i = 0; i < Gems.Length; i++)
        {
            Gems[i] = 0;
        }
        SanityLost = 0;
        MaxHealth += StatValues[0];
        BaseHealth = MaxHealth;
        MaxSanity += StatValues[1];
        Silver += StatValues[4];
        Health = MaxHealth;
        Sanity = MaxSanity;
        LoseSanity(StatValues[10], false);
        UpdateInfo();
    }

    public void LoseSanity(int amount, bool considerated)
    {
        if (considerated)
            SanityLost += amount;
        else
        {
            SanityLost += amount;
            Sanity -= amount;
            UpdateInfo();
        }
    }

    public void UpdateInfo()
    {
        HealthFill.fillAmount = (Health * 1f) / (MaxHealth * 1f);
        HealthText.text = Health.ToString("") + "/" + MaxHealth.ToString("");

        SanityFill.fillAmount = (Sanity * 1f) / (MaxSanity * 1f);
        SanityText.text = Sanity.ToString("") + "/" + MaxSanity.ToString("");

        SilverText.text = Silver.ToString("");
        IronText.text = Iron.ToString("");
        CommonsText.text = DeckScript.CommonCardsInDeck().ToString("0");
        UncommonsText.text = DeckScript.UncommonCardsInDeck().ToString("0");
        RaresText.text = DeckScript.RareCardsInDeck().ToString("0");

        if (StatValues[2] > 0)
        {
            AddCommonButton.SetActive(true);
            CommonAddText.text = StatValues[2].ToString("0");
        }
        else AddCommonButton.SetActive(false);

        if (StatValues[3] > 0)
        {
            AddUncommonButton.SetActive(true);
            UncommonAddText.text = StatValues[3].ToString("0");
        }
        else AddUncommonButton.SetActive(false);
    }

    public void ShowInfo()
    {
        if (opened)
        {
            opened = false;
            InfoObject.SetActive(false);
            DeckOpenButton.SetActive(true);
        }
        else
        {
            opened = true;
            DisplayPlayerInfo();
            InfoObject.SetActive(true);
            DeckOpenButton.SetActive(false);
        }
    }

    public void ShowMap()
    {
        if (!map)
        {
            MapInfo.SetActive(true);
            map = true;
        }
        else
        {
            MapInfo.SetActive(false);
            map = false;
        }
    }

    public void GainHP(int amount)
    {
        MaxHealth += amount;
        Health += amount;
        UpdateInfo();
    }

    public void RestoreHealth(int amount)
    {
        Health += amount;
        if (Health > MaxHealth)
            Health = MaxHealth;
        UpdateInfo();
    }

    public void GainMaxSanity(int amount)
    {
        MaxSanity += amount;
        Sanity += amount;
        UpdateInfo();
    }

    public void RestoreSanity(int amount)
    {
        Sanity += amount;
        UpdateInfo();
    }

    public void GainSilver(int amount)
    {
        Silver += amount;
        UpdateInfo();
    }

    public void SpendSilver(int amount)
    {
        Silver -= amount;
        UpdateInfo();
    }

    public void GainIron(int amount)
    {
        Iron += amount;
        UpdateInfo();
    }

    public void SpendIron(int amount)
    {
        Iron -= amount;
        UpdateInfo();
    }

    public void AddCommon()
    {
        StatValues[2]--;
        UpdateInfo();
    }

    public void AddUncommon()
    {
        StatValues[3]--;
        UpdateInfo();
    }

    public void RemoveCurse()
    {
        if (CursesCount > 0)
        {
            do
            {
                roll = Random.Range(0, CurseValue.Length);
            } while (CurseValue[roll] == 0);
            CurseValue[roll]--;
            CursesCount--;
        }
    }

    public void CollectItem(int which)
    {
        ItemsScript.CollectItem(which);
        Item[which] = true;
        switch (which)
        {
            case 1:
                GainHP(20);
                break;
            case 2:
                StatValues[8] += 2;
                break;
            case 3:
                StatValues[7] += 2;
                break;
            case 4:
                StatValues[9] += 3;
                break;
            case 6:
                BaseMana++;
                break;
            case 7:
                StatValues[5] += 7;
                // thorns
                break;
            case 18:
                BaseMana++;
                break;
            case 20:
                BaseMana++;
                BaseDraw--;
                break;
            case 25:
                StatValues[8] += 1;
                break;
            case 27:
                StatValues[5] += 5;
                break;
            case 28:
                StatValues[8] += 1;
                break;
            case 29:
                StatValues[7] += 1;
                break;
            case 30:
                StatValues[8] += 1;
                CheckForRings();
                break;
            case 31:
                StatValues[7] += 1;
                CheckForRings();
                break;
            case 32:
                StatValues[9] += 2;
                CheckForRings();
                break;
            case 33:
                CheckForRings();
                break;
            case 34:
                StatValues[9] += 1;
                break;
            case 35:
                GainHP(4);
                break;
            case 37:
                GainHP(8);
                break;
        }
        UpdateInfo();
    }

    void DisplayPlayerInfo()
    {
        WeaponInfoText[0].text = weaponDamage.ToString("");
        WeaponInfoText[1].text = "+" + weaponStrengthBonus.ToString("") + "/";
        WeaponInfoText[2].text = weaponEnergyRequirement.ToString("");

        count = 0;
        for (int i = 5; i < StatValues.Length - 1; i++)
        {
            if (StatValues[i] > 0)
            {
                EffectObject[count].SetActive(true);
                EffectIcon[count].sprite = EffectSprite[i - 5];
                EffectValueText[count].text = StatValues[i].ToString("");
                EffectID[count] = i;
                count++;
            }
        }

        count = 0;
        for (int i = 0; i < CurseValue.Length; i++)
        {
            if (CurseValue[i] > 0)
            {
                CurseObject[count].SetActive(true);
                CurseIcon[count].sprite = CurseSprite[i];
                CurseValueText[count].text = CurseValue[i].ToString("");
                CurseID[count] = i;
                count++;
            }
        }
    }

    public void InfoHovered(bool curse, int order)
    {
        if (curse)
        {
            switch (CurseID[order])
            {
                case 0:
                    HoveredText.text = "Doubt:\nGain " + (2 * CurseValue[CurseID[order]]).ToString("0") + " Weak. Weak is more effective";
                    break;
                case 1:
                    HoveredText.text = "Madness:\nAt the end of each Turn take " + (4 * CurseValue[CurseID[order]]).ToString("0") + " Damage for every Card left in your hand";
                    break;
                case 2:
                    HoveredText.text = "Pride:\nEnemies gain " + (3 * CurseValue[CurseID[order]]).ToString("0") + " Strength. Every 2 Turns enemies gain " + CurseValue[CurseID[order]].ToString("0") + " Strength";
                    break;
                case 3:
                    HoveredText.text = "Fear:\nGain " + (20 * CurseValue[CurseID[order]]).ToString("0") + "% Card draw skip. Taking unblocked Damage also reduces Sanity";
                    break;
                case 4:
                    HoveredText.text = "Frailty:\nGain " + (2 * CurseValue[CurseID[order]]).ToString("0") + " Frail. Frail is more effective";
                    break;
            }
        }
        else
        {
            switch (EffectID[order])
            {
                case 5:
                    HoveredText.text = "Shield\nStart each Combat with " + StatValues[EffectID[order]].ToString("0") + " Shield";
                    break;
                case 6:
                    HoveredText.text = "Armor\nStart each Combat with " + StatValues[EffectID[order]].ToString("0") + " Armor";
                    break;
                case 7:
                    HoveredText.text = "Resistance:\nStart each Combat with " + StatValues[EffectID[order]].ToString("0") + " Resistance";
                    break;
                case 8:
                    HoveredText.text = "Strength:\nStart each Combat with " + StatValues[EffectID[order]].ToString("0") + " Strength";
                    break;
                case 9:
                    HoveredText.text = "Dexterity:\nStart each Combat with " + StatValues[EffectID[order]].ToString("0") + " Dexterity";
                    break;
            }
        }
    }

    public void Unhovered()
    {
        HoveredText.text = "";
    }

    void CheckForRings()
    {
        if (Item[30] && Item[31] && Item[32] && Item[33])
            BaseMana++;
    }
}
