using UnityEngine;
using System.Collections;

public class Juggernaut : IItem
{
    public ItemType type => ItemType.Juggernaut;

    public ItemTarget target => ItemTarget.Player;

    public string name => "Juggernaut";

    public void Use(Player target, Player _)
    {
        target.AddEffect(Effect.MineResistant, 1);
    }
}