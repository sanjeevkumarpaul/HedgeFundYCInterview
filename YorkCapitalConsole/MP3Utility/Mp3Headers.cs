using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wrappers;
using Extensions;

using TagLib;
using MP3Utility.Taglib;
using MP3Utility.Entities;

namespace MP3Utility
{
    public class Mp3Headers
    {
        private Mp3Options Options;


        public Mp3Headers(Mp3Options options)
        {
            if (options == null) throw new Exception("Options can not be null");
            if ((options.Folder ?? options.File) == null) throw new Exception("Either Folder or File option should have been set");
            Options = options;
        }


        public Mp3Headers DisplayHeaders()
        {
            WrapIOs.FindFiles(Options.Folder ?? Options.File, Options.IncludeSubfolders)
                               .ToList()
                               .ForEach(p =>
                               {

                                   var _file = TagLib.File.Create(new FileAbstraction(p));

                                   if (_file.Tag.Performers != null)
                                   {
                                       Console.WriteLine($"Performers for the song: {System.IO.Path.GetFileName(_file.Name)}, are ...");
                                       foreach (var arts in _file.Tag.Performers)
                                       {
                                           arts.SplitEx(',').Select(a => a.Trim()).ToList().ForEach(a =>
                                           {
                                               Console.WriteLine($"{a}");

                                           });
                                       }
                                   }
                               });

            return this;
        }

        
        public Mp3Headers ReplacePhraseOnName()
        {
            if (!Options.RemovePhraseFromName.Empty())
            {
                var _replace = Options.ReplacePhaseOnRemove.Empty() ? "" : Options.ReplacePhaseOnRemove;

                WrapIOs.FindFiles(Options.Folder ?? Options.File, Options.IncludeSubfolders)
                                  .ToList()
                                  .ForEach(p =>
                                  {
                                      if (p.Contains(Options.RemovePhraseFromName))
                                          WrapIOs.Rename(p, p.Replace(Options.RemovePhraseFromName, _replace ));
                                  });
            }
            return this;
        }
    }
}
