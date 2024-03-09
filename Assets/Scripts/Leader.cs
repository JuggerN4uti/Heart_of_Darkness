using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leader : MonoBehaviour
{
    [Header("Scripts")]
    public Perks SkillTree;

    [Header("Stats")]
    public int Health;
    public int Sanity, SkillPoints, SkillsLearned;
    public bool opened;

    [Header("UI")]
    public GameObject SkillTreeHUD;

    public void OpenSkillTree()
    {
        opened = !opened;
        SkillTreeHUD.SetActive(opened);
        SkillTree.UpdateStats();
    }
}
