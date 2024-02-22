using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTile : MonoBehaviour
{
    [Header("Stats")]
    public bool middle;
    public bool[] path;

    [Header("UI")]
    public GameObject[] Line;

    void Start()
    {
        SetPaths();
    }

    void SetPaths()
    {
        if (middle)
        {
            for (int i = 0; i < 2; i++)
            {
                if (Random.Range(1, 10) > 7)
                    UnlockPath(i);
            }
        }
        else
        {
            if (Random.Range(1, 18) > 11)
                UnlockPath(0);
        }
    }

    void UnlockPath(int which)
    {
        path[which] = true;
        Line[which].SetActive(true);
    }
}
