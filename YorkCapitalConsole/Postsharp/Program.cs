using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Postsharp.AspectEntities;

namespace PostsharpAspect
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Running...");
            Console.WriteLine(new Program().AspectIt());

            Console.ReadLine();
        }
      
        [MethodInjectionAspect]
        public string AspectIt()
        {
            Console.WriteLine("From AspectIT Method.");

            return "Have Fun!";
        }
    }
}
