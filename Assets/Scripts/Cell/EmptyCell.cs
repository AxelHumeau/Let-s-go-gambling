using UnityEngine;
using System.Collections;

public class EmptyCell : MonoBehaviour, ICellType
{
    public string GetCellName() => "Empty";

    public void OnStopOnCell(Cell cell, Player player)
    {
        // No action for empty cell
    }

    public void OnPassOnCell(Cell cell, Player player)
    {
        // No action for empty cell
    }
}