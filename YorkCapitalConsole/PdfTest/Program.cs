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
                                                                                                             File = "TaxReturns_2007.pdf",
                                                                                                             Ranges = new List<PdfSharps.Entities.PageRange>
                                                                                                             {
                                                                                                                 new PdfSharps.Entities.PageRange(2,0)
                                                                                                             },
                                                                                                             ExcludeFileNames = new string[] { "BOA_2017_Savings 1099-INTt.pdf"},
                                                                                                             ExcludePattern = "w2*.*",
                                                                                                             //CompressToCopy = true,
                                                                                                             DivisionPageSize = 0,
                                                                                                             DivisionOnRanges = true
                                                                                                           }))
            {
                //_sharp.WriteFileNames();
                //_sharp.Merge();
                //_sharp.RemovePages();
                //_sharp.RemovePagesSelection();
                //_sharp.AddSamplePage();
                //_sharp.CompressSelection();
                //_sharp.Compress();
                //_sharp.DivideSelection();

            }
            Console.ReadLine();

        }
    }
}
