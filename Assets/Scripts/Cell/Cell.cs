using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public List<Cell> nextCells;
    public List<Cell> previousCells;
    ICellType cellType;

    void Start()
    {
        if (!TryGetComponent<ICellType>(out cellType))
        {
            cellType = gameObject.AddComponent<EmptyCell>();
        }
    }

    void Update()
    {

    }

    public void OnStopOnCell(Player player)
    {
        cellType.OnStopOnCell(this, player);
    }

    public void OnPassOnCell(Player player)
    {
        cellType.OnPassOnCell(this, player);
    }
}