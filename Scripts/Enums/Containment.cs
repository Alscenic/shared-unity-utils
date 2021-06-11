using UnityEngine;

namespace CGenStudios.UnityUtils
{
	/// <summary>
	/// Containment flags.
	/// </summary>
	[System.Flags]
	public enum Containment
	{
		/// <summary>
		/// Not very useful but it's here
		/// </summary>
		None = 0,

		/// <summary>
		/// X value is to the left of the area
		/// </summary>
		XNegative = 1 << 0,

		/// <summary>
		/// X value is to the right of the area
		/// </summary>
		XPositive = 1 << 1,

		/// <summary>
		/// X value is contained within the area
		/// </summary>
		XContained = 1 << 2,

		/// <summary>
		/// Y value is above the area
		/// </summary>
		YNegative = 1 << 3,

		/// <summary>
		/// Y value is below the area
		/// </summary>
		YPositive = 1 << 4,

		/// <summary>
		/// Y value is contained within the area
		/// </summary>
		YContained = 1 << 5,

		/// <summary>
		/// Y value is behind the area
		/// </summary>
		ZNegative = 1 << 6,

		/// <summary>
		/// X value is in front of the area
		/// </summary>
		ZPositive = 1 << 7,

		/// <summary>
		/// X value is contained within the area
		/// </summary>
		ZContained = 1 << 8,

		/// <summary>
		/// Not contained = !(FullyContained)
		/// </summary>
		FullyContained = XContained | YContained | ZContained,
	}
}
