using UnityEngine;
using System.Collections;

public class SubMoneyCell : MonoBehaviour, ICellType
{
    public string GetCellName() => "Sub Money";

    public void OnStopOnCell(Cell cell, Player player)
    {
        player.SubtractMoney(Random.Range(10, 50));
    }

    public void OnPassOnCell(Cell cell, Player player)
    {
        // No action for empty cell
    }
}