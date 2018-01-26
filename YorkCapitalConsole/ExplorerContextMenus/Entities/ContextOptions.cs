using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsExplorerContextMenus.Entities
{
    public class ContextOptions
    {
        public string Description { get; set; }
        public string ApplicationPath { get; set; }
        public bool FileContextMenu { get; set; }
        public bool FolderContextMenu { get; set; }

    }
}
