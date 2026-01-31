using UnityEngine;
using UnityEngine.Events;

public class GameController : MonoBehaviour
{
	#region enum
	public enum GameState
	{
		Movement,
		Building
	}
	#endregion

	#region EditorFields
	[SerializeField] private bool DEBUG_DONT_SWITCH_STATE = false;
	[SerializeField] private GameState _startingState;

	[Header("Events")]
	[SerializeField] private UnityEvent EnteredMovementState;
	[SerializeField] private UnityEvent EnteredBuildingState;
	#endregion

	#region Field
	private GameState _currentState;
	private int _currentLevelIndex = -1;
	#endregion

	#region Mono
	private void Awake()
	{
		SetGameState(_startingState);
	}
	private void OnEnable()
	{
		PuzzleController.LevelCompleted += OnPuzzleCompleted;
	}
	private void OnDisable()
	{
		PuzzleController.LevelCompleted -= OnPuzzleCompleted;
	}
	#endregion

	#region Methods
	private void OnPuzzleCompleted(object sender, int e)
	{
		// set the current level index 
		_currentLevelIndex = e;

		if (DEBUG_DONT_SWITCH_STATE) return;
		// update the game state to the building state
		SetGameState(GameState.Building);
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
		}
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