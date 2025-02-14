using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardToHand : MonoBehaviour
{
    [SerializeField] private Image artwork;
    private CardSO cardData;
    private HandManager handManager;

    public void Initialize(CardSO cardData, HandManager handManager)
    {
        this.cardData = cardData;
        this.handManager = handManager;

        artwork.sprite = cardData.Sprite;

    }
}
