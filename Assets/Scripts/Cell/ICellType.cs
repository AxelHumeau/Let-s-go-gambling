using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface ICellType
{
    public string GetCellType();
    public void ExecuteCellAction(Cell cell);
}
