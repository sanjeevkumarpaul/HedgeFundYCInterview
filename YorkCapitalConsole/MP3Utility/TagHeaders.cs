using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wrappers;
using Extensions;

using TagLib;
using TagUtility.Entities;
using TagUtility.Utility;
using TagUtility.Taglib;


namespace TagUtility
{
    public class TagHeaders
    {
        private TagOptions Options;


        public TagHeaders(TagOptions options)
        {
            if (options == null) throw new Exception("Options can not be null");
            if ((options.Folder ?? options.File) == null) throw new Exception("Either Folder or File option should have been set");
            Options = options;
        }


        public TagHeaders DisplayHeaders()
        {
            FileSearch.Informer<TagDisplayer>(Options); //This is done via Reactive Way and Observation Pattern 

            return this;
        }

        public TagHeaders UnZip()
        {
            if (Options.ExtractZip)
            {                
                FileSearch.Zipper<TagUnZipper>(Options);
            }

            return this;
        }
        
        public TagHeaders ReplacePhraseOnName()
        {
            if (!Options.SearchPhraseFromName.JoinExt().Empty() )
            {
                var _replace = Options.ReplacePhase.Empty() ? "" : Options.ReplacePhase;
                
                FileSearch.Search(Options, Replace);  //Observable Sequence

                //C# 7.0 Local function.
                void Replace(string path)
                {
                    Console.WriteLine($"Renaming - {path}");
                    Options.SearchPhraseFromName.First(p => WrapIOs.Rename(path, p, _replace));                     
                }
            }
            return this;
        }


        ///PRIVATE///
        
        private void Subscription(ITagResult result)
        {
            result.Action(Options);
        }
    }
}
