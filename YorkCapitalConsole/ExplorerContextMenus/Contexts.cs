using ExplorerContextMenus.Entities;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Extensions;
using Wrappers;

namespace WindowsExplorerContextMenus
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
            if (Options.FileContextMenu)  CreateKey();            
        }

        private void CreateForFolders()
        {
            if (Options.FolderContextMenu) CreateKey(true);           
        }

        private void RemoveForFiles()
        {
            RemoveKey();
        }

        private void RemoveForFolders()
        {
            RemoveKey(true);
        }

        /// <summary>
        /// Creates root Key for File Level
        /// </summary>
        /// <param name="isFolder"></param>
        private void CreateKey(bool isFolder = false)
        {
            using (RegistryKey _key = Registry.ClassesRoot.OpenSubKey(isFolder ? ContextConstants.FOLDER_CONTEXT_MENU_REGISTRY : ContextConstants.FILE_CONTEXT_MENU_REGISTRY, true))
            {
                CreateSubRegistry(_key);
                _key.Close();
            }
        }

        /// <summary>
        /// Creates root key for Folder Level
        /// </summary>
        /// <param name="isFolder"></param>
        private void RemoveKey(bool isFolder = false)
        {
            using (RegistryKey _key = Registry.ClassesRoot.OpenSubKey( (isFolder ? ContextConstants.FOLDER_CONTEXT_MENU_REGISTRY : ContextConstants.FILE_CONTEXT_MENU_REGISTRY  ) + "\\", true))
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
            if (!WrapIOs.Exists(Options.ApplicationPath)) return;

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
