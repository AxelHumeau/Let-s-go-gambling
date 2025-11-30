using UnityEngine;
using System.Collections;

public class DoubleDice : IItem
{
    public ItemType type => ItemType.DoubleDice;

    public ItemTarget target => ItemTarget.Player;

    public void Use(Player target)
    {
        target.AddEffect(Effect.Double, 1);
    }
}