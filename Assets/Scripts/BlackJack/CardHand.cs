using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Splines;
using DG.Tweening;

public class CardHand : MonoBehaviour {
    private readonly List<Card> _cards = new();

    [SerializeField] private int _maxCardCount = 11;
    [SerializeField] private SplineContainer _splineContainer;
    [SerializeField] private GameObject _cardPrefab;
    [SerializeField] private Transform _spawnPoint;
    private List<GameObject> _cardObjs = new();

    public void AddCard(Card card, bool flip = true, int sortingOrder = 0) {
        GameObject cardObj = Instantiate(_cardPrefab, _spawnPoint.position, Quaternion.identity, transform);
        cardObj.transform.rotation = Quaternion.Euler(0, 0, 180.0f);
        CardObj cardObjComponent = cardObj.GetComponent<CardObj>();
        cardObjComponent.SetCard(card, true, sortingOrder);
        if (flip)
            cardObjComponent.Flip();
        _cardObjs.Add(cardObj);
        _cards.Add(card);
        UpdateCardPositions();
    }

    private void UpdateCardPositions() {
        if (_cardObjs.Count == 0)
            return;
        float cardSpacing = 1.0f / _maxCardCount;
        float firstCardPosition = 0.5f - (_cardObjs.Count - 1 ) * cardSpacing / 2.0f;
        Spline spline = _splineContainer.Spline;
        for (int i = 0; i < _cardObjs.Count; i++) {
            float cardPosition = firstCardPosition + i * cardSpacing;
            Vector3 splinePosition = spline.EvaluatePosition(cardPosition);
            _cardObjs[i].transform.DOMove(transform.position + splinePosition, 0.25f);
        }
    }

    public int GetValue() {
        int value = 0;
        int aceCount = 0;

        foreach (Card card in _cards) {
            if (card.GetRank() == Card.Rank.Ace) {
                aceCount++;
                value += 11;
            }
            else
                value += card.GetBlackjackValue();
        }
        while (value > 21 && aceCount > 0) {
            value -= 10;
            aceCount--;
        }
        return value;
    }

    public int GetCardCount() { return _cards.Count; }

    public bool IsBust() { return GetValue() > 21; }

    public bool IsBlackjack() { return _cards.Count == 2 && GetValue() == 21; }

    public void Clear() {
        _cards.Clear();
        foreach (var cardObj in _cardObjs) {
            Destroy(cardObj);
        }
        _cardObjs.Clear();
    }

    public List<Card> GetCards() { return new List<Card>(_cards); }
}
