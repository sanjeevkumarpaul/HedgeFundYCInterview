using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TagUtility.Taglib
{
    public class FileAbstraction : TagLib.File.IFileAbstraction
    {
        private string name;

        public FileAbstraction(string file)
        {
            name = file;
        }

        public string Name
        {
            get { return name; }
        }

        public System.IO.Stream ReadStream
        {
            get { return new System.IO.FileInfo(name).OpenRead(); }
        }

        public System.IO.Stream WriteStream
        {
            get { return new System.IO.FileInfo(name).OpenWrite(); }
        }

        public void CloseStream(System.IO.Stream stream)
        {
            stream.Close();
        }

    }
}
