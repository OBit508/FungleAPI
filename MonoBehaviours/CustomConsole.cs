using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.MonoBehaviours
{
    public class CustomConsole : MonoBehaviour
    {
        public Action onUse;
        public bool deadsCanUse;
        public static CustomConsole CreateConsole(float distance, bool deadsCanUse, Action onUse, Sprite sprite)
        {
            CustomConsole console = null;
            foreach (SystemConsole c in GameObject.FindObjectsOfType<SystemConsole>())
            {
                console = GameObject.Instantiate<SystemConsole>(c).gameObject.AddComponent<CustomConsole>();
                break;
            }
            for (int i = 0; i < console.transform.GetChildCount(); i++)
            {
                Destroy(console.transform.GetChild(i).gameObject);
            }
            console.GetComponent<SystemConsole>().useIcon = ImageNames.UseButton;
            console.deadsCanUse = deadsCanUse;
            console.onUse = onUse;
            console.GetComponent<SpriteRenderer>().sprite = sprite;
            console.GetComponent<SystemConsole>().usableDistance = distance;
            console.transform.localScale = Vector3.one;
            console.transform.position = Vector3.zero;
            return console;
        }
    }
}
