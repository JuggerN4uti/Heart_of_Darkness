using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Map : MonoBehaviour
{
    [Header("Scipts")]
    public MapTile[] TopTiles;
    public MapTile[] MidTiles, BotTiles;
    public Story StoryScript;
    public Player PlayerScript;
    public Combat CombatScript;
    public ForgeChoice ForgeScript;
    public CampChoice CampScript;
    public MerchantChoice MerchantScript;
    public ItemPick ItemPickScript;
    public SceneChange Fade;
    public EnemiesLibrary Library;

    [Header("Stats")]
    public int[] tileEvent;
    public int[] EventCooldown, EventsCooldowns;
    public int currentTile, currentRow, tilesAmount, treasureTile;
    public float danger, eliteDanger, experience;
    int roll, tempi;
    float temp;
    bool viable;

    [Header("UI")]
    public RectTransform SlidingMap;
    public GameObject CombatScene, Hand, CardEventObject, StoryScene;
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
                CardEventObject.SetActive(true);
                break;
            case 1:
                ForgeScript.Open();
                break;
            case 2:
                Fade.StartDarken();
                SetEnemies();
                Invoke("StartCombat", 0.4f);
                break;
            case 3:
                CampScript.Open();
                break;
            case 4:
                MerchantScript.Open();
                break;
            case 5:
                Fade.StartDarken();
                SetElite();
                Invoke("StartCombat", 0.4f);
                break;
            case 6:
                ItemPickScript.RollItems();
                break;
        }
    }

    public void SelectLastTile()
    {
        if (StoryScript.StoryChapter == 3)
        {
            Fade.StartDarken();
            CombatScript.SetEnemy(6, 0);
            Invoke("ContinueStory", 0.4f);
        }
        else
        {
            Fade.StartDarken();
            SetBoss();
            Invoke("StartCombat", 0.4f);
        }

        for (int i = 0; i < 3; i++)
        {
            TileImage[currentTile * 3 + i - 2].color = new Color(1f, 1f, 1f, 1f);
        }
        TileImage[currentTile * 3 + currentRow - 2].sprite = EmptyTileSprite;

        LastButton.interactable = false;
        LastImage.sprite = PlayerSprite;
        LastImage.color = new Color(0.6f, 1f, 0.6f, 1f);

       
        //Invoke("StartCombat", 0.4f);
    }

    void ContinueStory()
    {
        StoryScript.NewDialogue();
        StoryScene.SetActive(true);
    }

    void SetEnemies()
    {
        danger += 0.56f + danger * 0.02f;
        CombatScript.elite = false;
        CombatScript.boss = false;
        if (danger < 1.6f)
            CombatScript.SetEnemy(0, 0);
        else
        {
            tempi = 0;
            temp = danger;
            while (temp > 3.25f)
            {
                temp -= 1.72f + tempi * 0.34f;
                tempi++;
            }
            roll = Library.BasicRoll();
            CombatScript.SetEnemy(roll, tempi);
        }
    }

    void SetElite()
    {
        danger += 0.6f + danger * 0.02f;
        eliteDanger += 0.25f + eliteDanger * 0.01f;
        tempi = 0;
        temp = danger + eliteDanger;
        while (temp > 5.25f)
        {
            temp -= 2.72f + tempi * 0.82f;
            tempi++;
        }
        roll = Library.EliteRoll();
        CombatScript.elite = true;
        CombatScript.boss = false;
        CombatScript.SetEnemy(roll, tempi);
    }

    void SetBoss()
    {
        roll = Library.BossRoll();
        CombatScript.elite = false;
        CombatScript.boss = true;
        CombatScript.SetEnemy(roll, 0);
    }

    void MoveTile(int row)
    {
        danger += 0.34f + 0.01f * currentTile;

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

    public void StartCombat()
    {
        CombatScene.SetActive(true);
        CombatScript.mapDanger = danger;
        Hand.SetActive(true);
    }

    public void SetTileEvents()
    {
        experience = 0f;
        for (int i = 1; i < TileImage.Length; i++)
        {
            viable = false;
            do
            {
                roll = Random.Range(0, TileSprites.Length - 1);
                EventCooldown[roll]--;
                if (EventCooldown[roll] == 0)
                    viable = true;
            } while (!viable);

            EventCooldown[roll] += EventsCooldowns[roll];
            if (roll > 2)
                EventsCooldowns[roll]++;

            TileImage[i].sprite = TileSprites[roll];
            tileEvent[i] = roll;
        }
        for (int i = 1; i < 4; i++)
        {
            TileImage[treasureTile * 3 + i - 3].sprite = TileSprites[6];
            tileEvent[treasureTile * 3 + i - 3] = 6;
        }
        for (int i = 1; i < 4; i++)
        {
            TileImage[tilesAmount * 3 + i - 3].sprite = TileSprites[3];
            tileEvent[tilesAmount * 3 + i - 3] = 3;
        }
        UpdateInfo();
    }
}
