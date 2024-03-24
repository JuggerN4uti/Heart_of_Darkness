using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampChoice : MonoBehaviour
{
    [Header("Scripts")]
    public Player PlayerScript;

    [Header("Stats")]
    public int healthRestoration;
    public int sanityRestoration;

    [Header("UI")]
    public GameObject CampEventObject;
    public TMPro.TextMeshProUGUI HealthRestoredValue, SanityRestoredValue;

    public void Open()
    {
        healthRestoration = (PlayerScript.MaxHealth * 26) / 100;
        sanityRestoration = 10 + (PlayerScript.MaxSanity * 21 + 5) / 100;
        if (PlayerScript.Item[35])
        {
            PlayerScript.RestoreHealth(2 * (PlayerScript.DeckScript.cardsInDeck / 5));
            PlayerScript.RestoreSanity((PlayerScript.DeckScript.cardsInDeck / 5));
        }
        HealthRestoredValue.text = "Restore\n" + healthRestoration.ToString("") + " Health";
        SanityRestoredValue.text = "Restore\n" + sanityRestoration.ToString("") + " Sanity";
        CampEventObject.SetActive(true);
    }

    public void RestBody()
    {
        PlayerScript.RestoreHealth(healthRestoration);
        CampEventObject.SetActive(false);
    }

    public void RestSpirit()
    {
        PlayerScript.RestoreSanity(sanityRestoration);
        CampEventObject.SetActive(false);
    }

    public void Skip()
    {
        CampEventObject.SetActive(false);
    }
}
