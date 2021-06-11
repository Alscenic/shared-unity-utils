// Code by Kyle Lamothe
// from current.gen Studios

using UnityEngine;

namespace CGenStudios.UnityUtils.CompositeClasses
{
	/// <summary>
	/// Keeps a position and rotation interpolated perfectly in sync with each other.
	/// </summary>
	public class SmoothedTransform
	{

		#region Public Indexers + Properties

		/// <summary>
		/// Gets or sets the position.
		/// </summary>
		public Vector3 Position
		{
			get => SmoothedPosition.Value;
			set => SmoothedPosition.Value = value;
		}

		/// <summary>
		/// Gets the rotation.
		/// </summary>
		public Quaternion Rotation
		{
			get => this.m_SmoothedRotation;
			set => this.m_TargetRotation = value;
		}

		/// <summary>
		/// Gets or sets the speed.
		/// </summary>
		public float Speed
		{
			get => SmoothedPosition.Speed;
			set => SmoothedPosition.Speed = value;
		}

		/// <summary>
		/// Gets or sets the smoothness.
		/// </summary>
		public float Smoothness
		{
			get => SmoothedPosition.Smoothness;
			set => SmoothedPosition.Smoothness = value;
		}

		#endregion

		#region Private Indexers + Properties

		/// <summary>
		/// Gets the position.
		/// </summary>
		private SmoothedPosition SmoothedPosition { get; set; } = null;

		/// <summary>
		/// Gets the rotation speed.
		/// </summary>
		private float RotationSpeed => (Quaternion.Angle(this.m_MovingRotation,this.m_TargetRotation) / SmoothedPosition.Difference.magnitude) * Speed;

		#endregion

		#region Private Fields

		private Quaternion m_TargetRotation = Quaternion.identity;

		private Quaternion m_MovingRotation = Quaternion.identity;

		private Quaternion m_SmoothedRotation = Quaternion.identity;

		#endregion

		#region Public Constructors + Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="SmoothedTransform"/> class.
		/// </summary>
		/// <param name="position">The initial position.</param>
		/// <param name="rotation">The initial rotation.</param>
		/// <param name="speed">The speed.</param>
		/// <param name="smoothness">The smoothness.</param>
		public SmoothedTransform(Vector3 position,Quaternion rotation,float speed,float smoothness)
		{
			SmoothedPosition = new SmoothedPosition(position,speed,smoothness);
			Jump(position,rotation);
			UnityUtilsManager.OnUpdate.AddListener(Update);
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Instantly moves the position and rotation values to their respective targets.
		/// </summary>
		public void Jump()
		{
			this.SmoothedPosition.Jump();
			this.m_MovingRotation = this.m_TargetRotation;
			this.m_SmoothedRotation = this.m_TargetRotation;
		}

		/// <summary>
		/// Instantly moves to the specified position and rotation.
		/// </summary>
		/// <param name="position">The position.</param>
		/// <param name="rotation">The rotation.</param>
		public void Jump(Vector3 position,Quaternion rotation)
		{
			SmoothedPosition.Jump(position);

			this.m_TargetRotation = rotation;
			Jump();
		}

		/// <summary>
		/// Disposes the SmoothedTransform.
		/// </summary>
		public void Dispose()
		{
			SmoothedPosition.Dispose();
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Update.
		/// </summary>
		private void Update()
		{
			this.m_MovingRotation = Quaternion.RotateTowards(this.m_MovingRotation,this.m_TargetRotation,RotationSpeed * Time.deltaTime);
			this.m_SmoothedRotation = Quaternion.Lerp(this.m_SmoothedRotation,this.m_MovingRotation,Smoothness * Time.deltaTime);
		}

		#endregion

	}
}
