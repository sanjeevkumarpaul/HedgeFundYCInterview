using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wrappers.Consoles;

namespace Wrappers.Outputers.Interface
{
    public interface IOutputTable
    {
        IOutputTable Add(ConsoleTable table);
        IOutputTable Remove(ConsoleTable table);
        IOutputTable Remove(int index);
        void Draw();
    }
}
