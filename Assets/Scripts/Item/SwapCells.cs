using UnityEngine;
using System.Collections;

public class SwapCells : IItem
{
    public ItemType type => ItemType.SwapCells;

    public ItemTarget target => ItemTarget.None;

    public string name => "Thuth Lasso";

    public void Use(Player _, Player __)
    {
        // Randomly swap cells across the board
    }
}