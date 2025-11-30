using UnityEngine;
using System.Collections;
using System.Linq;

public class SwapPlayerCell : MonoBehaviour, ICellType
{
    public string GetCellName() => "Swap Players";

    public void OnStopOnCell(Cell cell, Player player)
    {
        var players = FindAnyObjectByType<TurnManager>().Players.Where(p => p != player);
        int index = Random.Range(0, players.Count());
        Player targetPlayer = players.ElementAt(index);
        Cell targetCell = targetPlayer.CurrentCell;
        targetPlayer.CurrentCell = cell;
        player.CurrentCell = targetCell;
    }

    public void OnPassOnCell(Cell cell, Player player)
    {
        // No action for empty cell
    }
}