using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wrappers;

namespace TagUtility.Entities
{
    public class TagUnZipper : ITagResult
    {
        public string FilePath { get; set; }

        public void Action(TagOptions options)
        {
            Console.WriteLine($"Unziping the file : [ {FilePath} ]");
            WrapIOs.UnZip(FilePath, deleteAfterExtraction: options.DeleteAfterExtraction);
            Console.WriteLine("Done.!"); Console.WriteLine();
        }
    }
}
