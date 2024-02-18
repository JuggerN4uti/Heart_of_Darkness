using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FirstMap : MonoBehaviour
{
    [Header("Scripts")]
    public Player PlayerScript;
    public SceneChange Fade;
    public CardPick CardChoice;

    [Header("Stats")]
    public int[] tileEvent; // 0 - Card Pick, 1 - Forge, 2 - Threat, 3 - Watchtower
    public int tilesAmount, currentTile;

    [Header("UI")]
    public GameObject CardPickObject;
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
        //Fade.StartDarken();
        MoveTile();

        switch (tileEvent[which])
        {
            case 0:
                CardChoice.RollCards();
                CardPickObject.SetActive(true);
                break;
            case 1:
                if (PlayerScript.DeckScript.CommonCardsInDeck() > 0)
                    PlayerScript.DeckScript.ShowCardsToForge();
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
}
