using UnityEngine;
using System.Collections;

public class LesserDice : IItem
{
    public ItemType type => ItemType.LesserDice;

    public ItemTarget target => ItemTarget.Player;

    public void Use(Player target)
    {
        target.AddEffect(Effect.Half, 1);
    }
}