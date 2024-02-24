using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FirstMap : MonoBehaviour
{
    [Header("Scripts")]
    public Player PlayerScript;
    public Combat CombatScript;
    public Story StoryScript;
    public SceneChange Fade;
    public ForgeChoice ForgeScript;

    [Header("Stats")]
    public int[] tileEvent; // 0 - Card Pick, 1 - Forge, 2 - Threat, 3 - Watchtower
    public int tilesAmount, currentTile;

    [Header("UI")]
    public GameObject CardEventObject;
    public GameObject CombatScene, Hand, StoryScene;
    public Button[] TileButton;
    public Image[] TileImage;

    [Header("Sprites")]
    public Sprite PlayerSprite;
    public Sprite EmptyTileSprite;

    void Start()
    {
        UpdateInfo();
    }

    public void UpdateInfo()
    {
        TileButton[currentTile + 1].interactable = true;
        TileImage[currentTile + 1].color = new Color(1f, 1f, 0.6f, 1f);
    }

    public void SelectTile(int which)
    {
        MoveTile();

        switch (tileEvent[which])
        {
            case 0:
                CardEventObject.SetActive(true);
                break;
            case 1:
                ForgeScript.Open();
                break;
            case 2:
                Fade.StartDarken();
                CombatScript.SetEnemy(0);
                Invoke("StartCombat", 0.4f);
                break;
            case 3:
                Fade.StartDarken();
                Invoke("ContinueStory", 0.4f);
                break;
        }
    }

    public void MoveTile()
    {
        TileImage[currentTile].color = new Color(1f, 1f, 1f, 1f);
        TileImage[currentTile].sprite = EmptyTileSprite;

        currentTile++;

        TileImage[currentTile].color = new Color(0.6f, 1f, 0.6f, 1f);
        TileImage[currentTile].sprite = PlayerSprite;
        TileButton[currentTile].interactable = false;

        if (currentTile < tilesAmount)
        {
            UpdateInfo();
        }
    }

    void StartCombat()
    {
        CombatScene.SetActive(true);
        Hand.SetActive(true);
    }

    void ContinueStory()
    {
        StoryScript.NewDialogue();
        StoryScene.SetActive(true);
    }
}
