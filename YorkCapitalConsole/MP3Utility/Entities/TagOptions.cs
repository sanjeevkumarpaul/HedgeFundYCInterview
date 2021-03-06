﻿
namespace TagUtility.Entities
{
    public class TagOptions
    {
        public string Folder { get; set; }
        public string File { get; set; }
        public bool IncludeSubfolders { get; set; }
        public bool ExtractZip { get; set; }
        public bool DeleteAfterExtraction { get; set; }
        public string SearchPattern { get; set; }

        public string[] SearchPhraseFromName { get; set; } 
        public string ReplacePhase { get; set; }
    }
}
