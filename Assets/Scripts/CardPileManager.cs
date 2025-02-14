using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEngine;

public class CardPileManager : MonoBehaviour
{
    [Category("References")]
    [SerializeField] private TextMeshProUGUI deckText;
    [SerializeField] private DeckSO deckData;
    [SerializeField] private Card cardPrefab;
    [SerializeField] private HandManager handManager;
    protected List<CardSO> deck;

    public void InitializeDeck()
    {
        
        if (deckData != null)
        {
            deck = new List<CardSO>(deckData.allCards);
            ShuffleDeck();
        }
        else
        {
            deck = new List<CardSO>();
        }
        UpdateDeckInfo();
    }

    public void ShuffleDeck()
    {
        for (int i = deck.Count - 1; i > 0; i--)
        {
            int rand = Random.Range(0, i + 1);
            (deck[i], deck[rand]) = (deck[rand], deck[i]);
        }
    }

    public void AddCard(CardSO card)
    {
        deck.Add(card);
        UpdateDeckInfo();
    }

    public Card DrawCard()
    {
        if (deck.Count == 0)
        {
            return null;
        }

        CardSO drawnCardData = deck[0];

        Card drawnCard = Instantiate(cardPrefab, transform.position, transform.rotation, handManager.GetHandTransform());
        drawnCard.Initialize(drawnCardData);
        deck.RemoveAt(0);
        UpdateDeckInfo();
        return drawnCard;
    }

    private void UpdateDeckInfo()
    {
        deckText.text = "Cards: " + deck.Count;
    }
}