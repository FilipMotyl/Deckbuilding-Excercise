using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private HandManager handManager;
    [SerializeField] private CardPileManager deckManager;
    [SerializeField] private CardPileManager discardPileManager;
    [SerializeField] private TextMeshProUGUI RoundText;
    private int currentRound = 1;

    private void Start()
    {
        RoundText.text = "Turn: " + currentRound;
        deckManager.InitializeDeck();
        discardPileManager.InitializeDeck();

        StartCoroutine(handManager.FillHandWithCards());
    }

    public void EndTurn()
    {
        currentRound++;
        RoundText.text = "Turn: " + currentRound;
        StartCoroutine(handManager.FillHandWithCards());
    }
}
