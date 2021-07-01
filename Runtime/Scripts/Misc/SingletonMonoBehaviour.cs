using System.Collections.Generic;
using UnityEngine;

namespace CGenStudios.UnityUtils
{
    /// <summary>
    /// Inherit from this class with your class as the type, and in a static method with the [RuntimeInitializeOnLoadMethod] attribute, call the inherited method Initialize().
    /// </summary>
    public class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T Instance { get; private set; } = null;

        private static bool InstanceInitialized { get; set; } = false;

        /// <summary>
        /// Call this method from a static method with the [RuntimeInitializeOnLoadMethod] attribute.
        /// </summary>
        protected static void Initialize()
        {
            if (!InstanceInitialized)
            {
                Instance = new GameObject(typeof(T).Name).AddComponent<T>();
                Instance.Invoke("Initialized", 0.0f);

                InstanceInitialized = true;
            }
        }

        protected static void Initialize(GameObject prefab)
        {
            if (!InstanceInitialized)
            {
                Instance = Instantiate(prefab).GetComponent<T>();
                Instance.Invoke("Initialized", 0.0f);

                InstanceInitialized = true;
            }
        }

        protected virtual void Initialized() { }
    }
}
