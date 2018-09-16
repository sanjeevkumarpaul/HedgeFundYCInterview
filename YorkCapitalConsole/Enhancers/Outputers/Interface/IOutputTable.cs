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
        void Add(ConsoleTable table);
        void Remove(ConsoleTable table);
        void Remove(int index);
        void Draw();
    }
}
