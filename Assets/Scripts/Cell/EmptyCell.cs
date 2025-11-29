using UnityEngine;
using System.Collections;

public class EmptyCell : MonoBehaviour, ICellType
{
    public string GetCellType() => "Empty";

    public void OnStopOnCell(Cell cell)
    {
        // No action for empty cell
    }

    public void OnPassOnCell(Cell cell)
    {
        // No action for empty cell
    }
}