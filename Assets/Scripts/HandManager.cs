using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using DG.Tweening;
using System;
using System.ComponentModel;

public class HandManager : MonoBehaviour
{
    [Category("References")]
    [SerializeField] private Transform handArea;
    [SerializeField] private CardPileManager deckManager;
    [SerializeField] private SplineContainer splineContainer;

    [Category("Values")]
    [SerializeField] private int maxHandSize = 1;
    [SerializeField] private float cardSpacing = 150f;
    [Range(0f, 1f)][SerializeField] private float minCardSpacingOnSpline = 0.1f;
    [Range(0f, 1f)][SerializeField] private float maxCardSpacingOnSpline = 0.75f;
    [Range(0f, 1f)][SerializeField] private float hoverShiftAmount = 0.08f;
    [SerializeField] private float drawCardInterval = 0.15f;
    private List<Card> hand = new List<Card>();


    public IEnumerator FillHandWithCards()
    {
        if (hand.Count >= maxHandSize)
        {
            yield break;
        }
        int cardsToDraw = maxHandSize - hand.Count;
        for (int x = 0; x < cardsToDraw; x++)
        {
            Card newCard = deckManager.DrawCard();
            if (newCard == null)
            {
                yield break;
            }
            AddCard(newCard);
            yield return new WaitForSeconds(drawCardInterval);
        }
    }

    public Transform GetHandTransform()
    {
        return handArea;
    }

    public void UpdateCardPositions()
    { 
        if (hand.Count == 0)
        {
            return;
        }
        float cardSpacing = Mathf.Min(maxCardSpacingOnSpline / maxHandSize, minCardSpacingOnSpline);
        float firstCardPosition = 0.5f - (hand.Count - 1) * cardSpacing / 2f;
        Spline spline = splineContainer.Spline;

        for (int i = 0; i < hand.Count; i++)
        {
            float p = firstCardPosition + i * cardSpacing;
            Vector3 splinePosition = spline.EvaluatePosition(p);
            Vector3 forward = spline.EvaluateTangent(p);
            Vector3 up = -spline.EvaluateUpVector(p);

            Quaternion rotation = Quaternion.LookRotation(up, Vector3.Cross(up, forward).normalized);
            Sequence sequence = DOTween.Sequence();
            sequence.Join(hand[i].transform.DOLocalMove(splinePosition, Card.SetupTimeValue));
            sequence.Join(hand[i].transform.DOLocalRotateQuaternion(rotation, Card.SetupTimeValue));
            sequence.Play();
        }
    }

    private void AddCard(Card newCard)
    {
        hand.Add(newCard);
        newCard.OnCardStartHover += HandleCardStartHover;
        newCard.OnCardStopHover += HandleCardStopHover;
        newCard.OnCardOnCardDropped += HandleCardPositionExchange;
        UpdateCardPositions();
    }

    public void RemoveCard(Card card)
    {
        if (hand.Remove(card))
        {
            card.OnCardStartHover -= HandleCardStartHover;
            card.OnCardStopHover -= HandleCardStopHover;
            card.OnCardOnCardDropped -= HandleCardPositionExchange;
        }
        UpdateCardPositions();
    }


    private void HandleCardPositionExchange(Card targetCard, Card droppedCard)
    {
        int targetIndex = hand.IndexOf(targetCard);
        int droppedIndex = hand.IndexOf(droppedCard);
        (hand[targetIndex], hand[droppedIndex]) = (hand[droppedIndex], hand[targetIndex]);

        for (int i = 0; i < hand.Count; i++)
        {
            if (hand[i].GetState() != CardState.Dragged)
            {
                hand[i].SetSiblingOrder(i);
            }
        }
        foreach (Card card in hand)
        {
            if (card.GetState() == CardState.Dragged)
            {
                card.SetSiblingOrder(999);
            }
        }
        HandleCardStartHover(droppedCard);
    }


    private void HandleCardStartHover(Card hoveredCard)
    {
        float cardSpacing = Mathf.Min(maxCardSpacingOnSpline / maxHandSize, minCardSpacingOnSpline);
        float firstCardPosition = 0.5f - (hand.Count - 1) * cardSpacing / 2f;
        Spline spline = splineContainer.Spline;

        for (int i = 0; i < hand.Count; i++)
        {
            Card card = hand[i];
            float p = firstCardPosition + i * cardSpacing;

            if (i < hand.IndexOf(hoveredCard))
            {
                p -= hoverShiftAmount;
            }
            else if (i > hand.IndexOf(hoveredCard))
            {
                p += hoverShiftAmount;
            }

            Vector3 splinePosition = spline.EvaluatePosition(p);

            Vector3 forward = spline.EvaluateTangent(p);
            Vector3 up = -spline.EvaluateUpVector(p);

            Quaternion rotation = Quaternion.LookRotation(up, Vector3.Cross(up, forward).normalized);

            Sequence sequence = DOTween.Sequence();
            if (hand[i].GetState() == CardState.HandHover)
            {
                sequence.Join(card.transform.DOLocalRotateQuaternion(Quaternion.Euler(0f, 0f, 0f), Card.SetupTimeValue));
                sequence.Join(card.transform.DOLocalMove(splinePosition, Card.SetupTimeValue));
            }
            else
            {
                sequence.Join(card.transform.DOLocalMove(splinePosition, Card.SetupTimeValue));
                sequence.Join(card.transform.DOLocalRotateQuaternion(rotation, Card.SetupTimeValue));
            }
            sequence.Play();
        }
    }
    
    private void HandleCardStopHover()
    {
        UpdateCardPositions();
        for (int i = 0; i < hand.Count; i++)
        {
            hand[i].SetSiblingOrder(i);
        }
    }
}