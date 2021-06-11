using UnityEngine;

namespace CGenStudios.UnityUtils
{
	/// <summary>
	/// Various extensions.
	/// </summary>
	public static class Extensions
	{

		#region Public Methods

		public static Vector3 SetYZ(this Vector3 vector,float y,float z)
		{
			return new Vector3(vector.x,y,z);
		}

		public static Vector3 SetXZ(this Vector3 vector,float x,float z)
		{
			return new Vector3(x,vector.y,z);
		}

		public static Vector3 SetXY(this Vector3 vector,float x,float y)
		{
			return new Vector3(x,y,vector.z);
		}

		#endregion
	}
}
