using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class CardSpriteEntry {
    public Card.Familly familly;
    public Card.Rank rank;
    public Sprite sprite;
}

public class CardSpriteManager : MonoBehaviour {

    [Header("Card Sprites")]
    public List<CardSpriteEntry> cardSprites = new();

    [Header("Back Card Sprite")]
    public Sprite cardBackSprite;

    private static CardSpriteManager _instance;
    private Dictionary<(Card.Familly, Card.Rank), Sprite> _spriteCache;

    public static CardSpriteManager Instance {
        get {
            if (_instance == null) {
                _instance = new();
            }
            return _instance;
        }
    }

    void Awake() {
        if (_instance == null) {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        } else if (_instance != this) {
            Destroy(gameObject);
        }

        BuildSpriteCache();
    }

    private void BuildSpriteCache() {
        _spriteCache = new Dictionary<(Card.Familly, Card.Rank), Sprite>();
        foreach (var entry in cardSprites) {
            if (entry.sprite != null) {
                _spriteCache[(entry.familly, entry.rank)] = entry.sprite;
            }
        }
    }

    public Sprite GetCardSprite(Card.Familly familly, Card.Rank rank) {
        if (_spriteCache != null && _spriteCache.TryGetValue((familly, rank), out Sprite sprite)) {
            return sprite;
        }
        return null;
    }

    public Sprite GetCardBackSprite() {
        return cardBackSprite;
    }
}
