// Code by Kyle Lamothe
// from current.gen Studios

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using CGenStudios.UnityUtils.UnityEvents;

namespace CGenStudios.UnityUtils.Input
{
	/// <summary>
	/// "Converts" <see cref="InputAction"/>s into <see cref="UnityEvent"/>s.
	/// Meant to be added to a GameObject or prefab as a component, otherwise you may as well just
	/// use the InputSystem shit directly. But you do you, whatever works best in your situation.
	/// </summary>
	[AddComponentMenu("Alscenic/Unity Utils/Input/Player Input")]
	public class PlayerInput : MonoBehaviour
	{

		#region Public Classes + Interfaces + Structs

		/// <summary>
		/// A director.
		/// </summary>
		[System.Serializable]
		public class Director
		{

			#region Public Indexers + Properties

			/// <summary>
			/// Gets the name.
			/// </summary>
			public string Name => Action?.name ?? "";

			/// <summary>
			/// Gets the value type.
			/// </summary>
			public string ValueType => Action?.expectedControlType ?? "invalid";

			/// <summary>
			/// Gets the action.
			/// </summary>
			public InputAction Action => this.m_Action?.action ?? null;

			/// <summary>
			/// Gets the cursor lock state check type.
			/// </summary>
			public CursorLockStateCheckType CursorLockStateCheckType => this.m_CursorLockStateCheckType;

			#endregion

			#region Private Indexers + Properties

			/// <summary>
			/// Gets or sets a value indicating whether the button action is pressed.
			/// </summary>
			private bool Pressed { get; set; } = false;

			#endregion

			#region Private Fields

			//[BoxGroup("$Name")]
			[SerializeField]
			private InputActionReference m_Action = null;

			//[BoxGroup("$Name")]
			[SerializeField]
			private CursorLockStateCheckType m_CursorLockStateCheckType = CursorLockStateCheckType.Any;

			//[BoxGroup("$Name")]
			//[ShowIf("@ValueType != \"Button\"")]
			[SerializeField]
			private float m_Multiplier = 1.0f;

			//[ShowIf("@ValueType == \"float\"")]
			//[BoxGroup("$Name")]
			[SerializeField]
			private UnityEventFloat m_FloatDirector = new UnityEventFloat();

			//[ShowIf("@ValueType == \"Vector2\"")]
			//[BoxGroup("$Name")]
			[SerializeField]
			private UnityEventVector2 m_Vector2Director = new UnityEventVector2();

			//[ShowIf("@ValueType == \"Button\"")]
			//[BoxGroup("$Name")]
			//[FoldoutGroup("$Name/Button Down",Expanded = false)]
			[SerializeField]
			private UnityEvent m_ButtonDownDirector = new UnityEvent();

			//[ShowIf("@ValueType == \"Button\"")]
			//[BoxGroup("$Name")]
			//[FoldoutGroup("$Name/Button Up",Expanded = false)]
			[SerializeField]
			private UnityEvent m_ButtonUpDirector = new UnityEvent();

			//[ShowIf("@ValueType == \"Button\"")]
			//[BoxGroup("$Name")]
			//[FoldoutGroup("$Name/Button Pressed",Expanded = false)]
			[SerializeField]
			private UnityEventBool m_ButtonPressedDirector = new UnityEventBool();

			/// <summary>
			/// Initializes a new instance of the <see cref="Director"/> class.
			/// </summary>
			public Director(bool pressed,InputActionReference action,CursorLockStateCheckType cursorLockStateCheckType,float multiplier,UnityEventFloat floatDirector,UnityEventVector2 vector2Director,UnityEvent buttonDownDirector,UnityEvent buttonUpDirector,UnityEventBool buttonPressedDirector)
			{
				Pressed = pressed;
				this.m_Action = action;
				this.m_CursorLockStateCheckType = cursorLockStateCheckType;
				this.m_Multiplier = multiplier;
				this.m_FloatDirector = floatDirector;
				this.m_Vector2Director = vector2Director;
				this.m_ButtonDownDirector = buttonDownDirector;
				this.m_ButtonUpDirector = buttonUpDirector;
				this.m_ButtonPressedDirector = buttonPressedDirector;
			}

			#endregion

			#region Public Methods

