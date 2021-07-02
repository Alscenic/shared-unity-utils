using System.Collections.Generic;
using UnityEngine;

namespace CGenStudios.UnityUtils
{
    /// <summary>
    /// Inherit from this class with your class as the type, and in a static method with the [RuntimeInitializeOnLoadMethod] attribute, call the inherited method Initialize().
    /// </summary>
    public class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
    {
        [SerializeField]
        [TextArea]
        private string m_SingletonInfo = SINGLETON_INFO;

        private const string SINGLETON_INFO = "This script is not meant to be manually " +
            "added to GameObjects, as they will be created automatically at run-time. Leaving " +
            "this script on an object will cause at least one duplicate.";

        protected virtual void OnValidate()
        {
            this.m_SingletonInfo = SINGLETON_INFO;
        }

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
                DontDestroyOnLoad(Instance.gameObject);
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
