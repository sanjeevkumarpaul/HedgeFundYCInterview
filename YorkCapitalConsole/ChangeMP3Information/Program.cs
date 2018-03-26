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
                                 Folder = @"C:\Users\Sanje\Downloads\Movies\Songs\2018\Albumbs\Movies",
                                 IncludeSubfolders = true,
                                 RemovePhraseFromName = " - Songs.pk - 320Kbps",
                                 ReplacePhaseOnRemove = "",
                             })
                .ReplacePhraseOnName()
                .DisplayHeaders();


            Console.ReadKey();

        }
    }
}
