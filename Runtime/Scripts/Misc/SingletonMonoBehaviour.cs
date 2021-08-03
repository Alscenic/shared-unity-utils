using System.Collections.Generic;
using UnityEngine;

namespace CGenStudios.UnityUtils
{
    /// <summary>
    /// Inherit from this class with your class as the type, and in a static method with the [RuntimeInitializeOnLoadMethod] attribute, call the inherited method Initialize().
    /// </summary>
    public class SingletonMonoBehaviour<T> : MonoBehaviour, ISingleton where T : MonoBehaviour, ISingleton
    {
        public static T Instance { get; private set; } = null;

        public static bool InstanceInitialized { get; private set; } = false;

        protected bool ThisInstanceInitialized { get; private set; } = false;

        protected virtual void Start() { }

        protected virtual void Awake()
        {
            // If the object is initialized via scene instead of [RuntimeInitializeOnLoadMethod]
            if (!InstanceInitialized)
            {
                Initialize(GetComponent<T>());
            }
            // If we've already initialized an object
            else
            {
                // and this one hasn't been initialized
                if (!ThisInstanceInitialized)
                {
                    // then we're a dupe & need to destroy
                    Destroy(GetComponent<T>());
                }
            }
        }

        /// <summary>
        /// Call this method from a static method that has the [RuntimeInitializeOnLoadMethod] attribute.
        /// </summary>
        protected static void Initialize()
        {
            if (!InstanceInitialized)
            {
                Instance = new GameObject(typeof(T).Name).AddComponent<T>();
                CompleteInitialization();
            }
        }

        /// <summary>
        /// You probably mean to call <see cref="Initialize"/>.
        /// </summary>
        protected static void Initialize(GameObject prefab)
        {
            if (!InstanceInitialized)
            {
                Instance = Instantiate(prefab).GetComponent<T>();
                CompleteInitialization();
            }
        }

        /// <summary>
        /// You probably mean to call <see cref="Initialize"/>.
        /// </summary>
        protected static void Initialize(T component)
        {
            Instance = component;
            CompleteInitialization();
        }

        private static void CompleteInitialization()
        {
            DontDestroyOnLoad(Instance.gameObject);
            Instance.OnInitialize();
            InstanceInitialized = true;
        }

        public virtual void OnInitialize()
        {
            ThisInstanceInitialized = true;
        }
    }
}
