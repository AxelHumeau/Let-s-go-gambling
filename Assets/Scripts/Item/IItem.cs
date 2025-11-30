using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum ItemType { Juggernaut, DoubleDice, LesserDice, TruthLasso, Krach, TeleportXCellAhead, SwapPlayer, SwapCells, TeleportToStocksCell, StealItem }
public enum ItemTarget { Player, None }
public interface IItem
{
    ItemType type { get; }
    string name { get; }
    ItemTarget target { get; }
    void Use(Player target, Player user);
}
