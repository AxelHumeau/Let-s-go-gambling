using UnityEngine;
using System.Collections;

public class LesserDice : IItem
{
    public ItemType type => ItemType.LesserDice;

    public ItemTarget target => ItemTarget.Player;

    public string name => "Lesser Dice";

    public void Use(Player target, Player _)
    {
        target.AddEffect(Effect.Half, 1);
    }
}