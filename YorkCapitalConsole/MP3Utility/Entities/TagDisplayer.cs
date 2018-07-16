using Extensions;
using System;
using System.Linq;
using TagUtility.Taglib;

namespace TagUtility.Entities
{
    public class TagDisplayer : ITagResult
    {
        public string FilePath { get; set; }

        public void Action(TagOptions options)
        {
            if (Wrappers.WrapIOs.Exists(FilePath))
            {
                var _file = TagLib.File.Create(new FileAbstraction(FilePath));

                if (_file.Tag.Performers != null)
                {
                    Console.WriteLine($"Performers for the song: {System.IO.Path.GetFileName(_file.Name)}, are ...");
                    Console.WriteLine($"{ (".".Repeat(75)) }{Environment.NewLine}");
                    foreach (var arts in _file.Tag.Performers)
                    {
                        arts.SplitEx(',').Select(a => a.Trim()).ToList().ForEach(a =>
                        {
                            Console.Write($"[{a}]  ");

                        });
                    }
                    Console.WriteLine($"{Environment.NewLine}{Environment.NewLine}Other Information-{Environment.NewLine}{ (".".Repeat(30)) }");
                    Console.WriteLine($"Beats Per Minute: {_file.Tag.BeatsPerMinute}");
                    Console.WriteLine($"Albumb          : {_file.Tag.Album}");
                    Console.WriteLine($"Year            : {_file.Tag.Year}");
                    Console.WriteLine($"Track Number    : {_file.Tag.Track}");
                    Console.WriteLine($"Title           : {_file.Tag.Title}");

                    Console.WriteLine($"{Environment.NewLine}{ ("=".Repeat(100)) }{Environment.NewLine}");
                }
            }
        }
    }
}
