using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Story : MonoBehaviour
{
    [Header("Scripts")]
    public SceneChange Fade;
    public Map MapScript;
    public int StoryChapter, line;
    public Dialogue[] dialogues;
    public Dialogue CurrentDialogue;

    [Header("UI")]
    public GameObject StoryScene;
    public GameObject PlayerHUD, ArmySelectScene, TutorialMapScene, MapScene;
    public Image LeftCharacter;
    public Image RightCharacter;
    public TMPro.TextMeshProUGUI CharacterName, CharacterDialogue;

    void Start()
    {
        if (StoryChapter == 0)
            NewDialogue();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (CurrentDialogue.DialoguesCount == line)
            {
                Fade.StartDarken();
                Invoke("Proceed", 0.4f);
            }
            else NextLine();
        }
    }

    public void NewDialogue()
    {
        CurrentDialogue = dialogues[StoryChapter];
        line = 0;
        StoryChapter++;

        NextLine();
    }

    void NextLine()
    {
        LeftCharacter.sprite = CurrentDialogue.LeftCharacterSprite[line];
        if (CurrentDialogue.LeftCharacterHighlight[line])
            LeftCharacter.color = new Color(1f, 1f, 1f, 1f);
        else LeftCharacter.color = new Color(0.3f, 0.3f, 0.3f, 1f);

        RightCharacter.sprite = CurrentDialogue.RightCharacterSprite[line];
        if (CurrentDialogue.RightCharacterHighlight[line])
            RightCharacter.color = new Color(1f, 1f, 1f, 1f);
        else RightCharacter.color = new Color(0.3f, 0.3f, 0.3f, 1f);

        CharacterName.text = CurrentDialogue.CharacterName[line];
        CharacterDialogue.text = CurrentDialogue.CharacterDialogue[line];

        line++;
    }

    void Proceed()
    {
        switch (StoryChapter)
        {
            case 1:
                StoryScene.SetActive(false);
                ArmySelectScene.SetActive(true);
                break;
            case 2:
                StoryScene.SetActive(false);
                PlayerHUD.SetActive(true);
                TutorialMapScene.SetActive(true);
                break;
            case 3:
                StoryScene.SetActive(false);
                TutorialMapScene.SetActive(false);
                MapScript.SetTileEvents();
                MapScript.danger += 0.25f;
                MapScene.SetActive(true);
                break;
            case 4:
                StoryScene.SetActive(false);
                MapScript.StartCombat();
                break;
            case 5:
                StoryScene.SetActive(false);
                break;
        }
    }
}
