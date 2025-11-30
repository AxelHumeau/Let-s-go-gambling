using UnityEngine;
using System.Collections;

public class AddMoneyCell : MonoBehaviour, ICellType
{
    public string GetCellName() => "Add Money";

    public void OnStopOnCell(Cell cell, Player player)
    {
        player.AddMoney(Random.Range(25, 100));
    }

    public void OnPassOnCell(Cell cell, Player player)
    {
        // No action for empty cell
    }
}