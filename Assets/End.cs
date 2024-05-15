using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class End : MonoBehaviour
{
    public int endValue;
    public GameObject[] EndScreens;

    void Start()
    {
        endValue = PlayerPrefs.GetInt("End");
        EndScreens[endValue].SetActive(true);
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene(0);
    }
}
