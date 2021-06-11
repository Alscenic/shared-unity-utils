using UnityEngine;

namespace CGenStudios.UnityUtils.Audio
{
	/// <summary>
	/// Created automatically at runtime.
	/// </summary>
	public class AudioManager : LazySingletonMonoBehaviour<AudioManager>
	{
		#region Public Indexers + Properties

		/// <summary>
		/// Gets the audio prefab.
		/// </summary>
		public GameObject AudioPrefab => this.m_AudioPrefab;

		#endregion

		#region Private Fields

		[SerializeField]
		private GameObject m_AudioPrefab = null;

		#endregion
	}
}
