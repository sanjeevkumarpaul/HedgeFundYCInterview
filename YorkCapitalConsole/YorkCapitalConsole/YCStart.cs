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

            Console.WriteLine($"Factorial of 12 : {_console.Factorial(12)}");
            Console.WriteLine($"Factorial of 12 Recursive : {_console.FactorialRecursive(12)}");

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

        public long Factorial(int value)
        {
            long _fact = 1;
            for (int index = 2; index <= value; index++) _fact *= index;

            return _fact;
        }

        public long FactorialRecursive(int value)
        {
            if (value <= 1) return 1;            

            return value * FactorialRecursive(value - 1);
        }
    }

}
