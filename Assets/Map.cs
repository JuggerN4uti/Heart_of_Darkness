using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Map : MonoBehaviour
{
    [Header("Scipts")]
    public MapTile[] TopTiles;
    public MapTile[] MidTiles, BotTiles;
    public Player PlayerScript;
    public Combat CombatScript;
    public SceneChange Fade;
    public CardPick CardChoice;

    [Header("Stats")]
    public int[] tileEvent;
    public int currentTile, currentRow, tilesAmount;
    public float danger;
    int roll;

    [Header("UI")]
    public RectTransform SlidingMap;
    public GameObject CombatScene, Hand, CardPickObject;
    public Slider slider;
    public Image[] TileImage;
    public Image LastImage;
    public Button[] TileButton;
    public Button LastButton;

    [Header("Sprite")]
    public Sprite[] TileSprites;
    public Sprite EmptyTileSprite, PlayerSprite;

    void Start()
    {
        SetTileEvents();
    }

    void UpdateInfo()
    {
        if (currentTile == 0)
        {
            for (int i = 1; i < 4; i++)
            {
                TileButton[i].interactable = true;
            }
        }
        else if (currentTile == tilesAmount)
        {
            LastButton.interactable = true;
            LastImage.color = new Color(1f, 1f, 0.6f, 1f);
        }
        else
        {
            switch (currentRow)
            {
                case 0:
                    TileButton[currentTile * 3 + 1].interactable = true;
                    TileImage[currentTile * 3 + 1].color = new Color(1f, 1f, 0.6f, 1f);
                    if (TopTiles[currentTile - 1].path[0])
                    {
                        TileButton[currentTile * 3 + 2].interactable = true;
                        TileImage[currentTile * 3 + 2].color = new Color(1f, 1f, 0.6f, 1f);
                    }
                    break;
                case 1:
                    TileButton[currentTile * 3 + 2].interactable = true;
                    TileImage[currentTile * 3 + 2].color = new Color(1f, 1f, 0.6f, 1f);
                    if (MidTiles[currentTile - 1].path[0])
                    {
                        TileButton[currentTile * 3 + 1].interactable = true;
                        TileImage[currentTile * 3 + 1].color = new Color(1f, 1f, 0.6f, 1f);
                    }
                    if (MidTiles[currentTile - 1].path[1])
                    {
                        TileButton[currentTile * 3 + 3].interactable = true;
                        TileImage[currentTile * 3 + 3].color = new Color(1f, 1f, 0.6f, 1f);
                    }
                    break;
                case 2:
                    TileButton[currentTile * 3 + 3].interactable = true;
                    TileImage[currentTile * 3 + 3].color = new Color(1f, 1f, 0.6f, 1f);
                    if (BotTiles[currentTile - 1].path[0])
                    {
                        TileButton[currentTile * 3 + 2].interactable = true;
                        TileImage[currentTile * 3 + 2].color = new Color(1f, 1f, 0.6f, 1f);
                    }
                    break;
            }
        }
    }

    public void SlideMap()
    {
        SlidingMap.position = new Vector2(-slider.value, 0f);
    }

    public void SelectTile(int row)
    {
        MoveTile(row);

        switch (tileEvent[currentTile * 3 + currentRow - 2])
        {
            case 0:
                CardChoice.RollCards();
                CardPickObject.SetActive(true);
                break;
            case 1:
                if (PlayerScript.DeckScript.CommonCardsInDeck() > 0)
                    PlayerScript.DeckScript.ShowCardsToForge();
                break;
            case 2:
                Fade.StartDarken();
                CombatScript.SetEnemy(SetEnemies());
                Invoke("StartCombat", 0.4f);
                break;
        }
    }

    public void SelectLastTile()
    {
        for (int i = 0; i < 3; i++)
        {
            TileImage[currentTile * 3 + i - 2].color = new Color(1f, 1f, 1f, 1f);
        }
        TileImage[currentTile * 3 + currentRow - 2].sprite = EmptyTileSprite;

        LastButton.interactable = false;
        LastImage.sprite = PlayerSprite;
        LastImage.color = new Color(0.6f, 1f, 0.6f, 1f);

        Fade.StartDarken();
        CombatScript.SetEnemy(3);
        Invoke("StartCombat", 0.4f);
    }

    int SetEnemies()
    {
        danger += 0.55f + danger * 0.02f;
        if (danger < 1.6f)
            return 0;
        else
        {
            return Random.Range(1, 3);
        }
    }

    void MoveTile(int row)
    {
        danger += 0.35f;

        if (currentTile == 0)
        {
            TileImage[0].color = new Color(1f, 1f, 1f, 1f);
            TileImage[0].sprite = EmptyTileSprite;
        }
        else
        {
            for (int i = 0; i < 3; i++)
            {
                TileImage[currentTile * 3 + i - 2].color = new Color(1f, 1f, 1f, 1f);
            }
            TileImage[currentTile * 3 + currentRow - 2].sprite = EmptyTileSprite;
        }

        currentTile++;
        currentRow = row;

        for (int i = 0; i < 3; i++)
        {
            TileButton[currentTile * 3 + i - 2].interactable = false;
            TileImage[currentTile * 3 + i - 2].color = new Color(1f, 1f, 1f, 1f);
        }
        TileImage[currentTile * 3 + currentRow - 2].color = new Color(0.6f, 1f, 0.6f, 1f);
        TileImage[currentTile * 3 + currentRow - 2].sprite = PlayerSprite;

        UpdateInfo();
    }

    void StartCombat()
    {
        CombatScene.SetActive(true);
        Hand.SetActive(true);
    }

    public void SetTileEvents()
    {
        for (int i = 1; i < TileImage.Length; i++)
        {
            roll = Random.Range(0, TileSprites.Length);
            TileImage[i].sprite = TileSprites[roll];
            tileEvent[i] = roll;
        }
        UpdateInfo();
    }
}
