// Code by Kyle Lamothe
// from current.gen Studios

using UnityEngine;

namespace CGenStudios.UnityUtils.CompositeClasses
{
	/// <summary>
	/// A position, rotation, and scale conveniently compressed into one composite class.
	/// </summary>
	[System.Serializable]
	public struct SpaceData
	{
		#region Public Indexers + Properties

		/// <summary>
		/// Gets the world origin as a <see cref="SpaceData"/>.
		/// </summary>
		public static SpaceData Origin => new SpaceData(Vector3.zero,Quaternion.identity,Vector3.one);

		/// <summary>
		/// Gets or sets the position.
		/// </summary>
		public Vector3 Position { get; set; }

		/// <summary>
		/// Gets or sets the rotation.
		/// </summary>
		public Quaternion Rotation { get; set; }

		/// <summary>
		/// Gets or sets the scale.
		/// </summary>
		public Vector3 Scale { get; set; }

		#endregion

		#region Public Constructors + Destructors

		/// <summary>
		/// Initializes a new <see cref="SpaceData"/>.
		/// </summary>
		/// <param name="position">The position.</param>
		/// <param name="rotation">The rotation.</param>
		/// <param name="scale">The scale.</param>
		public SpaceData(Vector3 position,Quaternion rotation,Vector3 scale)
		{
			Position = position;
			Rotation = rotation;
			Scale = scale;
		}

		/// <summary>
		/// Initializes a new <see cref="SpaceData"/>.
		/// </summary>
		/// <param name="position">The position.</param>
		public SpaceData(Vector3 position) : this(position,Quaternion.identity,Vector3.one) { }

		/// <summary>
		/// Initializes a new <see cref="SpaceData"/>.
		/// </summary>
		/// <param name="rotation">The rotation.</param>
		public SpaceData(Quaternion rotation) : this(Vector3.zero,rotation,Vector3.one) { }

		/// <summary>
		/// Initializes a new <see cref="SpaceData"/>.
		/// </summary>
		/// <param name="position">The position.</param>
		/// <param name="rotation">The rotation.</param>
		public SpaceData(Vector3 position,Quaternion rotation) : this(position,rotation,Vector3.one) { }

		/// <summary>
		/// Initializes a new <see cref="SpaceData"/>.
		/// </summary>
		/// <param name="transform">The transform.</param>
		/// <param name="world">If true, world.</param>
		public SpaceData(Transform transform,bool world) : this(
			world ? transform.position : transform.localPosition,
			world ? transform.rotation : transform.localRotation,
			transform.localScale)
		{ }

		#endregion

		#region Public Methods

		public static bool operator ==(SpaceData lh,SpaceData rh)
		{
			return lh.Position == rh.Position && lh.Rotation == rh.Rotation && lh.Scale == rh.Scale;
		}

		public static bool operator !=(SpaceData lh,SpaceData rh)
		{
			return !(lh == rh);
		}

		/// <summary>
		/// </summary>
		public override bool Equals(object obj)
		{
			return obj is SpaceData data &&
				   Position.Equals(data.Position) &&
				   Rotation.Equals(data.Rotation) &&
				   Scale.Equals(data.Scale);
		}

		/// <summary>
		/// </summary>
		public override int GetHashCode()
		{
			int hashCode = 1352853554;
			hashCode = hashCode * -1521134295 + Position.GetHashCode();
			hashCode = hashCode * -1521134295 + Rotation.GetHashCode();
			hashCode = hashCode * -1521134295 + Scale.GetHashCode();
			return hashCode;
		}

		#endregion

	}
}
