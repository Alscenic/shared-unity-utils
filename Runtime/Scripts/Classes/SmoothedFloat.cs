// Code by Kyle Lamothe
// from current.gen Studios

using UnityEngine;

namespace CGenStudios.UnityUtils
{
	/// <summary>
	/// A smoothed Vector3.
	/// </summary>
	public class SmoothedFloat
	{

		#region Public Indexers + Properties

		/// <summary>
		/// Gets or sets the value.
		/// </summary>
		public float Value
		{
			get => this.m_SmoothedValue;
			set => this.m_TargetValue = value;
		}

		/// <summary>
		/// Gets or sets the smoothness.
		/// </summary>
		public float Smoothness { get; set; } = 1.0f;

		/// <summary>
		/// Gets or sets the speed.
		/// </summary>
		public float Speed { get; set; } = 1.0f;

		/// <summary>
		/// Gets the difference between where we're at and where we're going.
		/// </summary>
		public float Difference => this.m_TargetValue - this.m_MovingValue;

		#endregion

		#region Private Fields

		private float m_TargetValue = 0.0f;

		private float m_MovingValue = 0.0f;

		private float m_SmoothedValue = 0.0f;

		#endregion

		#region Public Constructors + Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="SmoothedFloat"/> class.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="speed">The speed.</param>
		/// <param name="smoothness">The smoothness.</param>
		public SmoothedFloat(float value,float speed,float smoothness)
		{
			Jump(value);
			Speed = speed;
			Smoothness = smoothness;
			UnityUtilsManager.OnUpdate.AddListener(Update);
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Instantly jumps to the specified value.
		/// </summary>
		/// <param name="value">The value.</param>
		public void Jump(float value)
		{
			this.m_TargetValue = value;
			Jump();
		}

		/// <summary>
		/// Instantly jumps to the target value.
		/// </summary>
		public void Jump()
		{
			this.m_MovingValue = this.m_TargetValue;
			this.m_SmoothedValue = this.m_TargetValue;
		}

		/// <summary>
		/// Disposes the SmoothedFloat.
		/// </summary>
		public void Dispose()
		{
			UnityUtilsManager.OnUpdate.RemoveListener(Update);
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Update.
		/// </summary>
		private void Update()
		{
			this.m_MovingValue = Mathf.MoveTowards(this.m_MovingValue,this.m_TargetValue,Time.deltaTime * Speed);
			this.m_SmoothedValue = Mathf.Lerp(this.m_SmoothedValue,this.m_MovingValue,Time.deltaTime * Smoothness);
		}

		#endregion

	}
}
