using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class BlackjackCell : MonoBehaviour, ICellType
{
    public string GetCellName() => "Blackjack cell";

    public void OnStopOnCell(Cell cell, Player player)
    {
        int amount = Mathf.Min(100, player.Money);
        BlackJackManager.betAmount = amount;
        player.SubtractMoney(amount);
        FindAnyObjectByType<TurnManager>().PauseForCellAction(cell);
        SceneManager.LoadScene("BlackJack", LoadSceneMode.Additive);
        BlackJackManager.OnGameEnd = (_) =>
        {
            SceneManager.UnloadSceneAsync("BlackJack");
            player.AddMoney(BlackJackManager.betAmount);
            FindAnyObjectByType<TurnManager>().CompleteCellAction(cell);
        };
    }

    public void OnPassOnCell(Cell cell, Player player)
    {
        // No action for empty cell
    }
}