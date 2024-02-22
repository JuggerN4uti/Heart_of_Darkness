using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dialogue : MonoBehaviour
{
    public string DialogueName;
    public string[] CharacterName, CharacterDialogue;
    public Sprite[] LeftCharacterSprite, RightCharacterSprite;
    public bool[] LeftCharacterHighlight, RightCharacterHighlight;
    public int DialoguesCount;
}
