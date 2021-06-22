// Code by Kyle Lamothe
// from current.gen Studios

using UnityEngine;

namespace CGenStudios.JustBusiness
{
	/// <summary>
	/// A collection of ParticleSystems.
	/// </summary>
	[AddComponentMenu("Alscenic/Unity Utils/Effects/Effect Collection")]
	public class EffectCollection : MonoBehaviour
	{
		[SerializeField]
		private ParticleSystem[] m_ParticleSystems = new ParticleSystem[] { };

		/// <summary>
		/// Gets the particle systems.
		/// </summary>
		public ParticleSystem[] ParticleSystems => this.m_ParticleSystems;

		public void ForEach(System.Action<ParticleSystem> action)
		{
			for (int i = 0; i < this.m_ParticleSystems.Length; i++)
			{
				action?.Invoke(this.m_ParticleSystems[i]);
			}
		}
	}
}
