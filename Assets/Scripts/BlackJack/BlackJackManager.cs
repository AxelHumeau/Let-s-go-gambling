using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using System;

public class BlackJackManager : MonoBehaviour
{
    private CardDeck _deck;
    public CardHand playerHand;
    public CardHand dealerHand;

    [Header("UI Elements")]
    public Button hitButton;
    public Button standButton;
    public TextMeshProUGUI resultText;

    private bool _gameInProgress = false;
    private bool _dealerTurn = false;

    bool _hasPlayerPlayed = false;

    public static event Action<string> OnGameEnd;
    public static event Action OnGameStart;

    void Start() {
        InitializeGame();
        SetupUI();
    }

    void SetupUI() {
        if (hitButton != null)
            hitButton.onClick.AddListener(PlayerHit);
        if (standButton != null)
            standButton.onClick.AddListener(PlayerStand);
    }

    public void InitializeGame() {
        if (_deck == null) {
            GameObject deckObj = new("Deck");
            deckObj.transform.SetParent(transform);
            _deck = deckObj.AddComponent<CardDeck>();
        }
        StartCoroutine(StartNewGame());
    }

    public IEnumerator StartNewGame() {
        playerHand.Clear();
        dealerHand.Clear();
        _deck.Shuffle();

        _gameInProgress = true;
        _dealerTurn = false;
        _hasPlayerPlayed = false;

        if (resultText != null)
            resultText.text = "";

        SetButtonsInteractable(false);

        playerHand.AddCard(_deck.DrawCard(), true, playerHand.GetCardCount());
        yield return new WaitForSeconds(0.5f);

        dealerHand.AddCard(_deck.DrawCard(), true, dealerHand.GetCardCount());
        yield return new WaitForSeconds(0.5f);

        playerHand.AddCard(_deck.DrawCard(), true, playerHand.GetCardCount());
        yield return new WaitForSeconds(0.5f);

        dealerHand.AddCard(_deck.DrawCard(), false, dealerHand.GetCardCount());

        OnGameStart?.Invoke();

        if (playerHand.IsBlackjack())
        {
            yield return StartCoroutine(RevealDealerHiddenCard());
            if (dealerHand.IsBlackjack())
            {
                EndGame("Push! Both have Blackjack!");
            }
            else
            {
                EndGame("Blackjack! You win!");
            }
            yield break;
        }

        SetButtonsInteractable(true);

        StartCoroutine(PlayerTurn());
    }

    IEnumerator PlayerTurn() {
        _hasPlayerPlayed = false;
        yield return new WaitUntil(() => _hasPlayerPlayed);
    }

    public void PlayerHit() {
        if (!_gameInProgress || _dealerTurn)
            return;

        playerHand.AddCard(_deck.DrawCard(), true, playerHand.GetCardCount());

        int playerValue = playerHand.GetValue();

        if (playerValue > 21)
        {
            _hasPlayerPlayed = true;
            EndGame("Bust! You exceeded 21. You lose.");
        }
        else if (playerValue == 21)
        {
            _hasPlayerPlayed = true;
            PlayerStand();
        }
    }

    public void PlayerStand() {
        if (!_gameInProgress || _dealerTurn)
            return;

        _hasPlayerPlayed = true;
        _dealerTurn = true;
        StartCoroutine(DealerPlay());
    }

    IEnumerator DealerPlay() {
        SetButtonsInteractable(false);

        yield return StartCoroutine(RevealDealerHiddenCard());
        yield return new WaitForSeconds(0.5f);

        while (dealerHand.GetValue() < 17) {
            dealerHand.AddCard(_deck.DrawCard(), true, dealerHand.GetCardCount());
            yield return new WaitForSeconds(1f);
        }

        DetermineWinner();
    }

    IEnumerator RevealDealerHiddenCard() {
        if (dealerHand.transform.childCount > 0)
        {
            Transform lastCard = dealerHand.transform.GetChild(dealerHand.transform.childCount - 1);
            CardObj cardObj = lastCard.GetComponent<CardObj>();
            if (cardObj != null)
            {
                cardObj.Flip();
            }
        }
        yield return new WaitForSeconds(0.5f);
    }

    void DetermineWinner()
    {
        int playerValue = playerHand.GetValue();
        int dealerValue = dealerHand.GetValue();

        if (dealerValue > 21) {
            EndGame($"Dealer busts! You win with {playerValue}!");
        }
        else if (playerValue > dealerValue) {
            EndGame($"You win! {playerValue} vs {dealerValue}");
        }
        else if (dealerValue > playerValue) {
            EndGame($"Dealer wins! {dealerValue} vs {playerValue}");
        }
        else {
            EndGame($"Push! Tie at {playerValue}");
        }
    }

    void EndGame(string message) {
        _gameInProgress = false;
        _dealerTurn = false;

        if (resultText != null)
            resultText.text = message;

        Debug.Log(message);

        SetButtonsInteractable(false);

        OnGameEnd?.Invoke(message);
    }

    void SetButtonsInteractable(bool interactable) {
        if (hitButton != null)
            hitButton.interactable = interactable;
        if (standButton != null)
            standButton.interactable = interactable;
    }
}
