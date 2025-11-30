using UnityEngine;
using System.Collections;

public class TeleportToStockCell : IItem
{
    public ItemType type => ItemType.TeleportToStocksCell;

    public ItemTarget target => ItemTarget.None;

    public string name => "Teleport to Stock cell";

    public void Use(Player _, Player user)
    {
        // Teleport to nearest Stock cell
    }
}