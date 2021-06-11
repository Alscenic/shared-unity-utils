// Code by Kyle Lamothe
// from current.gen Studios

using UnityEngine;

namespace CGenStudios.RadMags
{

	/// <summary>
	/// The RangedFloat.
	/// </summary>
	[System.Serializable]
	public class RangedFloat
	{

		#region Public Indexers + Properties

		/// <summary>
		/// Gets the range.
		/// </summary>
		public float Range
		{
			get => Max - Min;
		}

		/// <summary>
		/// Gets or sets the median.
		/// </summary>
		public float Median
		{
			get => Mathf.Lerp(Min,Max,0.5f);

			set
			{
				float diff = value - Median;
				Min += diff;
				Max += diff;
			}
		}

		/// <summary>
		/// Gets the value.
		/// </summary>
		public float Value => Random.Range(Min,Max);

		/// <summary>
		/// Gets or sets the min.
		/// </summary>
		public float Min { get => this.m_Min; set => this.m_Min = value; }

		/// <summary>
		/// Gets or sets the max.
		/// </summary>
		public float Max { get => this.m_Max; set => this.m_Max = value; }

		#endregion

		#region Private Fields

		[SerializeField]
		private float m_Min = 0.0f;

		[SerializeField]
		private float m_Max = 1.0f;

		#endregion

		#region Public Constructors + Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="RangedFloat"/> class.
		/// </summary>
		/// <param name="min">The min.</param>
		/// <param name="max">The max.</param>
		public RangedFloat(float min,float max)
		{
			Min = min;
			Max = max;
		}

		#endregion
	}
}
