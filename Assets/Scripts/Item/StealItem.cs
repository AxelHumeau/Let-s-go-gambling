using UnityEngine;
using System.Collections;

public class StealItem : IItem
{
    public ItemType type => ItemType.StealItem;

    public ItemTarget target => ItemTarget.Player;

    public string name => "Steal Item";

    public void Use(Player target, Player user)
    {
        int index = Random.Range(0, target.Inventory.Count);
        IItem stolenItem = target.Inventory[index];
        target.RemoveItem(index);
    }
}