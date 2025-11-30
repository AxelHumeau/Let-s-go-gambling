using UnityEngine;

public class StocksCell : MonoBehaviour, ICellType
{
    public string GetCellName() => "Stocks";

    public void OnStopOnCell(Cell cell, Player player)
    {
        FindAnyObjectByType<TurnManager>().PauseForCellAction(cell);
        StockManager.Instance.OpenMenu(() => FindAnyObjectByType<TurnManager>().CompleteCellAction(cell));
    }

    public void OnPassOnCell(Cell cell, Player player) {}
}
