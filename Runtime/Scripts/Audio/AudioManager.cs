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
		public GameObject AudioPrefab => Resources.Load<GameObject>("alscenicUtils_audio");

		#endregion
	}
}
