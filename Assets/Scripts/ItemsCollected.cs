using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemsCollected : MonoBehaviour
{
    [Header("Scripts")]
    public ItemLibrary Library;
    public PlayerCombat Player;

    [Header("UI")]
    public GameObject[] ItemObject;
    public GameObject[] ItemTextObject;
    public Image[] ItemImage;
    public TMPro.TextMeshProUGUI[] ItemText;

    [Header("Stats")]
    public int[] ItemID;
    public int collected;

    public void CollectItem(int which)
    {
        ItemObject[collected].SetActive(true);
        ItemImage[collected].sprite = Library.Items[which].ItemSprite;
        if (Library.Items[which].text)
            ItemTextObject[collected].SetActive(true);
        ItemID[collected] = which;
        collected++;
    }

    public void SetText()
    {
        for (int i = 0; i < collected; i++)
        {
            switch (ItemID[i])
            {
                case 8:
                    ItemText[i].text = (Player.turns % 3).ToString("");
                    break;
                case 9:
                    ItemText[i].text = (Player.attacks % 6).ToString("");
                    break;
                case 10:
                    ItemText[i].text = (Player.attacks % 6).ToString("");
                    break;
                case 11:
                    ItemText[i].text = (Player.attacks % 3).ToString("");
                    break;
                case 12:
                    ItemText[i].text = (Player.attacks % 8).ToString("");
                    break;
                case 17:
                    ItemText[i].text = (Player.attacks % 4).ToString("");
                    break;
                case 19:
                    ItemText[i].text = (Player.Cards.cardsPlayed % 6).ToString("");
                    break;
                case 34:
                    ItemText[i].text = (Player.drink).ToString("");
                    break;
                case 43:
                    ItemText[i].text = (3 + Player.CombatScript.turn).ToString("");
                    break;
            }
        }
    }

    public void ResetText()
    {
        for (int i = 0; i < collected; i++)
        {
            ItemText[i].text = "";
        }
    }
}
