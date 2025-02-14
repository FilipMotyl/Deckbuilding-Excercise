using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DeckSO", menuName = "Card Game/Deck")]
public class DeckSO : ScriptableObject
{
    public List<CardSO> allCards;
}