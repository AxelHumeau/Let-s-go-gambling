using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum ItemType { Juggernaut, LuckyCharm, DoubleDice, LesserDice, TruthLasso, CurseDoll, Krach, TeleportXCellAhead, SwapPlayer, SwapCells, TeleportToStocksCell, StealItem }
public enum ItemTarget { Player, None }
public interface IItem
{
    ItemType type { get; }
    ItemTarget target { get; }
    void Use(Player target);
}
