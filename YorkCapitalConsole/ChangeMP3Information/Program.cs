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

            new Mp3Headers(  new MP3Utility.Entities.Mp3Options
                             {
                                 Folder = @"C:\Users\Sanje\Downloads\Movies\Songs",
                                 IncludeSubfolders = true,
                                 RemovePhraseFromName = " - Songs.pk - 320Kbps",
                                 ReplacePhaseOnRemove = "",
                                 ExtractZip = true,
                                 DeleteAfterExtraction = true
                             })
                .UnZip()
                .ReplacePhraseOnName()
                .DisplayHeaders();


            Console.ReadKey();

        }
    }
}
