using UnityEngine;
using System.Collections;

public class EmptyCell : MonoBehaviour, ICellType
{
    public string GetCellType() => "Empty";

    public void ExecuteCellAction(Cell cell)
    {
        // No action for empty cell
    }
}