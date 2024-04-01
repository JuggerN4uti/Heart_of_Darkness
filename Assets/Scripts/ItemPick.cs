using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemPick : MonoBehaviour
{
    [Header("Scripts")]
    public ItemLibrary Library;
    public Player PlayerScript;

    [Header("Stats")]
    public int[] rolledID;
    int roll;

    [Header("UI")]
    public GameObject ItemPickObject;
    public Image[] ItemIcon;
    public TMPro.TextMeshProUGUI[] ItemName, ItemEffectText;

    public void RollItems()
    {
        do
        {
            roll = Random.Range(0, Library.Items.Length);
        } while ((PlayerScript.Item[roll]) || (Library.Items[roll].nonNeutral && Library.Items[roll].Class != PlayerScript.Class));
        rolledID[0] = roll;

        do
        {
            roll = Random.Range(0, Library.Items.Length);
        } while ((PlayerScript.Item[roll] || roll == rolledID[0]) || (Library.Items[roll].nonNeutral && Library.Items[roll].Class != PlayerScript.Class));
        rolledID[1] = roll;

        do
        {
            roll = Random.Range(0, Library.Items.Length);
        } while ((PlayerScript.Item[roll] || roll == rolledID[0] || roll == rolledID[1]) || (Library.Items[roll].nonNeutral && Library.Items[roll].Class != PlayerScript.Class));
        rolledID[2] = roll;

        ItemPickObject.SetActive(true);
        SetItems();
    }

    void SetItems()
    {
        for (int i = 0; i < 3; i++)
        {
            ItemIcon[i].sprite = Library.Items[rolledID[i]].ItemSprite;
            ItemName[i].text = Library.Items[rolledID[i]].ItemName;
            ItemEffectText[i].text = Library.Items[rolledID[i]].ItemTooltip;
        }
    }

    public void CollectCard(int slot)
    {
        PlayerScript.CollectItem(rolledID[slot]);
        ItemPickObject.SetActive(false);
    }

    public void SkipChoice()
    {
        ItemPickObject.SetActive(false);
    }
}
