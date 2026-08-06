using UnityEngine;


public class DisableBeforeEnding : MonoBehaviour
{
	#region Editor Fields
	[SerializeField] private GameObject[] _disableObjects;
	#endregion

	#region Fields
	#endregion

	#region Properties
	#endregion

	#region Mono
	private void OnEnable()
	{
		GameController.GameStateChanged += OnStateChanged;
	}
	private void OnDisable()
	{
		GameController.GameStateChanged += OnStateChanged;
	}

	#endregion

	#region Methods
	private void OnStateChanged(object sender, GameController.GameState e)
	{
		if (e != GameController.GameState.EndGame) return;

		if (_disableObjects == null || _disableObjects.Length == 0) return;

		foreach (GameObject go in _disableObjects)
		{
			if (go == null) continue;
			go.SetActive(false);
		}
	}
	#endregion
}

