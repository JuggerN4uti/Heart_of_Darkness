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
        healthRestoration = PlayerScript.MaxHealth / 4;
        sanityRestoration = 10 + PlayerScript.MaxSanity / 5;
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
