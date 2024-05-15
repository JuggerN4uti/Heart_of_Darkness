using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatTutorial : MonoBehaviour
{
    public GameObject[] TutorialPage;
    public Button PrevButton, NextButton;
    public TMPro.TextMeshProUGUI PageValue;
    public int currentPage;

    public void NextPage()
    {
        TutorialPage[currentPage].SetActive(false);
        currentPage++;
        TutorialPage[currentPage].SetActive(true);
        PrevButton.interactable = true;
        if (currentPage == 3)
            NextButton.interactable = false;
        PageValue.text = (currentPage + 1).ToString("0");
    }

    public void PreviousPage()
    {
        TutorialPage[currentPage].SetActive(false);
        currentPage--;
        TutorialPage[currentPage].SetActive(true);
        NextButton.interactable = true;
        if (currentPage == 0)
            PrevButton.interactable = false;
        PageValue.text = (currentPage + 1).ToString("0");
    }
}
