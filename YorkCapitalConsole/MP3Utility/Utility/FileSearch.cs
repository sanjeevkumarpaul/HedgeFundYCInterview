using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Reactive.Concurrency;
using System.Reactive.Linq;

using Wrappers;

namespace TagUtility.Utility
{
    public static class FileSearch
    {
        static FileSearch() { }

        private static string[] FindAll(TagUtility.Entities.TagOptions options, string pattern = null)
        {
            return WrapIOs.FindFiles(options.Folder ?? options.File, options.IncludeSubfolders, pattern ?? options.SearchPattern ?? "*.*");
        }

        private static void Observe<T>(TagUtility.Entities.TagOptions options, Action<T> subscriber, string pattern = null, Func<string, bool> where = null)
        {
            if (where == null) where = ((f) => { return true; });

            FindAll(options, pattern)
                   .Where(where)
                   .Cast<T>()
                   .ToObservable<T>( NewThreadScheduler.Default )                  
                   .Subscribe<T>(subscriber);
        }
        
        public static void Zips(TagUtility.Entities.TagOptions options, Action<string> subscriber)
        {
            Observe(options, subscriber, "*.zip");
        }

        public static void Search(TagUtility.Entities.TagOptions options, Action<string> subscriber)
        {
            Observe(options, 
                    subscriber, 
                    where: (f) => { return options.SearchPhraseFromName.Any(s => f.Contains(s)); } );
        }

        public static void All(TagUtility.Entities.TagOptions options, Action<string> subscriber)
        {
            Observe(options, subscriber);
        }

        public static void Renamer(TagUtility.Entities.TagOptions options, Action<Entities.ITagResult> subscriber)
        {

        }
    }
}
