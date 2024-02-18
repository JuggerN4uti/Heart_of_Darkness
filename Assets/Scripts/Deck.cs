using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    [Header("Cards")]
    public int[] CardID;
    public int[] CardLevel;
    public int cardsInDeck;

    public void AddACard(int which, int level)
    {
        CardID[cardsInDeck] = which;
        CardLevel[cardsInDeck] = level;
        cardsInDeck++;
    }
}
