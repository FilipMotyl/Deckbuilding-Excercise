using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private HandManager handManager;
    [SerializeField] private CardPileManager deckManager;
    [SerializeField] private CardPileManager discardPileManager;

    private void Start()
    {
        deckManager.InitializeDeck();
        discardPileManager.InitializeDeck();

        StartCoroutine(handManager.FillHandWithCards());
    }

    public void EndTurn()
    {
        StartCoroutine(handManager.FillHandWithCards());
    }
}
