using UnityEngine;
using System.Collections;

public class Krach : IItem
{
    public ItemType type => ItemType.Krach;

    public ItemTarget target => ItemTarget.Player;

    public string name => "Krach";

    public void Use(Player target, Player _)
    {
        // Make the player sell all his stocks
    }
}