using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class CustomInputManager
{
	#region Enums
	public enum Player
	{
		Move,
		ShiftMovementPlane,
		RotateXYClockwise,
		RotateXYCounterClockwise,
		RotateZYClockwise,
		RotateZYCounterClockwise,
		ResetLevel,
		BeginLevel,
		StartGame,
	}
	public enum InputType
	{
		Keyboard,
		GamePad
	}
	#endregion

	#region Fields
	private List<InputAction> _playerInputActions = new List<InputAction>();
	private GameInput _inputSystem;

	private bool _lastInputWasController = false;
	private InputType _currentDeviceType;
	#endregion

	#region Properties
	public InputType CurrentDeviceType
	{
		get
		{
			return _currentDeviceType;
		}
		set
		{
			if (_currentDeviceType != value)
			{
				_currentDeviceType = value;
				OnDeviceSwitch(_currentDeviceType);
			}
		}
	}

	public event EventHandler<DeviceSwitchEventArgs> DeviceSwitch;

	private static CustomInputManager _instance;
	public static CustomInputManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new CustomInputManager();
			}
			return _instance;
		}
	}
	#endregion

	#region Constructor
	private CustomInputManager()
	{
		_inputSystem = new GameInput();

		AssignInputActions();

		EnableAll();
		SetupDeviceType();
	}
	#endregion

	#region Methods
	private void AssignInputActions()
	{
		// Player Map
		_playerInputActions.Add(_inputSystem.Player.Move);
		_playerInputActions.Add(_inputSystem.Player.ShiftMovementPlane);
		_playerInputActions.Add(_inputSystem.Player.RotateXYClockwise);
		_playerInputActions.Add(_inputSystem.Player.RotateXYCounterClockwise);
		_playerInputActions.Add(_inputSystem.Player.RotateZYClockWise);
		_playerInputActions.Add(_inputSystem.Player.RotateZYCounterClockWise);
		_playerInputActions.Add(_inputSystem.Player.ResetLevel);
		_playerInputActions.Add(_inputSystem.Player.BeginLevel);
		_playerInputActions.Add(_inputSystem.Player.StartGame);
	}

	private InputAction GetAction(Player inputType)
	{
		return _playerInputActions[(int)inputType];
	}

	private void EnableAction(Player input)
	{
		_playerInputActions[(int)input].Enable();
	}
	private void DisableAction(Player input)
	{
		_playerInputActions[(int)input].Disable();
	}
	private void EnableAllPlayer()
	{
		foreach (var action in _playerInputActions)
		{
			action.Enable();
		}
	}
	private void DisableAllPlayer()
	{
		foreach (var action in _playerInputActions)
		{
			action.Disable();
		}
	}

	private void EnableAll()
	{
		EnableAllPlayer();
	}
	private void DisableAll()
	{
		DisableAllPlayer();
	}

	#region Events

	private void SubscribeOnPerform(Player input, Action<InputAction.CallbackContext> function)
	{
		_playerInputActions[(int)input].performed += function;
	}
	private void UnsubscribeOnPerform(Player input, Action<InputAction.CallbackContext> function)
	{
		_playerInputActions[(int)input].performed -= function;
	}
	private void SubscribeOnCancel(Player input, Action<InputAction.CallbackContext> function)
	{
		_playerInputActions[(int)input].canceled += function;
	}
	private void UnsubscribeOnCancel(Player input, Action<InputAction.CallbackContext> function)
	{
		_playerInputActions[(int)input].canceled -= function;
	}
	#endregion

	#region Static Methods
	public static InputAction GetInputAction(Player inputType)
	{
		return Instance.GetAction(inputType);
	}

	public static void EnableInputAction(Player input)
	{
		Instance.EnableAction(input);
	}
	public static void DisableInputAction(Player input)
	{
		Instance.DisableAction(input);
	}
	public static void EnableAllPlayerActions()
	{
		Instance.EnableAllPlayer();
	}
	public static void DisableAllPlayerActions()
	{
		Instance.DisableAllPlayer();
	}
	public static void EnableAllActions()
	{
		Instance.EnableAll();
	}
	public static void DisableAllActions()
	{
		Instance.DisableAll();
	}

	public static void SubscribeToPerformed(Player input, Action<InputAction.CallbackContext> function)
	{
		Instance.SubscribeOnPerform(input, function);
	}
	public static void UnsubscribeFromPerformed(Player input, Action<InputAction.CallbackContext> function)
	{
		Instance.UnsubscribeOnPerform(input, function);
	}
	public static void SubscribeToCancelled(Player input, Action<InputAction.CallbackContext> function)
	{
		Instance.SubscribeOnCancel(input, function);
	}

	public static void UnsubscribeFromCancelled(Player input, Action<InputAction.CallbackContext> function)
	{
		Instance.UnsubscribeOnCancel(input, function);
	}
	#endregion
	#endregion

	#region Device Switch
	private void SetupDeviceType()
	{
		//Subscribe to all the inputaction.started to know what the last input was
		foreach (var action in _playerInputActions)
		{
			action.started += ListenToDeviceType;
		}
	}

	private void ListenToDeviceType(InputAction.CallbackContext context)
	{
		//Check the context to see if the input was from a controller or keyboard
		_lastInputWasController = context.control.device is Gamepad or Joystick;

		if (!_lastInputWasController)
		{
			CurrentDeviceType = InputType.Keyboard;
			return;
		}

		CurrentDeviceType = InputType.GamePad;
	}

	private void OnDeviceSwitch(InputType type)
	{
		var handler = DeviceSwitch;
		handler?.Invoke(this, new DeviceSwitchEventArgs(type));
	}
}
public class DeviceSwitchEventArgs : EventArgs
{
	public CustomInputManager.InputType Type { get; private set; }

	public DeviceSwitchEventArgs(CustomInputManager.InputType type)
	{
		Type = type;
	}
}
#endregion
