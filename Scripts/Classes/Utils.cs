using UnityEngine;

namespace CGenStudios.UnityUtils
{
	/// <summary>
	/// Misc utilities.
	/// </summary>
	public static class Utils
	{
		/// <summary>
		/// Returns a little more than just if a point is inside a bounds.
		/// </summary>
		/// <param name="bounds">The bounds.</param>
		/// <param name="pos">The pos.</param>
		/// <returns>A Containment enum.</returns>
		public static Containment CheckContainment(Bounds bounds,Vector3 pos)
		{
			Containment containment = Containment.None;

			if (pos.x < -bounds.extents.x + bounds.center.x)
			{
				containment |= Containment.XNegative;
			}
			else if (pos.x > bounds.extents.x + bounds.center.x)
			{
				containment |= Containment.XPositive;
			}
			else
			{
				containment |= Containment.XContained;
			}

			if (pos.y < -bounds.extents.y + bounds.center.y)
			{
				containment |= Containment.YNegative;
			}
			else if (pos.y > bounds.extents.y + bounds.center.y)
			{
				containment |= Containment.YPositive;
			}
			else
			{
				containment |= Containment.YContained;
			}

			if (pos.z < -bounds.extents.z + bounds.center.z)
			{
				containment |= Containment.YNegative;
			}
			else if (pos.z > bounds.extents.z + bounds.center.z)
			{
				containment |= Containment.YPositive;
			}
			else
			{
				containment |= Containment.YContained;
			}

			return containment;
		}

		/// <summary>
		/// Creates any singleton MonoBehaviour.
		/// </summary>
		/// <returns>Any MonoBehaviour.</returns>
		public static T CreateSingletonMonoBehaviour<T>() where T : MonoBehaviour
		{
			return new GameObject(typeof(T).Name).AddComponent<T>();
		}
	}
}
