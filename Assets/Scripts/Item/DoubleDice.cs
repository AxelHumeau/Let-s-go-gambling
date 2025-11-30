using UnityEngine;
using System.Collections;

public class DoubleDice : IItem
{
    public ItemType type => ItemType.DoubleDice;

    public ItemTarget target => ItemTarget.Player;

    public string name => "Double Dice";

    public void Use(Player target, Player _)
    {
        target.AddEffect(Effect.Double, 1);
    }
}