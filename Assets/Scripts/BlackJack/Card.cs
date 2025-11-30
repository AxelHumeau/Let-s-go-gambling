using UnityEngine;

[System.Serializable]
public class Card {

    public enum Familly {
        Hearts,
        Diamonds,
        Clubs,
        Spades
    }

    public enum Rank {
        Two = 2,
        Three = 3,
        Four = 4,
        Five = 5,
        Six = 6,
        Seven = 7,
        Eight = 8,
        Nine = 9,
        Ten = 10,
        Jack = 11,
        Queen = 12,
        King = 13,
        Ace = 14
    }

    private readonly Familly _familly;
    private readonly Rank _rank;
    private Sprite _sprite;

    public Card(Familly familly, Rank rank) {
        _familly = familly;
        _rank = rank;
    }

    public int GetBlackjackValue(bool aceAsEleven = true) {
        if (_rank == Rank.Ace)
            return aceAsEleven ? 11 : 1;
        else if ((int)_rank >= 11 && (int)_rank <= 13)
            return 10;
        else
            return (int)_rank;
    }

    public Familly GetFamilly() { return _familly; }

    public Rank GetRank() { return _rank; }

    public Sprite GetSprite() { return _sprite; }

    public void SetSprite(Sprite sprite) { _sprite = sprite; }

    public bool IsAce() { return _rank == Rank.Ace; }
}
