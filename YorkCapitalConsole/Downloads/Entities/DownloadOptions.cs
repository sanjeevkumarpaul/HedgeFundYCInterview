using Extensions;

namespace Downloads.Entities
{
    public enum FileTypes
    {
        PDF,
        HTML,
        JPEG,
        PNG,
        EXCEL,
        WORD,
        CSV
    }

    public class DownloadOptions
    {
        /// <summary>
        /// This is used since only set acessor is available for FileName property.
        /// </summary>
        private string _filename = "";
        
        /// <summary>
        /// Extension decides the content Type, if not specified, a default file name will be considered along with the File type is set.
        /// </summary>
        public string FileName { set { _filename = value;  } }

        /// <summary>
        /// If this is specified, utility will not take decision as per the filename extension
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// This may also decide the content type.
        /// </summary>
        public FileTypes? FileType { get; set; }

        /// <summary>
        /// Always get the fileName from ehre
        /// </summary>
        /// <returns></returns>
        internal string GetFileName()
        {
            string defaultfile = "DownloadedContent.";

            if (!_filename.Empty()) return _filename;
            if (FileType == null) return $"{defaultfile}.pdf";

            switch(FileType)
            {
                case FileTypes.PDF: return $"{defaultfile}.pdf";
                case FileTypes.HTML: return $"{defaultfile}.html";
                case FileTypes.WORD: return $"{defaultfile}.docx";
                case FileTypes.EXCEL: return $"{defaultfile}.xlsx";
                case FileTypes.JPEG: return $"{defaultfile}.jpeg";
                case FileTypes.PNG: return $"{defaultfile}.png";
                case FileTypes.CSV: return $"{defaultfile}.csv";
                default: return $"{defaultfile}.pdf";
            }
        }
    }
}
