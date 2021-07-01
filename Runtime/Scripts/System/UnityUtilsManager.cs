using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

namespace CGenStudios.UnityUtils
{
    /// <summary>
    /// Created automatically at runtime.
    /// </summary>
    public sealed class UnityUtilsManager : SingletonMonoBehaviour<UnityUtilsManager>
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void RuntimeInitializeOnLoadMethod()
        {
            Initialize();
        }

        public static UnityEvent OnUpdate { get; } = new UnityEvent();

        private void Update()
        {
            OnUpdate.Invoke();
        }
    }
}
