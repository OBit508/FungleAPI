using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.Utilities.Prefabs
{
    public class Prefab<T> where T : UnityEngine.Object
    {
        public T prefab;
        public Prefab(T prefab)
        {
            this.prefab = prefab.DontUnload();
            if (prefab.SafeCast<Component>() != null)
            {
                prefab.SafeCast<Component>().gameObject.SetActive(false);
                prefab.SafeCast<Component>().DontDestroy();
            }
        }
        private T Enable(T p)
        {
            if (p.SafeCast<Component>() != null)
            {
                p.SafeCast<Component>().gameObject.SetActive(true);
            }
            return p;
        }
        public T Instantiate()
        {
            return Enable(GameObject.Instantiate<T>(prefab));
        }
        public T Instantiate(Transform parent)
        {
            return Enable(GameObject.Instantiate<T>(prefab, parent));
        }
        public T Instantiate(Vector3 position, Quaternion rotation)
        {
            return Enable(GameObject.Instantiate<T>(prefab, position, rotation));
        }
        public T Instantiate(Transform parent, bool worldPositionStays)
        {
            return Enable(GameObject.Instantiate<T>(prefab, parent, worldPositionStays));
        }
        public T Instantiate(Vector3 position, Quaternion rotation, Transform parent)
        {
            return Enable(GameObject.Instantiate<T>(prefab, position, rotation, parent));
        }
    }
}
