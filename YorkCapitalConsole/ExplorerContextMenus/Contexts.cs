using ExplorerContextMenus.Entities;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Extensions;

namespace ExplorerContextMenus
{
    public class Contexts
    {
        ContextOptions Options;

        public Contexts(ContextOptions options)
        {
            if (options == null) throw new Exception("Contexts options are missing");
            Options = options;
        }

        public void Create()
        {
            if (Options.ApplicationPath.Empty() || Options.Description.Empty()) return;

            CreateForFiles();
            CreateForFolders();
        }

        public void Remove()
        {
            if (Options.Description.Empty()) return;

            RemoveForFiles();
            RemoveForFolders();
        }

        /// <summary>
        /// PRIVATE METHODS
        /// </summary>
        private void CreateForFiles()
        {
            if (Options.FileContextMenu)
            {
                using (RegistryKey _key = Registry.ClassesRoot.OpenSubKey(ContextConstants.FILE_CONTEXT_MENU_REGISTRY, true))
                {
                    CreateSubRegistry(_key);
                    _key.Close();
                }
            }
        }

        private void CreateForFolders()
        {
            if (Options.FolderContextMenu)
            {
                using (RegistryKey _key = Registry.ClassesRoot.OpenSubKey(ContextConstants.FOLDER_CONTEXT_MENU_REGISTRY, true))
                {
                    CreateSubRegistry(_key);
                    _key.Close();
                }
            }
        }

        private void RemoveForFiles()
        {
           using (RegistryKey _key = Registry.ClassesRoot.OpenSubKey(ContextConstants.FILE_CONTEXT_MENU_REGISTRY + "\\", true))
           {
                    _key.DeleteSubKey(Options.Description);
                    _key.Close();           
           }
        }

        private void RemoveForFolders()
        {
            using (RegistryKey _key = Registry.ClassesRoot.OpenSubKey(ContextConstants.FOLDER_CONTEXT_MENU_REGISTRY, true))
            {
                _key.DeleteSubKey(Options.Description);
                _key.Close();
            }
        }

        /// <summary>
        /// Internal Method to create SubKey into Registry either for File or folder.
        /// </summary>
        /// <param name="rootRegisteryKey"></param>
        private void CreateSubRegistry(RegistryKey rootRegisteryKey)
        {
            using (RegistryKey newkey = rootRegisteryKey.CreateSubKey(Options.Description))
            {
                using (RegistryKey subNewkey = newkey.CreateSubKey(ContextConstants.COMMAND_REGISTRY))
                {
                    subNewkey.SetValue("", Options.ApplicationPath);
                    subNewkey.Close();
                }
                newkey.Close();
            }
        }

        
       

    }
}
