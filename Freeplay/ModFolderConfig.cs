using FungleAPI.Attributes;
using FungleAPI.Configuration;
using FungleAPI.Configuration.Attributes;
using FungleAPI.Utilities;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.Freeplay
{
    /// <summary>
    /// Class used to create the mod folder in freeplay
    /// </summary>
    [FungleIgnore]
    public class ModFolderConfig
    {
        public bool initialized;
        public Dictionary<string, List<Item>> Folders = new Dictionary<string, List<Item>>();
        public List<Item> Items = new List<Item>();
        public virtual void Initialize()
        {
            if (!initialized)
            {
                Type type = GetType();
                foreach (PropertyInfo property in type.GetProperties())
                {
                    Item att = (Item)property.GetCustomAttribute(typeof(Item));
                    if (att != null)
                    {
                        if (property.PropertyType == typeof(Action))
                        {
                            att.OnUse = (Action)property.GetValue(this);
                            if (att.FolderName != null)
                            {
                                if (Folders.ContainsKey(att.FolderName))
                                {
                                    Folders[att.FolderName].Add(att);
                                }
                                else
                                {
                                    Folders.Add(att.FolderName, new List<Item>() { att });
                                }
                            }
                            else
                            {
                                Items.Add(att);
                            }
                        }
                    }
                }
                initialized = true;
            }
        }
        public virtual TaskAddButton CreateButton(TaskAdderGame taskAdderGame, Item item)
        {
            TaskAddButton taskAddButton = UnityEngine.Object.Instantiate(taskAdderGame.RoleButton);
            taskAddButton.Overlay.gameObject.SetActive(false);
            taskAddButton.SafePositionWorld = taskAdderGame.SafePositionWorld;
            taskAddButton.Text.text = item.ItemName;
            taskAddButton.GetComponent<SpriteRenderer>().color = item.ItemColor;
            taskAddButton.RolloverHandler.OutColor = item.ItemColor;
            if (item.OnUse != null)
            {
                taskAddButton.Button.SetNewAction(item.OnUse);
            }
            return taskAddButton;
        }
        /// <summary>
        /// Attribute used to create a file with an Action
        /// </summary>
        [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
        public class Item : Attribute
        {
            public Item(string itemName, string hexColor, string folderName)
            {
                ItemName = itemName;
                FolderName = folderName;
                Color color = Color.white;
                ColorUtility.TryParseHtmlString(hexColor, out color);
                ItemColor = color;
            }
            public string ItemName;
            public Color ItemColor;
            public string FolderName;
            public Action OnUse;
        }
    }
}
