// Code by Kyle Lamothe
// from current.gen Studios

using UnityEngine;

namespace CGenStudios.UnityUtils.CompositeClasses
{
	/// <summary>
	/// A composite of 4 AnimationCurves to create what is essentially a "Vector4 curve."
	/// Think of it like the curves used in ParticleSystems.
	/// </summary>
	[System.Serializable]
	public class VectorCurve
	{
		#region Public Indexers + Properties

		/// <summary>
		/// Gets the x curve.
		/// </summary>
		public AnimationCurve X => this.m_X;

		/// <summary>
		/// Gets the y curve.
		/// </summary>
		public AnimationCurve Y => this.m_Y;

		/// <summary>
		/// Gets the z curve.
		/// </summary>
		public AnimationCurve Z => this.m_Z;

		/// <summary>
		/// Gets the w curve.
		/// </summary>
		public AnimationCurve W => this.m_W;

		#endregion

		#region Private Fields

		[SerializeField]
		private AnimationCurve m_X = new AnimationCurve();

		[SerializeField]
		private AnimationCurve m_Y = new AnimationCurve();

		[SerializeField]
		private AnimationCurve m_Z = new AnimationCurve();

		[SerializeField]
		private AnimationCurve m_W = new AnimationCurve();

		#endregion

		#region Public Methods

		/// <summary>
		/// Evaluates as a Vector2.
		/// </summary>
		/// <param name="t">The time.</param>
		/// <returns>A Vector2.</returns>
		public Vector2 EvaluateAsVector2(float t)
		{
			return new Vector2(this.m_X.Evaluate(t),this.m_Y.Evaluate(t));
		}

		/// <summary>
		/// Evaluates as a Vector3.
		/// </summary>
		/// <param name="t">The time.</param>
		/// <returns>A Vector2.</returns>
		public Vector3 EvaluateAsVector3(float t)
		{
			return new Vector3(this.m_X.Evaluate(t),this.m_Y.Evaluate(t),this.m_Z.Evaluate(t));
		}

		/// <summary>
		/// Evaluates as a Vector4.
		/// </summary>
		/// <param name="t">The time.</param>
		/// <returns>A Vector2.</returns>
		public Vector4 EvaluateAsVector4(float t)
		{
			return new Vector4(this.m_X.Evaluate(t),this.m_Y.Evaluate(t),this.m_Z.Evaluate(t),this.m_W.Evaluate(t));
		}

		/// <summary>
		/// Evaluates as a Color.
		/// </summary>
		/// <param name="t">The time.</param>
		/// <returns>A Vector2.</returns>
		public Color EvaluateAsColor(float t)
		{
			return new Color(this.m_X.Evaluate(t),this.m_Y.Evaluate(t),this.m_Z.Evaluate(t),this.m_W.Evaluate(t));
		}

		#endregion
	}
}
