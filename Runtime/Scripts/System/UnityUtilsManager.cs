using UnityEngine;
using UnityEngine.Events;

namespace CGenStudios.UnityUtils
{
	/// <summary>
	/// Created automatically at runtime.
	/// </summary>
	public sealed class UnityUtilsManager : LazySingletonMonoBehaviour<UnityUtilsManager>
	{

		#region Public Indexers + Properties

		public static UnityEvent OnUpdate { get; } = new UnityEvent();

		#endregion

		#region Private Methods

		private void Update()
		{
			OnUpdate.Invoke();
		}

		#endregion
	}
}
