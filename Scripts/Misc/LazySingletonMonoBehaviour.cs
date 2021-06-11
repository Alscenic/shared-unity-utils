using UnityEngine;

namespace CGenStudios.UnityUtils
{
	/// <summary>
	/// Used when you want a singleton class that works like a <see cref="MonoBehaviour"/>.
	///
	/// Inherit from this class and use your new class as the generic (<typeparamref name="T"/>).
	///
	/// Do not put components that inherit from this class into your scene, as they will be created
	/// automatically at run-time.
	/// </summary>
	public class LazySingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
	{
		#region Public Indexers + Properties

		/// <summary>
		/// Gets this class's singleton MonoBehaviour.
		/// </summary>
		public static T Instance
		{
			get
			{
				if (!instance)
				{
					Instance = Utils.CreateSingletonMonoBehaviour<T>();
				}

				return instance;
			}

			private set => instance = value;
		}

		#endregion

		#region Protected Fields

		[SerializeField]
		[TextArea]
		private string m_LazySingletonInfo = LAZY_SINGLETON_INFO;

		/// <summary>
		/// </summary>
		private const string LAZY_SINGLETON_INFO = "This script is not meant to be manually " +
			"added to GameObjects, as they will be created automatically at run-time. Leaving " +
			"this script on an object will cause at least one duplicate.";

		protected static T instance = null;

		protected virtual void OnValidate()
		{
			this.m_LazySingletonInfo = LAZY_SINGLETON_INFO;
		}

		#endregion
	}
}
