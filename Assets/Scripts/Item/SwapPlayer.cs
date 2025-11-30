using UnityEngine;
using System.Collections;

public class SwapPlayer : IItem
{
    public ItemType type => ItemType.SwapPlayer;

    public ItemTarget target => ItemTarget.Player;

    public string name => "Swap Player";

    public void Use(Player target, Player user)
    {
        Cell targetCell = target.CurrentCell;
        target.CurrentCell = user.CurrentCell;
        user.CurrentCell = targetCell;
    }
}