using UnityEngine;
using System.Collections;
using System.Linq;

public class TaxCollectorCell : MonoBehaviour, ICellType
{
    public string GetCellName() => "Tax collector";

    public void OnStopOnCell(Cell cell, Player player)
    {
        var players = FindAnyObjectByType<TurnManager>().Players.Where(p => p != player);
        int percentage = Random.Range(3, 11);
        foreach (var targetPlayer in players)
        {
            player.AddMoney(targetPlayer.Money * percentage / 100);
            targetPlayer.SubtractMoney(targetPlayer.Money * percentage / 100);
        }
    }

    public void OnPassOnCell(Cell cell, Player player)
    {
        // No action for empty cell
    }
}