using UnityEngine;


public class SetPlayerSeeThroughEffect : MonoBehaviour
{
	#region Editor Fields
	[SerializeField] private GameObject _playerModel;
	// would be intro, game start and game ended
	[SerializeField] private GameController.GameState[] _disableEffectStates;

	#endregion

	#region Fields
	#endregion

	#region Properties
	#endregion

	#region Mono
	private void OnEnable()
	{
		GameController.GameStateChanged += ListenToGameState;
	}
	private void OnDisable()
	{
		GameController.GameStateChanged -= ListenToGameState;
	}

	private void ListenToGameState(object sender, GameController.GameState e)
	{
		bool oneOfDisabledStates = false;
		foreach (var state in _disableEffectStates)
		{
			if (state == e)
			{
				oneOfDisabledStates = true;
				break;
			}
		}

		if (oneOfDisabledStates)
		{
			_playerModel.layer = Layers.Default;
		}
		else
		{
			_playerModel.layer = Layers.Player;
		}
	}
	#endregion

	#region Methods
	#endregion
}

