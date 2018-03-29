﻿using System;
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
            FileSearch.All(Options, Display); //Observable Sequence

            //C# 7.0 Local function.
            void Display(string path)
            {
                var _file = TagLib.File.Create(new FileAbstraction(path));

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

            return this;
        }

        public TagHeaders UnZip()
        {
            if (Options.ExtractZip)
            {
                FileSearch.Zips(Options, UnZipSubscriber);  //Observable Sequence

                //C# 7.0 Local function.
                void UnZipSubscriber(string zipPath)
                {
                    Console.WriteLine($"Unziping the file : [ {zipPath} ]");
                    WrapIOs.UnZip(zipPath, deleteAfterExtraction: Options.DeleteAfterExtraction);
                    Console.WriteLine("Done.!"); Console.WriteLine();
                }
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
            result.Display(Options);
        }
    }
}
