using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Reactive.Concurrency;
using System.Reactive.Linq;

using Wrappers;
using TagUtility.Entities;

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
                       .ToObservable<T>(NewThreadScheduler.Default)
                       .Subscribe<T>(subscriber);
            
        }

        private static void Observe<T>(TagUtility.Entities.TagOptions options, string pattern = null, Func<string, bool> where = null) where T: ITagResult, new()
        {
            if (where == null) where = ((f) => { return true; });

            FindAll(options, pattern)
                   .Where(where)
                   .Select(f => new T { FilePath = f })
                   .ToObservable<T>(NewThreadScheduler.Default)
                   .Do<T>((tag) => { Console.WriteLine($"Proessing - { System.IO.Path.GetFileName(((ITagResult)tag).FilePath) }"); })
                   .Subscribe((tag) => { ((ITagResult)tag).Action(options); });
            
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

        public static void Informer<T>(TagUtility.Entities.TagOptions options) where T : ITagResult, new()
        {
            Observe<T>(options);
        }

        public static void Zipper<T>(TagUtility.Entities.TagOptions options) where T : ITagResult, new()
        {
            Observe<T>(options, "*.zip");
        }

        public static void Search<T>(TagUtility.Entities.TagOptions options) where T : ITagResult, new()
        {
            Observe<T>(options, where: (f) => { return options.SearchPhraseFromName.Any(s => f.Contains(s)); });
        }
    }
}
