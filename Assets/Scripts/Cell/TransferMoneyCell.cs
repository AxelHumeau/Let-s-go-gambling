using UnityEngine;
using System.Collections;
using System.Linq;

public class TransfertMoneyCell : MonoBehaviour, ICellType
{
    public string GetCellName() => "Transfert money";

    public void OnStopOnCell(Cell cell, Player player)
    {
        var players = FindAnyObjectByType<TurnManager>().Players.Where(p => p != player);
        int index = Random.Range(0, players.Count());
        Player targetPlayer = players.ElementAt(index);
        int percentage = Random.Range(5, 16);
        targetPlayer.AddMoney(player.Money * percentage / 100);
        player.SubtractMoney(player.Money * percentage / 100);
    }

    public void OnPassOnCell(Cell cell, Player player)
    {
        // No action for empty cell
    }
}