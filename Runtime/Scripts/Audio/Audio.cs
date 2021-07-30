using CGenStudios.UnityUtils.CompositeClasses;
using UnityEngine;

namespace CGenStudios.UnityUtils.Audio
{
    /// <summary>
    /// Not meant to be manually added as a component.
    /// </summary>
    public class Audio : MonoBehaviour
    {
        /// <summary>
        /// The AudioData.
        /// </summary>
        [System.Serializable]
        public class AudioData
        {
            /// <summary>
            /// Gets or sets the clips.
            /// </summary>
            public AudioClip[] Clips { get => this.m_Clips; set => this.m_Clips = value; }

            /// <summary>
            /// Gets or sets the volume.
            /// </summary>
            public RangedFloat Volume { get => this.m_Volume; set => this.m_Volume = value; }

            /// <summary>
            /// Gets or sets the pitch.
            /// </summary>
            public RangedFloat Pitch { get => this.m_Pitch; set => this.m_Pitch = value; }

            /// <summary>
            /// Gets or sets a value indicating whether this AudioData is positional.
            /// </summary>
            public bool IsPositional { get => this.m_IsPositional; set => this.m_IsPositional = value; }

            /// <summary>
            /// Gets or sets a value indicating whether to play on awake.
            /// </summary>
            public bool PlayOnAwake { get => this.m_PlayOnAwake; set => this.m_PlayOnAwake = value; }

            /// <summary>
            /// Gets or sets a value indicating whether loop.
            /// </summary>
            public bool Loop { get => this.m_Loop; set => this.m_Loop = value; }

            /// <summary>
            /// Gets the max distance.
            /// </summary>
            public RangedFloat MaxDistance { get => this.m_MaxDistance; set => this.m_MaxDistance = value; }

            public RangedFloat MinDistance { get => m_MinDistance; set => m_MinDistance = value; }

            [SerializeField]
            private AudioClip[] m_Clips = new AudioClip[] { };

            [SerializeField]
            private RangedFloat m_Volume = new RangedFloat(1.0f, 1.0f);

            [SerializeField]
            private RangedFloat m_Pitch = new RangedFloat(1.0f, 1.0f);

            [SerializeField]
            private bool m_IsPositional = true;

            [SerializeField]
            private bool m_PlayOnAwake = true;

            [SerializeField]
            private bool m_Loop = false;

            [SerializeField]
            private RangedFloat m_MinDistance = new RangedFloat(0.0f, 0.0f);

            [SerializeField]
            private RangedFloat m_MaxDistance = new RangedFloat(500.0f, 500.0f);

            public AudioData() { }

            public AudioData(AudioClip[] clips, RangedFloat volume, RangedFloat pitch, bool isPositional, bool playOnAwake, bool loop, RangedFloat maxDistance, RangedFloat minDistance)
            {
                m_Clips = clips;
                m_Volume = volume;
                m_Pitch = pitch;
                m_IsPositional = isPositional;
                m_PlayOnAwake = playOnAwake;
                m_Loop = loop;
                m_MaxDistance = maxDistance;
                m_MinDistance = minDistance;
            }
        }

        /// <summary>
        /// Gets the AudioData.
        /// </summary>
        public AudioData Data { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this audio has played.
        /// </summary>
        private bool HasPlayed { get; set; } = false;

        public AudioSource Source => m_Source;

        [SerializeField]
        private AudioSource m_Source = null;

        /// <summary>
        /// Plays an AudioClip.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>An Audio.</returns>
        public static Audio Play(AudioData data)
        {
            Audio audio = Instantiate(AudioManager.Instance.AudioPrefab).GetComponent<Audio>();

            audio.Data = data;
            audio.UpdateSource();

            if (audio.Data.PlayOnAwake)
            {
                audio.Play();
            }

            return audio;
        }

        /// <summary>
        /// Plays multiple AudioClips.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>An array of Audios.</returns>
        public static Audio[] Play(params AudioData[] data)
        {
            Audio[] audios = new Audio[data.Length];

            for (int i = 0; i < data.Length; i++)
            {
                audios[i] = Play(data[i]);
            }

            return audios;
        }

        /// <summary>
        /// Plays the AudioClip.
        /// </summary>
        public void Play()
        {
            this.Source.Play();
            HasPlayed = true;
        }

        /// <summary>
        /// Updates the source.
        /// </summary>
        private void UpdateSource()
        {
            this.Source.clip = Data.Clips[Mathf.FloorToInt(Random.Range(0, Data.Clips.Length))];
            this.Source.loop = Data.Loop;
            this.Source.volume = Data.Volume.Value;
            this.Source.pitch = Data.Pitch.Value;
            this.Source.spatialBlend = Data.IsPositional ? 1.0f : 0.0f;
            this.Source.minDistance = Data.MinDistance.Value;
            this.Source.maxDistance = Data.MaxDistance.Value;
        }

        /// <summary>
        /// </summary>
        private void Update()
        {
            if (HasPlayed && !this.Source.loop && !this.Source.isPlaying)
            {
                Destroy(gameObject);
            }
        }
    }
}
