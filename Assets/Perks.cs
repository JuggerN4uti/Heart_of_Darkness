using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Perks : MonoBehaviour
{
    [Header("Scipts")]
    public Leader LeaderScript;

    [Header("UI")]
    public RectTransform SlidingMap;
    public Slider slider;
    public TMPro.TextMeshProUGUI SkillTooltip;
    public Button[] PerkButton;
    public Image[] PerkImage;

    [Header("Stats")]
    public int[] SkillCost;

    [Header("Sprites")]
    public Sprite ToLearnSprite;
    public Sprite LearnedSprite;

    public void Slide()
    {
        SlidingMap.position = new Vector2(-slider.value, 0f);
    }

    public void UpdateStats()
    {
        if (LeaderScript.SkillPoints >= SkillCost[LeaderScript.SkillsLearned])
            PerkButton[LeaderScript.SkillsLearned].interactable = true;
        else PerkButton[LeaderScript.SkillsLearned].interactable = false;
    }

    public void LearnSkill()
    {
        LeaderScript.SkillPoints -= SkillCost[LeaderScript.SkillsLearned];
        PerkButton[LeaderScript.SkillsLearned].interactable = false;
        PerkImage[LeaderScript.SkillsLearned].sprite = LearnedSprite;

        LeaderScript.SkillsLearned++;

        PerkImage[LeaderScript.SkillsLearned].sprite = ToLearnSprite;
        UpdateStats();
    }

    public void SkillHovered(string SkillName, string SkillEffect)
    {
        SkillTooltip.text = SkillName + ":\n" + SkillEffect;
    }

    public void Unhovered()
    {
        SkillTooltip.text = "";
    }
}
