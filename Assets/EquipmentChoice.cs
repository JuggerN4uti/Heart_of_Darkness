using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentChoice : MonoBehaviour
{
    [Header("Scripts")]
    public Player PlayerScript;
    public EquipmentLibrary Library;
    public Maps MapsScript;

    [Header("Stats")]
    public int[] rolledID;
    public int roll;
    public bool taken;

    [Header("UI")]
    public GameObject EquipmentChoiceScene;
    public Image[] EqIcon;
    public TMPro.TextMeshProUGUI[] EqName, EqCost, EqUses, EqCooldown, EqEffect;

    void Start()
    {
        RollChoices();
    }

    public void RollChoices()
    {
        roll = Random.Range(0, Library.Equipments.Length);
        rolledID[0] = roll;

        do
        {
            roll = Random.Range(0, Library.Equipments.Length);
        } while (roll == rolledID[0]);
        rolledID[1] = roll;

        do
        {
            roll = Random.Range(0, Library.Equipments.Length);
        } while (roll == rolledID[0] || roll == rolledID[1]);
        rolledID[2] = roll;

        SetEquipment();
    }

    void SetEquipment()
    {
        for (int i = 0; i < 3; i++)
        {
            EqIcon[i].sprite = Library.Equipments[rolledID[i]].EquipmentSprite;
            EqName[i].text = Library.Equipments[rolledID[i]].EquipmentName;
            EqCost[i].text = Library.Equipments[rolledID[i]].Cost.ToString("");
            EqUses[i].text = Library.Equipments[rolledID[i]].Uses.ToString("");
            EqCooldown[i].text = Library.Equipments[rolledID[i]].Gain.ToString("") + "/" + Library.Equipments[rolledID[i]].Cooldown.ToString("");
            EqEffect[i].text = Library.Equipments[rolledID[i]].EquipmentTooltip;
        }
    }

    public void ChooseEquipment(int slot)
    {
        if (taken)
            PlayerScript.equipment[1] = rolledID[slot];
        else PlayerScript.equipment[0] = rolledID[slot];
        EquipmentChoiceScene.SetActive(false);
        MapsScript.NextMap();
        taken = true;
    }
}
