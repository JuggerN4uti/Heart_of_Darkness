using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Adventure : MonoBehaviour
{
    [Header("UI")]
    public GameObject StoryScene;
    public GameObject HeroChoiceScene;

    [Header("Stats")]
    public int Story;
    public bool check;

    void Start()
    {
        if (check)
            Story = PlayerPrefs.GetInt("Story");

        if (Story == 1)
            StoryScene.SetActive(true);
        else HeroChoiceScene.SetActive(true);
    }
}
