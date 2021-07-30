using UnityEngine;

namespace CGenStudios.UnityUtils.Audio
{
    /// <summary>
    /// Created automatically at runtime.
    /// </summary>
    public class AudioManager : SingletonMonoBehaviour<AudioManager>
    {
        /// <summary>
        /// Gets the audio prefab.
        /// </summary>
        public GameObject AudioPrefab { get; private set; } = null;

        [RuntimeInitializeOnLoadMethod]
        private static void RuntimeInitializeOnLoadMethod()
        {
            Initialize();
        }

        public override void OnInitialize()
        {
            base.OnInitialize();

            AudioPrefab = Resources.Load<GameObject>("alscenicUtils_audio");
        }
    }
}
