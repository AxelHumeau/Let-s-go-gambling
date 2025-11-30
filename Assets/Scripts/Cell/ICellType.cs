using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface ICellType
{
    public string GetCellName();
    public void OnStopOnCell(Cell cell, Player player);
    public void OnPassOnCell(Cell cell, Player player);
}
