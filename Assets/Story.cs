using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Story : MonoBehaviour
{
    public int StoryChapter, line;
    public Dialogue[] dialogues;
    public Dialogue CurrentDialogue;

    [Header("UI")]
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
                CharacterDialogue.text = "nigga";
            else NextLine();
        }
    }

    void NewDialogue()
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
}
