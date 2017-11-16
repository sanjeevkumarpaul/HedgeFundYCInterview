using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YorkCapitalConsole
{
    class YCStart
    {
        static void Main(string[] args)
        {
            var _console = new YCConsole();
            
            Console.WriteLine($"Prime check of 121 : {_console.IsPrime(121)}");
            Console.WriteLine($"Prime check of 3 : {_console.IsPrime(3)}");

            Console.ReadLine();
        }
    }

    class YCConsole
    {
        public bool IsPrime(long value)
        {
            bool _flag = true;

            for (long index = value - 1; index > 1; index--)
            {
                if (value % index == 0) { _flag = false; break; }
            }

            return _flag;
        }

    }

}
