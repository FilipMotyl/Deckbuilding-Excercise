using UnityEngine;

public class DiscardPileManager : CardPileManager
{
    [SerializeField] private CardPileManager deckManager;

    public void MoveCardsToDeck()
    {
        for (int i = 0; i < deck.Count; i++)
        {
            deckManager.AddCard(deck[i]);
            deck.RemoveAt(0);
        }
    }
}
