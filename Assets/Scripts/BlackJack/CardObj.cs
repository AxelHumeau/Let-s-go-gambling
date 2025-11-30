using DG.Tweening;
using UnityEngine;

public class CardObj : MonoBehaviour {

    private bool _isFlipped = true;
    private Card _card = null;

    [SerializeField] private SpriteRenderer _frontCardSpriteRenderer;
    [SerializeField] private SpriteRenderer _backCardSpriteRenderer;

    public void SetCard(Card card, bool isFlipped, int sortingOrder = 0) {
        _card = card;
        _isFlipped = isFlipped;
        if (!_isFlipped)
            transform.rotation = Quaternion.Euler(0, 0f, 0);
        else
            transform.rotation = Quaternion.Euler(0, 180f, 0);
        _frontCardSpriteRenderer.sprite = CardSpriteManager.Instance.GetCardSprite(card.GetFamilly(), card.GetRank());
        _frontCardSpriteRenderer.sortingOrder = sortingOrder;
        _backCardSpriteRenderer.sprite = CardSpriteManager.Instance.GetCardBackSprite();
        _backCardSpriteRenderer.sortingOrder = sortingOrder;
    }

    public void Flip() {
        _isFlipped = !_isFlipped;
        transform.DORotate(new(0, _isFlipped ? 180f : 0.0f, 0), 0.25f);
    }
}
