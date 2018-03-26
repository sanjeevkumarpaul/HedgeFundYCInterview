using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wrappers;
using Extensions;

using TagLib;
using MP3Utility.Taglib;

namespace MP3Utility
{
    public class Mp3Headers
    {
        private string path = @"C:\Users\Sanje\Downloads\Movies\Songs\2018\Albumbs\Movies\Hate Story 4";


        public void DisplayHeaders()
        {
            WrapIOs.FindFiles(path)
                               .ToList()
                               .ForEach(p => {

                                   var _file = TagLib.File.Create( new FileAbstraction(p) );
                                   
                                   if (_file.Tag.Performers != null)
                                   {
                                       Console.WriteLine($"Performers for the song: {System.IO.Path.GetFileName(_file.Name)}, are ...");
                                       foreach (var arts in _file.Tag.Performers)
                                       {
                                           arts.SplitEx(',').ToList().ForEach(a =>
                                           {
                                               Console.WriteLine($"{a}");

                                           });
                                       }
                                   }
                               });
        }

       

    }
}
