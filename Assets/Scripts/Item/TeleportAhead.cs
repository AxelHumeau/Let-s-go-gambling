using UnityEngine;
using System.Collections;

public class TeleportAhead : IItem
{
    public ItemType type => ItemType.TeleportXCellAhead;

    public ItemTarget target => ItemTarget.None;

    public string name => "Teleport X ahead";

    public void Use(Player _, Player user)
    {
        // Teleport the player ahead by X cells, where X is a number chosen by the player
    }
}