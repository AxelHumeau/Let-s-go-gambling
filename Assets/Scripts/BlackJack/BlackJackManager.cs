using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using System;

public class BlackJackManager : MonoBehaviour
{
    public static int betAmount = 0;
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

    public static Action<string> OnGameEnd;
    public static event Action OnGameStart;

    void Start()
    {
        InitializeGame();
        SetupUI();
    }

    void SetupUI()
    {
        if (hitButton != null)
            hitButton.onClick.AddListener(PlayerHit);
        if (standButton != null)
            standButton.onClick.AddListener(PlayerStand);
    }

    public void InitializeGame()
    {
        if (_deck == null)
        {
            GameObject deckObj = new("Deck");
            deckObj.transform.SetParent(transform);
            _deck = deckObj.AddComponent<CardDeck>();
        }
        StartCoroutine(StartNewGame());
    }

    public IEnumerator StartNewGame()
    {
        playerHand.Clear();
        dealerHand.Clear();
        _deck.Shuffle();

        _gameInProgress = true;
        _dealerTurn = false;
        _hasPlayerPlayed = false;

        if (resultText != null)
            resultText.text = "";

        SetButtonsInteractable(false);

        playerHand.AddCard(_deck.DrawCard(), true, playerHand.GetCardCount() + 10);
        yield return new WaitForSeconds(0.5f);

        dealerHand.AddCard(_deck.DrawCard(), true, dealerHand.GetCardCount() + 10);
        yield return new WaitForSeconds(0.5f);

        playerHand.AddCard(_deck.DrawCard(), true, playerHand.GetCardCount() + 10);
        yield return new WaitForSeconds(0.5f);

        dealerHand.AddCard(_deck.DrawCard(), false, dealerHand.GetCardCount() + 10);

        OnGameStart?.Invoke();

        if (playerHand.IsBlackjack())
        {
            yield return StartCoroutine(RevealDealerHiddenCard());
            if (dealerHand.IsBlackjack())
            {
                EndGame("Push! Both have Blackjack!");
                betAmount = 0;
            }
            else
            {
                EndGame("Blackjack! You win!");
                betAmount = (int)(betAmount * 2.5);
            }
            yield break;
        }

        SetButtonsInteractable(true);

        StartCoroutine(PlayerTurn());
    }

    IEnumerator PlayerTurn()
    {
        _hasPlayerPlayed = false;
        yield return new WaitUntil(() => _hasPlayerPlayed);
    }

    public void PlayerHit()
    {
        if (!_gameInProgress || _dealerTurn)
            return;

        playerHand.AddCard(_deck.DrawCard(), true, playerHand.GetCardCount() + 10);

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

    public void PlayerStand()
    {
        if (!_gameInProgress || _dealerTurn)
            return;

        _hasPlayerPlayed = true;
        _dealerTurn = true;
        StartCoroutine(DealerPlay());
    }

    IEnumerator DealerPlay()
    {
        SetButtonsInteractable(false);

        yield return StartCoroutine(RevealDealerHiddenCard());
        yield return new WaitForSeconds(0.5f);

        while (dealerHand.GetValue() < 17)
        {
            dealerHand.AddCard(_deck.DrawCard(), true, dealerHand.GetCardCount() + 10);
            yield return new WaitForSeconds(1f);
        }

        DetermineWinner();
    }

    IEnumerator RevealDealerHiddenCard()
    {
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

        if (playerValue == 21 && dealerValue < 21)
        {
            EndGame("Blackjack! You win!");
            betAmount = (int)(betAmount * 2.5);
        }
        else if (playerValue == 21 && dealerValue == 21)
        {
            EndGame("Push! Both have Blackjack!");
            betAmount = 0;
        }
        else if (playerValue > 21)
        {
            EndGame($"Bust! You exceeded 21. Dealer wins with {dealerValue}.");
            betAmount = 0;
        }
        else if (dealerValue > 21)
        {
            EndGame($"Dealer busts! You win with {playerValue}!");
            betAmount *= 2;
        }
        else if (playerValue > dealerValue)
        {
            EndGame($"You win! {playerValue} vs {dealerValue}");
            betAmount *= 2;
        }
        else if (dealerValue > playerValue)
        {
            EndGame($"Dealer wins! {dealerValue} vs {playerValue}");
            betAmount = 0;
        }
        else
        {
            EndGame($"Push! Tie at {playerValue}");
        }
    }

    void EndGame(string message)
    {
        _gameInProgress = false;
        _dealerTurn = false;

        if (resultText != null)
            resultText.text = message;

        Debug.Log(message);

        SetButtonsInteractable(false);

        StartCoroutine(DelayBeforeEnd(5f, message));
    }

    void SetButtonsInteractable(bool interactable)
    {
        if (hitButton != null)
            hitButton.interactable = interactable;
        if (standButton != null)
            standButton.interactable = interactable;
    }

    IEnumerator DelayBeforeEnd(float delay, string message)
    {
        yield return new WaitForSeconds(delay);
        OnGameEnd?.Invoke(message);
    }
}
