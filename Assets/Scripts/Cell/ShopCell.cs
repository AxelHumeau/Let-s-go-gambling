using UnityEngine;
using System.Collections;

public class ShopCell : MonoBehaviour, ICellType
{
    public string GetCellName() => "Shop";

    public void OnStopOnCell(Cell cell, Player player)
    {
        FindAnyObjectByType<TurnManager>().PauseForCellAction(cell);
        FindAnyObjectByType<ShopManager>().OpenShop(player, () => FindAnyObjectByType<TurnManager>().CompleteCellAction(cell));
    }

    public void OnPassOnCell(Cell cell, Player player)
    {
        // No action for empty cell
    }
}