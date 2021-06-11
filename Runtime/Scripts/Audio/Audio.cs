using UnityEngine;
using CGenStudios.UnityUtils.CompositeClasses;

namespace CGenStudios.UnityUtils.Audio
{
	/// <summary>
	/// Not meant to be manually added as a component.
	/// </summary>
	public class Audio : MonoBehaviour
	{

		#region Public Classes + Interfaces + Structs

		/// <summary>
		/// The AudioData.
		/// </summary>
		[System.Serializable]
		public class AudioData
		{

			#region Public Indexers + Properties

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

			#endregion

			#region Private Fields

			[SerializeField]
			private AudioClip[] m_Clips = new AudioClip[] { };

			[SerializeField]
			private RangedFloat m_Volume = new RangedFloat(1.0f,1.0f);

			[SerializeField]
			private RangedFloat m_Pitch = new RangedFloat(1.0f,1.0f);

			[SerializeField]
			private bool m_IsPositional = true;

			[SerializeField]
			private bool m_PlayOnAwake = true;

			[SerializeField]
			private bool m_Loop = false;

			[SerializeField]
			private RangedFloat m_MaxDistance = new RangedFloat(500.0f,500.0f);

			#endregion
		}

		#endregion

		#region Public Indexers + Properties

		/// <summary>
		/// Gets the AudioData.
		/// </summary>
		public AudioData Data { get; private set; }

		#endregion

		#region Private Indexers + Properties

		/// <summary>
		/// Gets or sets a value indicating whether this audio has played.
		/// </summary>
		private bool HasPlayed { get; set; } = false;

		#endregion

		#region Private Fields

		[SerializeField]
		private AudioSource m_Source = null;

		#endregion

		#region Public Methods

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
			this.m_Source.Play();
			HasPlayed = true;
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Updates the source.
		/// </summary>
		private void UpdateSource()
		{
			this.m_Source.clip = Data.Clips[Mathf.FloorToInt(Random.value * Data.Clips.Length)];
			this.m_Source.loop = Data.Loop;
			this.m_Source.volume = Data.Volume.Value;
			this.m_Source.pitch = Data.Pitch.Value;
			this.m_Source.spatialBlend = Data.IsPositional ? 1.0f : 0.0f;
			this.m_Source.maxDistance = Data.MaxDistance.Value;
		}

		/// <summary>
		/// </summary>
		private void Update()
		{
			if (HasPlayed && !this.m_Source.loop && !this.m_Source.isPlaying)
			{
				Destroy(gameObject);
			}
		}

		#endregion

	}
}
