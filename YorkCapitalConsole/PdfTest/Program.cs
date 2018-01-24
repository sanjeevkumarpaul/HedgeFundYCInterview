using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfTest
{
    class Program
    {
        static void Main(string[] args)
        {
            using (PdfSharps.PdfAction _sharp = new PdfSharps.PdfAction(new PdfSharps.Entities.PdfOptions {  Folder = @"C:\Users\Sanje\Downloads\tax",
                                                                                                             OutText = "Have a break #name#.",
                                                                                                             Subfolders =true}))
            {

                _sharp.WriteFileNames();

            }
            Console.ReadLine();

        }
    }
}
