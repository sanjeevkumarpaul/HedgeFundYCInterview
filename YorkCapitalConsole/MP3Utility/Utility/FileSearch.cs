using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Reactive.Concurrency;
using System.Reactive.Linq;

using Wrappers;

namespace MP3Utility.Utility
{
    public static class FileSearch
    {
        static FileSearch() { }

        private static string[] FindAll(MP3Utility.Entities.TagOptions options, string pattern = null)
        {
            return WrapIOs.FindFiles(options.Folder ?? options.File, options.IncludeSubfolders, pattern ?? options.SearchPattern ?? "*.*");
        }

        private static void Observe(MP3Utility.Entities.TagOptions options, Action<string> subscriber, string pattern = null, Func<string, bool> where = null)
        {
            if (where == null) where = ((f) => { return true; });

            FindAll(options, pattern)
                   .Where(where)
                   .ToObservable( NewThreadScheduler.Default )                  
                   .Subscribe(subscriber);
        }
        
        public static void Zips(MP3Utility.Entities.TagOptions options, Action<string> subscriber)
        {
            Observe(options, subscriber, "*.zip");
        }

        public static void Search(MP3Utility.Entities.TagOptions options, Action<string> subscriber)
        {
            Observe(options, 
                    subscriber, 
                    where: (f) => { return options.SearchPhraseFromName.Any(s => f.Contains(s)); } );
        }

        public static void All(MP3Utility.Entities.TagOptions options, Action<string> subscriber)
        {
            Observe(options, subscriber);
        }
    }
}
