using UnityEngine;

namespace CGenStudios.UnityUtils
{
	/// <summary>
	///	Destroys a GameObject, or itself if one isn't specified.
	/// </summary>
	[AddComponentMenu("Alscenic/Unity Utils/Misc/Destroyer")]
	public class Destroyer : MonoBehaviour
	{

		#region Private Fields

		[Header("Destroys a GameObject, or itself if one isn't specified.")]
		[SerializeField]
		private GameObject m_DestroyObject = null;

		#endregion

		#region Public Methods

		public void Activate()
		{
			// yes unity3d visual studio extension, I know that the null coalescing operator shouldn't be used on unity objects.
			// (but it's what I want in this situation)
			Destroy(this.m_DestroyObject ?? gameObject);
		}

		public void Activate(GameObject obj)
		{
			// same goes for this one
			Destroy(obj ?? gameObject);
		}

		#endregion
	}
}
