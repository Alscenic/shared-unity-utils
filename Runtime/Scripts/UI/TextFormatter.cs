// Code by Kyle Lamothe
// from current.gen Studios

using UnityEngine;
using CGenStudios.UnityUtils.UnityEvents;

namespace CGenStudios.UnityUtils.UI
{
	/// <summary>
	/// The TextFormatter.
	/// </summary>
	[AddComponentMenu("Alscenic/Unity Utils/UI/TextFormatter")]
	public class TextFormatter : MonoBehaviour
	{
		#region Public Classes + Interfaces + Structs

		/// <summary>
		/// The formatted string.
		/// </summary>
		[System.Serializable]
		public class StringFormatter
		{
			#region Public Enums

			/// <summary>
			/// The thresh type.
			/// </summary>
			public enum ThreshType
			{
				Always,

				Less,

				LessOrEqual,

				Greater,

				GreaterOrEqual,

				Equal,

				NotEqual,

				Never,
			}

			#endregion

			#region Public Indexers + Properties

			/// <summary>
			/// Gets the thresh mode.
			/// </summary>
			public ThreshType ThreshMode => this.m_ThreshMode;

			#endregion

			#region Private Fields

			[SerializeField]
			private string m_PreString = "";

			[SerializeField]
			private string m_PostString = "";

			[SerializeField]
			private string m_ReplaceString = "";

			[SerializeField]
			private float m_Thresh = 0.0f;

			[SerializeField]
			private float m_NumberMultiplier = 1.0f;

			[SerializeField]
			private string m_NumberFormat = "";

			[SerializeField]
			private ThreshType m_ThreshMode = ThreshType.Always;

			#endregion

			#region Public Methods

			/// <summary>
			/// Formats the input string.
			/// </summary>
			/// <param name="str">The str.</param>
			public string Format(float n)
			{
				return Format((n * this.m_NumberMultiplier).ToString(this.m_NumberFormat));
			}

			/// <summary>
			/// Formats the input string.
			/// </summary>
			/// <param name="str">The str.</param>
			public string Format(string str)
			{
				if (!string.IsNullOrEmpty(this.m_ReplaceString))
					str = this.m_ReplaceString;

				return this.m_PreString + str + this.m_PostString;
			}

			/// <summary>
			/// Tests a threshold value.
			/// </summary>
			/// <param name="thresh">The thresh.</param>
			/// <returns>A bool.</returns>
			public bool TestThresh(float thresh)
			{
				switch (ThreshMode)
				{
					case ThreshType.Always:
						return true;

					case ThreshType.Less:
						return thresh < this.m_Thresh;

					case ThreshType.LessOrEqual:
						return thresh <= this.m_Thresh;

					case ThreshType.Greater:
						return thresh > this.m_Thresh;

					case ThreshType.GreaterOrEqual:
						return thresh >= this.m_Thresh;

					case ThreshType.Equal:
						return thresh == this.m_Thresh;

					case ThreshType.NotEqual:
						return thresh != this.m_Thresh;

					case ThreshType.Never:
						return false;

					default:
						goto case ThreshType.Never;
				}
			}

			#endregion
		}

		#endregion

		#region Private Fields

		[SerializeField]
		private StringFormatter m_BaseFormatter = new StringFormatter();

		[SerializeField]
		private UnityEventString m_Event = new UnityEventString();

		[SerializeField]
		private StringFormatter[] m_StringFormatters = new StringFormatter[] { };

		#endregion

		#region Public Methods

		/// <summary>
		/// Formats the input string and sends it to the event.
		/// </summary>
		/// <param name="str">The str.</param>
		public void FormatAndSend(string str)
		{
			Send(this.m_BaseFormatter.Format(str));
		}

		/// <summary>
		/// Sends the string.
		/// </summary>
		/// <param name="str">The str.</param>
		public void Send(string str)
		{
			this.m_Event.Invoke(str);
		}

		/// <summary>
		/// Formats the input and sends it to the event.
		/// </summary>
		/// <param name="str">The str.</param>
		public void FormatAndSend(float n)
		{
			Send(Format(n));
		}

		/// <summary>
		/// Formats a float.
		/// </summary>
		/// <param name="n">The n.</param>
		/// <returns>A string.</returns>
		public string Format(float n)
		{
			for (int i = 0; i < this.m_StringFormatters.Length; i++)
			{
				if (this.m_StringFormatters[i].TestThresh(n))
				{
					return this.m_StringFormatters[i].Format(n);
				}
			}

			return this.m_BaseFormatter.Format(n);
		}

		/// <summary>
		/// Formats the input and sends it to the event.
		/// </summary>
		/// <param name="str">The str.</param>
		public void FormatAndSend(int n)
		{
			FormatAndSend((float)n);
		}

		#endregion
	}
}