			/// <summary>
			/// </summary>
			public void OnEnable()
			{
				if (ValueType == "Button")
				{
					Action.performed += Action_performed;
					Action.canceled += Action_canceled;
				}
			}

			/// <summary>
			/// </summary>
			public void OnDisable()
			{
				if (ValueType == "Button")
				{
					Action.performed -= Action_performed;
					Action.canceled -= Action_canceled;
					Pressed = false;
				}
			}

			/// <summary>
			/// </summary>
			public void Update()
			{
				if (!CheckCursor())
					return;

				switch (ValueType)
				{
					case "float":
						this.m_FloatDirector.Invoke(Action.ReadValue<float>() * this.m_Multiplier);
						break;

					case "Vector2":
						this.m_Vector2Director.Invoke(Action.ReadValue<Vector2>() * this.m_Multiplier);
						break;

					case "Button":
						this.m_ButtonPressedDirector.Invoke(Pressed);
						break;
				}
			}

			#endregion

			#region Private Methods

			/// <summary>
			/// </summary>
			private void Action_performed(InputAction.CallbackContext obj)
			{
				if (!CheckCursor())
					return;

				this.m_ButtonDownDirector.Invoke();
				Pressed = true;
			}

			/// <summary>
			/// </summary>
			private void Action_canceled(InputAction.CallbackContext obj)
			{
				if (!CheckCursor())
					return;

				this.m_ButtonUpDirector.Invoke();
				Pressed = false;
			}

			/// <summary>
			/// Checks the cursor lock state.
			/// </summary>
			/// <returns>A bool.</returns>
			private bool CheckCursor()
			{
				return CursorLockStateCheckType == CursorLockStateCheckType.Any
					|| (CursorLockStateCheckType == CursorLockStateCheckType.MustBeLocked
					&& Cursor.lockState == CursorLockMode.Locked)
					|| (CursorLockStateCheckType == CursorLockStateCheckType.MustBeUnlocked
					&& Cursor.lockState == CursorLockMode.None);
			}

			#endregion

		}

		#endregion

		#region Public Indexers + Properties

		/// <summary>
		/// Gets or sets a value indicating whether to accept player input.
		/// </summary>
		public bool InputEnabled
		{
			get => Enabled;

			set
			{
				EnabledLate = value;
				EnabledFrames = 0;

				if (!value)
				{
					Enabled = value;
				}
			}
		}

		#endregion

		#region Private Indexers + Properties

		private bool EnabledLate { get; set; } = false;

		private int EnabledFrames { get; set; } = 0;

		//[ShowInInspector]
		//[DisableIf("@true")]
		private bool Enabled { get; set; } = false;

		#endregion

		#region Private Fields

		[SerializeField]
		private bool m_LockCursorOnEnable = false;

		//[ListDrawerSettings(AddCopiesLastElement = false,NumberOfItemsPerPage = 5,
		//ShowItemCount = true,ShowPaging = true)]
		[SerializeField]
		private List<Director> m_Directors = new List<Director>();

		#endregion

		#region Private Methods

		private void Awake()
		{
			InputEnabled = enabled;
			enabled = true;
		}

		/// <summary>
		/// </summary>
		private void OnInputEnable()
		{
			this.m_Directors.ForEach((d) =>
			{
				if (!d.Action.actionMap.asset.enabled)
				{
					d.Action.actionMap.asset.Enable();
				}
				d.OnEnable();
			});

			if (this.m_LockCursorOnEnable)
			{
				Cursor.lockState = CursorLockMode.Locked;
			}
		}

		/// <summary>
		/// </summary>
		private void OnInputDisable()
		{
			this.m_Directors.ForEach((d) =>
			{
				d.OnDisable();
			});
		}

		/// <summary>
		/// </summary>
		private void Update()
		{
			if (Enabled)
			{
				this.m_Directors.ForEach((d) =>
				{
					d.Update();
				});
			}
		}

		private void LateUpdate()
		{
			if (EnabledFrames > 1 && EnabledLate != Enabled)
			{
				Enabled = EnabledLate;
				if (Enabled)
				{
					OnInputEnable();
				}
				else
				{
					OnInputDisable();
				}
			}
			else if (EnabledFrames <= 1)
			{
				EnabledFrames++;
			}
		}

		#endregion

	}
}
