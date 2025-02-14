using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class Board : MonoBehaviour, IDropHandler
{
    [SerializeField] private DiscardPileManager discardPileManager;
    [SerializeField] private HandManager handManager;

    public void OnDrop(PointerEventData eventData)
    {
        Card droppedCard;
        if (eventData.pointerDrag.TryGetComponent<Card>(out droppedCard) && droppedCard.GetState() == CardState.Dragged)
        {
            handManager.RemoveCard(droppedCard);
            droppedCard.transform.SetParent(this.transform, true);
            droppedCard.SetState(CardState.Board);

            Card.SetIsAnyCardDragged(false);
            droppedCard.GetImage().raycastTarget = true;

            Sequence sequence = DOTween.Sequence();
            sequence.Join(droppedCard.transform.DOScale(Vector3.zero, Card.SetupTimeValue));
            sequence.Join(droppedCard.transform.DOMove(discardPileManager.transform.position, Card.SetupTimeValue));
            sequence.OnComplete(() =>
            {
                discardPileManager.AddCard(droppedCard.GetCardData());
                Destroy(droppedCard);
            });
            sequence.Play();
        }
        
    }
}