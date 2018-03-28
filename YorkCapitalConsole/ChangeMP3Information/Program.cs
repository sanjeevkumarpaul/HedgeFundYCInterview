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

            new TagHeaders(  new MP3Utility.Entities.TagOptions
                             {
                                 Folder = @"C:\Users\Sanje\Downloads\Movies\Songs",
                                 IncludeSubfolders = true,
                                 SearchPhraseFromName = new string[] { " - Songs.pk - 320Kbps", " - Songs.pk - 128Kbps" },
                                 ReplacePhase = "",
                                 SearchPattern = "*.mp3",
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
