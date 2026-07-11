using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using xCloud;

namespace FungleAPI.Freeplay.Helpers
{
    public class Folder
    {
        public string FolderName;
        public Color FolderColor = new Color(0.937f, 0.811f, 0.592f);
        public List<Folder> SubFolders = new List<Folder>();
        public List<FolderItem> Items = new List<FolderItem>();
        public Folder AddFolder(string folderName, Color folderColor)
        {
            Folder folder = new Folder { FolderName = folderName, FolderColor = folderColor };
            SubFolders.Add(folder);
            return folder;
        }
        public FolderItem AddItem(string name, Action onClick, Color color, Func<bool> overlay = null)
        {
            FolderItem item = new FolderItem
            {
                Name = name,
                OnClick = onClick,
                Color = color,
                Overlay = overlay ?? (() => false),
            };
            Items.Add(item);
            return item;
        }
    }
}
