using MP3Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChangeMP3Information
{
    class Program
    {
        static void Main(string[] args)
        {

            new Mp3Headers().DisplayHeaders();


            Console.ReadKey();

        }
    }
}
