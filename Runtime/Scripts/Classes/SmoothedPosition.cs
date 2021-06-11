// Code by Kyle Lamothe
// from current.gen Studios

using UnityEngine;

namespace CGenStudios.UnityUtils
{
	/// <summary>
	/// A smoothed Vector3.
	/// </summary>
	public class SmoothedPosition
	{

		#region Public Indexers + Properties

		/// <summary>
		/// Gets or sets the value.
		/// </summary>
		public Vector3 Value
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
		/// Gets the position difference between where we're at and where we're going.
		/// </summary>
		public Vector3 Difference => this.m_TargetValue - this.m_MovingValue;

		#endregion

		#region Private Fields

		private Vector3 m_TargetValue = new Vector3();

		private Vector3 m_MovingValue = new Vector3();

		private Vector3 m_SmoothedValue = new Vector3();

		#endregion

		#region Public Constructors + Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="SmoothedPosition"/> class.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="speed">The speed.</param>
		/// <param name="smoothness">The smoothness.</param>
		public SmoothedPosition(Vector3 value,float speed,float smoothness)
		{
			Jump(value);
			Speed = speed;
			Smoothness = smoothness;
			UnityUtilsManager.OnUpdate.AddListener(Update);
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Instantly moves to the specified position.
		/// </summary>
		/// <param name="value">The value.</param>
		public void Jump(Vector3 value)
		{
			this.m_TargetValue = value;
			Jump();
		}

		/// <summary>
		/// Instantly moves to the target position.
		/// </summary>
		public void Jump()
		{
			this.m_MovingValue = this.m_TargetValue;
			this.m_SmoothedValue = this.m_TargetValue;
		}

		/// <summary>
		/// Disposes the SmoothedVector3.
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
			this.m_MovingValue = Vector3.MoveTowards(this.m_MovingValue,this.m_TargetValue,Time.deltaTime * Speed);
			this.m_SmoothedValue = Vector3.Lerp(this.m_SmoothedValue,this.m_MovingValue,Time.deltaTime * Smoothness);
		}

		#endregion

	}
}
