using UnityEngine;
using System.Collections;

public class TruthLasso : IItem
{
    public ItemType type => ItemType.TruthLasso;

    public ItemTarget target => ItemTarget.Player;

    public string name => "Thuth Lasso";

    public void Use(Player target, Player _)
    {
        target.AddEffect(Effect.NoBluff, 2);
    }
}