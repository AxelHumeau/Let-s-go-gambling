using UnityEngine;
using System.Collections.Generic;
using System;

public class CardDeck : MonoBehaviour {

    private readonly List<Card> _cards = new();
    private int _currentCardIndex = 0;

    void Awake() { InitializeDeck(); }

    public void InitializeDeck() {
        _cards.Clear();

        foreach (Card.Familly familly in Enum.GetValues(typeof(Card.Familly))) {
            foreach (Card.Rank rank in Enum.GetValues(typeof(Card.Rank)))
                _cards.Add(new Card(familly, rank));
        }
        Shuffle();
    }

    public void Shuffle() {
        _currentCardIndex = 0;

        for (int i = _cards.Count - 1; i > 0; i--) {
            int j = UnityEngine.Random.Range(0, i + 1);
            (_cards[j], _cards[i]) = (_cards[i], _cards[j]);
        }
    }

    public Card DrawCard() {
        if (_currentCardIndex >= _cards.Count)
            Shuffle();

        Card card = _cards[_currentCardIndex];
        _currentCardIndex++;
        return card;
    }

    public int CardsRemaining() { return _cards.Count - _currentCardIndex; }

    public void ResetDeck() { InitializeDeck(); }
}
