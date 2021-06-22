// Code by Kyle Lamothe
// from current.gen Studios

using UnityEngine;

namespace CGenStudios.UnityUtils.Framework
{
	/// <summary>
	/// The UniqueID.
	/// </summary>
	[AddComponentMenu("Alscenic/Unity Utils/Framework/Unique ID")]
	[ExecuteAlways]
	[DisallowMultipleComponent]
	public class UniqueID : MonoBehaviour
	{
		public int ID => this.m_ID;

		[SerializeField]
		private int m_ID = 0;

		private void OnEnable()
		{
			if (!Application.isPlaying)
			{
				UpdateID();
			}
		}

		private void UpdateID()
		{
			this.m_ID = gameObject.GetInstanceID();
		}
	}
}
