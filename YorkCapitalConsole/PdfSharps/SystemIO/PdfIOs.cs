using PdfUtility.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Extensions;

namespace PdfUtility.SystemIO
{
    public static class PdfIOs
    {
        internal static PdfOptions Options { get; set; }

        internal static IList<FileInfo> Files()
        {
            var _files = new System.IO.DirectoryInfo(Options.Folder)
                             .GetFiles("*.pdf", Options.Subfolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)
                             .ToList();

            if (Options.ExcludeFileNames != null)
                Options.ExcludeFileNames.ToList().ForEach(f =>
                {
                    _files.RemoveAll(z => Path.GetFileName(z.FullName).Equals(f, StringComparison.CurrentCultureIgnoreCase) ||
                                          z.FullName.Equals(f, StringComparison.CurrentCultureIgnoreCase));
                });

            if (!Options.ExcludePattern.Empty())
                new System.IO.DirectoryInfo(Options.Folder)
                             .GetFiles(Options.ExcludePattern, Options.Subfolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)
                             .ToList().ForEach(f =>
                             {
                                 _files.RemoveAll(z => z.FullName.Equals(f.FullName, StringComparison.CurrentCultureIgnoreCase));
                             });

            return _files;
        }

        internal static string AppendToFileName(string filepath, string prefix = "", string postfix = "")
        {
            return $@"{Path.GetDirectoryName(filepath)}\{prefix}{Path.GetFileNameWithoutExtension(filepath)}{postfix}{Path.GetExtension(filepath)}";
        }

    }
}
