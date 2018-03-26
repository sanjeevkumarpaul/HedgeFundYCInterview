using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MP3Utility.Entities
{
    public class Mp3Options
    {
        public string Folder { get; set; }
        public string File { get; set; }
        public bool IncludeSubfolders { get; set; }

        public string RemovePhraseFromName { get; set; } 
        public string ReplacePhaseOnRemove { get; set; }
    }
}
