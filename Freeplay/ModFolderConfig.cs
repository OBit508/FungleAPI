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
        [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
        public class Item : Attribute
        {
            public Item(string itemName, string hexColor, string folderName)
            {
                ItemName = itemName;
                FolderName = folderName;
                try
                {
                    hexColor = hexColor.Replace("#", "");
                    byte r = 255;
                    byte g = 255;
                    byte b = 255;
                    byte a = 255;
                    if (hexColor.Length == 6)
                    {
                        r = byte.Parse(hexColor.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
                        g = byte.Parse(hexColor.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
                        b = byte.Parse(hexColor.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
                    }
                    else if (hexColor.Length == 8)
                    {
                        r = byte.Parse(hexColor.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
                        g = byte.Parse(hexColor.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
                        b = byte.Parse(hexColor.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
                        a = byte.Parse(hexColor.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
                    }
                    ItemColor = new Color(r, g, b, a);
                }
                catch
                {
                    ItemColor = Color.white;
                }
            }
            public string ItemName;
            public Color ItemColor;
            public string FolderName;
            public Action OnUse;
        }
    }
}
