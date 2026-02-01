using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class GameController : MonoBehaviour
{
	#region enum
	public enum GameState
	{
		Movement,
		Building,
		GameOver,
		StartGame,
		EndGame,
		WaitForIntro,
		Intro,
	}
	public enum PlayerState
	{
		Burning,
		Safe,
		Dead,
	}
	#endregion

	#region EditorFields
	[SerializeField] private bool DEBUG_DONT_SWITCH_STATE = false;
	[SerializeField] private GameState _startingState = GameState.StartGame;
	[SerializeField] private int _startingLevel = 0;

	[Header("PlayerHealth")]
	[SerializeField] private GroundColorPicker _shadowCheck;
	[SerializeField] private float _maxBurnTime;
	[SerializeField] private float _burnRate;
	[SerializeField] private float _healRate;

	[Header("Reset Level")]
	[SerializeField] private float _respawnTime;

	[Header("Events")]
	[SerializeField] private UnityEvent EnteredMovementState;
	[SerializeField] private UnityEvent EnteredBuildingState;
	[SerializeField] private UnityEvent EnteredGameOverState;
	[SerializeField] private UnityEvent EnteredStartGameState;
	[SerializeField] private UnityEvent EnteredEndGameState;
	[SerializeField] private UnityEvent EnteredIntroState;

	[Header("SFX")]
	[SerializeField] private AudioSource _imBurningSfx;
	[SerializeField] private AudioSource _iDiedSfx;
	#endregion

	#region Fields
	private GameState _currentState;
	private PlayerState _playerState;
	private int _lastCompletedLevelIndex = -1;
	private List<PuzzleController> _levels = new();

	private float _previousBurnTime;
	private float _burnTime;
	private float _respawnTimer;

	public PuzzleController CurrentLevel
	{
		get
		{
			var clampedCurrentLevelIndex = Mathf.Clamp(_lastCompletedLevelIndex, 0, _levels.Count);
			return _levels.Where((puzzle) => puzzle.LevelIndex == clampedCurrentLevelIndex).FirstOrDefault();
		}
	}
	#endregion

	#region Event
	public static event EventHandler<PlayerState> PlayerStateChanged;
	public static event EventHandler<GameState> GameStateChanged;
	// send a value from 0 to 1 of the burntime/maxburntime
	// 0 is no burn at all
	// 1 is fully burned up
	public static event EventHandler<float> BurnTimeUpdated;
	#endregion

	#region Mono
	private void Awake()
	{
		_lastCompletedLevelIndex = _startingLevel - 1;
		SetGameState(_startingState);
		_levels = FindObjectsByType<PuzzleController>(FindObjectsSortMode.None).ToList();
		if (_lastCompletedLevelIndex >= 0)
		{
			// warp to selectec starting level
			RestartLevel();
		}
	}
	private void OnEnable()
	{
		PuzzleController.LevelCompleted += OnPuzzleCompleted;

		CustomInputManager.SubscribeToPerformed(CustomInputManager.Player.BeginLevel, BeginLevel);
		CustomInputManager.SubscribeToPerformed(CustomInputManager.Player.ResetLevel, ResetLevel);

		CustomInputManager.SubscribeToPerformed(CustomInputManager.Player.StartGame, StartGame);
	}

	private void OnDisable()
	{
		PuzzleController.LevelCompleted -= OnPuzzleCompleted;

		CustomInputManager.UnsubscribeFromPerformed(CustomInputManager.Player.BeginLevel, BeginLevel);
		CustomInputManager.UnsubscribeFromPerformed(CustomInputManager.Player.ResetLevel, ResetLevel);

		CustomInputManager.UnsubscribeFromPerformed(CustomInputManager.Player.StartGame, StartGame);
	}
	private void Update()
	{
		if (_currentState == GameState.EndGame)
		{
			// set the player state
			SetPlayerState(PlayerState.Safe);
			return;
		}

		if (_playerState != PlayerState.Dead)
		{
			SetPlayerState(_shadowCheck.IsInShadow() ? PlayerState.Safe : PlayerState.Burning);

			switch (_playerState)
			{
				case PlayerState.Burning:
					_burnTime += _burnRate * Time.deltaTime;

					if (_burnTime > _maxBurnTime)
					{
						PlayerDeath();
					}
					else
					{
						// only play the burning sfx if you're not dead
						if (!_imBurningSfx.isPlaying)
							_imBurningSfx.Play();
					}

					break;
				case PlayerState.Safe:
					if (_burnTime > 0)
						_burnTime -= _healRate * Time.deltaTime;
					break;
			}

			_burnTime = Mathf.Clamp(_burnTime, 0, _maxBurnTime);

			if (_previousBurnTime != _burnTime)
			{
				BurnTimeUpdated?.Invoke(this, _burnTime / _maxBurnTime);
			}
			_previousBurnTime = _burnTime;
		}

		// player is dead
		else
		{
			_respawnTimer += Time.deltaTime;

			if (_respawnTimer > _respawnTime)
			{
				RestartLevel();
			}
		}
	}

	#endregion

	#region Methods
	private void StartGame(InputAction.CallbackContext context)
	{
		if (_currentState == GameState.WaitForIntro)
		{
			SetGameState(GameState.StartGame);
		}
	}
	private void SetPlayerState(PlayerState playerState)
	{
		// prevent duplicate
		if (_playerState == playerState) return;

		_playerState = playerState;

		PlayerStateChanged?.Invoke(this, playerState);
	}

	public void RestartLevel()
	{
		var clampedCurrentLevelIndex = Mathf.Clamp(_lastCompletedLevelIndex, 0, _levels.Count);
		var currentLevel = _levels.Where((puzzle) => puzzle.LevelIndex == clampedCurrentLevelIndex).FirstOrDefault();
		// reset the level
		currentLevel.ResetLevel();

		// set the player position to the level position again
		_shadowCheck.transform.position = currentLevel.transform.position;

		// reset the respawn timer and the burn time
		_respawnTimer = 0;
		_burnTime = 0;

		// set the playerState to safe
		SetPlayerState(PlayerState.Safe);
    }

	private void ResetLevel(InputAction.CallbackContext context)
	{
		RestartLevel();
    }

	private void BeginLevel(InputAction.CallbackContext context)
	{
		//only continue if we're in building state
		if (_currentState != GameState.Building) return;

		SetGameState(GameState.Movement);

		var clampedCurrentLevelIndex = Mathf.Clamp(_lastCompletedLevelIndex, 0, _levels.Count);
		var currentLevel = _levels.Where((puzzle) => puzzle.LevelIndex == clampedCurrentLevelIndex).FirstOrDefault();
		currentLevel.StartLevel();
	}

	private void OnPuzzleCompleted(object sender, LevelCompleteEventArgs e)
	{
		// failsafe
		if (sender is not PuzzleController) return;

		// freeze the current level 
		var clampedCurrentLevelIndex = Mathf.Clamp(e.LevelIndex - 1, 0, _levels.Count);
		var currentLevel = _levels.Where((puzzle) => puzzle.LevelIndex == clampedCurrentLevelIndex).FirstOrDefault();
		currentLevel.FreezeLevel();

		// set the current level index 
		_lastCompletedLevelIndex = e.LevelIndex;

		var clampedNextLevelIndex = Mathf.Clamp(e.LevelIndex, 0, _levels.Count);
		var nextLevel = _levels.Where((puzzle) => puzzle.LevelIndex == clampedNextLevelIndex).FirstOrDefault();
		nextLevel.SetupLevel();

		if (DEBUG_DONT_SWITCH_STATE) return;
		// update the game state to the building state
		SetGameState(e.SkipBuildMode ? GameState.Movement : GameState.Building);
	}


	public void SetGameState(GameState state)
	{
		_currentState = state;

		switch (_currentState)
		{
			case GameState.Movement:
				EnteredMovementState?.Invoke();
				break;
			case GameState.Building:
				EnteredBuildingState?.Invoke();
				break;
			case GameState.GameOver:
				EnteredGameOverState?.Invoke();
				break;
			case GameState.StartGame:
				EnteredStartGameState?.Invoke();
				break;
			case GameState.EndGame:
				EnteredEndGameState?.Invoke();
				break;
			case GameState.Intro:
				EnteredIntroState?.Invoke();
				break;
		}

		GameStateChanged?.Invoke(this, state);
	}

	private void PlayerDeath()
	{
		_imBurningSfx.Stop();
		_iDiedSfx.Play();
		SetPlayerState(PlayerState.Dead);
		// enable the gameOver state
		SetGameState(GameState.GameOver);
	}

	[ContextMenu("Toggle GameState")]
	private void ToggleGameState()
	{
		SetGameState(_currentState == GameState.Movement ? GameState.Building : GameState.Movement);
	}
	#endregion
}

public static class Layers
{
	public static int Default = 0;
	public static int Player = 6;
	public static int Ground = 7;
}