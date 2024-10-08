using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Maps : MonoBehaviour
{
    [Header("Scripts")]
    public Map[] maps;

    [Header("Stats")]
    public int currentMap;

    [Header("UI")]
    public GameObject[] MapObject;

    public void NextMap()
    {
        MapObject[currentMap].SetActive(true);
        maps[currentMap].danger *= 1.1f + 0.1f * currentMap;
        maps[currentMap].eliteDanger *= 1.1f + 0.1f * currentMap;
    }

    public void MapCompleted()
    {
        if (currentMap < 2)
        {
            maps[currentMap + 1].danger = maps[currentMap].danger;
            maps[currentMap + 1].danger += 0.25f * (currentMap + 1);
            maps[currentMap + 1].eliteDanger = maps[currentMap].eliteDanger;
            maps[currentMap + 1].danger += 0.1f * (currentMap + 1);
            maps[currentMap + 1].experience = maps[currentMap].experience;
            MapObject[currentMap].SetActive(false);
            currentMap++;
            NextMap();
        }
        else
        {
            PlayerPrefs.SetInt("End", 1);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}
