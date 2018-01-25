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
                                                                                                             Subfolders =true,
                                                                                                             File = "BOA_2017_Savings 1099-INTTEST.pdf",
                                                                                                             Ranges = new List<PdfSharps.Entities.PageRange>
                                                                                                             {
                                                                                                                 new PdfSharps.Entities.PageRange(3,0)
                                                                                                             },
                                                                                                             ExcludeFileNames = new string[] { "BOA_2017_Savings 1099-INTt.pdf"},
                                                                                                             ExcludePattern = "w2*.*",
                                                                                                             //CompressToCopy = true
                                                                                                           }))
            {

                //_sharp.WriteFileNames();
                //_sharp.Merge();
                //_sharp.RemovePages();
                //_sharp.RemovePagesFromSelection();
                //_sharp.AddSamplePage();
                //_sharp.CompressSelection();
                //_sharp.Compress();

            }
            Console.ReadLine();

        }
    }
}
