using UnityEngine;

namespace CGenStudios.UnityUtils.Audio
{
	/// <summary>
	/// Not meant to be manually added as a component.
	/// </summary>
	public class Audio : MonoBehaviour
	{

		#region Public Indexers + Properties

		public AudioSource Source => this.m_Source;

		#endregion

		#region Private Fields

		[SerializeField]
		private AudioSource m_Source = null;

		#endregion

		#region Public Methods

		public static Audio Play(AudioClip clip)
		{
			return Play(clip,0.0f,1.0f,0.0f);
		}

		public static Audio Play(AudioClip clip,float pan,float pitch,float pitchRange)
		{
			Audio audio = Instantiate(AudioManager.Instance.AudioPrefab).GetComponent<Audio>();

			audio.Source.clip = clip;
			audio.Source.panStereo = pan;
			audio.Source.pitch = pitch + (Random.value * 2.0f - 1.0f) * pitchRange;

			audio.Source.Play();

			return audio;
		}

		#endregion

		#region Private Methods

		private void Update()
		{
			if (!Source.isPlaying)
			{
				Destroy(gameObject);
			}
		}

		#endregion
	}
}
