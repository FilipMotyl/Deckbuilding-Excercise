using DG.Tweening;
using System;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum CardState
{
    Moving,
    Hand,
    HandHover,
    Dragged,
    Board,
}

public class Card : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IDropHandler
{
    public static float SetupTimeValue = 0.25f;
    public static bool IsCardDragged = false;

    public event Action<Card> OnCardStartHover;
    public event Action<Card, Card> OnCardOnCardDropped;
    public event Action OnCardStopHover;

    [Category("References")]
    [SerializeField] private Image image;
    [SerializeField] private RectTransform rectTransform;

    [Category("Values")]
    [SerializeField] private float hoverScaleMultiplier = 1.5f;
    [SerializeField] private Vector3 targetScale = new Vector3(0.25f, 0.25f, 0.25f);
    [SerializeField] private float dragSpeed = 20f;

    [Category("Debug Only")]
    [SerializeField] private CardState State;

    private CardSO cardData;

    public void Initialize(CardSO cardData)
    {
        this.cardData = cardData;
        image.sprite = cardData.Sprite;
        SetState(CardState.Moving);
        transform.DOScale(targetScale, Card.SetupTimeValue).OnComplete(() => SetState(CardState.Hand));
    }

    #region Drag & Drop
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (IsCardDragged == false & State == CardState.HandHover)
        {
            image.raycastTarget = false;
            SetState(CardState.Dragged);
            SetIsAnyCardDragged(true);

        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (State == CardState.Dragged)
        {
            transform.DOMove(Input.mousePosition, 0.1f).SetEase(Ease.OutQuad);
        }

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (State == CardState.Dragged)
        {
            image.raycastTarget = true;

            if (RectTransformUtility.RectangleContainsScreenPoint(rectTransform, Input.mousePosition))
            {
                SetState(CardState.Hand);
                OnPointerEnter(eventData);
            }
            else
            {
                SetState(CardState.Hand);
                transform.DOScale(targetScale, SetupTimeValue);
            }
            SetIsAnyCardDragged(false);
        }
    }

    #endregion



    #region Hovering

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (IsCardDragged == false && State == CardState.Hand)
        {
            transform.DOScale(targetScale * hoverScaleMultiplier, SetupTimeValue);
            SetState(CardState.HandHover);

            OnCardStartHover?.Invoke(this);
            rectTransform.SetSiblingIndex(999);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (IsCardDragged == false && (State == CardState.HandHover))
        {
            SetState(CardState.Moving);
            transform.DOScale(targetScale, SetupTimeValue).OnComplete(() => SetState(CardState.Hand));
            OnCardStopHover?.Invoke();
        }
    }

    #endregion

    public void OnDrop(PointerEventData eventData)
    {
        Card droppedCard;
        if (eventData.pointerDrag.gameObject == this) return;
        if (eventData.pointerDrag.TryGetComponent<Card>(out droppedCard))
        {
            OnCardOnCardDropped?.Invoke(this, droppedCard);
            transform.DOScale(targetScale, SetupTimeValue);
        }
    }
    public CardSO GetCardData()
    {
        return cardData;
    }
    public Image GetImage()
    {
        return image;
    }

    public CardState GetState()
    {
        return State;
    }

    public void SetState(CardState newState)
    {
        State = newState;
    }

    public void SetSiblingOrder(int order)
    {
        rectTransform.SetSiblingIndex(order);
    }

    public static void SetIsAnyCardDragged(bool b)
    {
        IsCardDragged = b;
    }
}