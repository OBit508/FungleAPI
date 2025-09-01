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
            this.prefab = prefab;
        }
        public T Instantiate()
        {
            return GameObject.Instantiate<T>(prefab);
        }
        public T Instantiate(Transform parent)
        {
            return GameObject.Instantiate<T>(prefab, parent);
        }
        public T Instantiate(Vector3 position, Quaternion rotation)
        {
            return GameObject.Instantiate<T>(prefab, position, rotation);
        }
        public T Instantiate(Transform parent, bool worldPositionStays)
        {
            return GameObject.Instantiate<T>(prefab, parent, worldPositionStays);
        }
        public T Instantiate(Vector3 position, Quaternion rotation, Transform parent)
        {
            return GameObject.Instantiate<T>(prefab, position, rotation, parent);
        }
    }
}
