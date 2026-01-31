using System;
using UnityEngine;


public class PuzzleController : MonoBehaviour
{
	#region Editor Fields
	[SerializeField] private int _levelIndex;
	[SerializeField] private Collider _puzzleCompleteTrigger;
	#endregion

	#region Fields
	#endregion

	#region Properties
	public static event EventHandler<int> LevelCompleted;
	public int LevelIndex => _levelIndex;
	#endregion

	#region Mono
	private void OnTriggerEnter(Collider other)
	{
		if (other.isTrigger) return;
		if (other.gameObject.layer == Layers.Player)
		{
			// player entered the trigger
			OnPlayerEnteredTrigger();

			// destroy the trigger to prevent multiple triggers
			Destroy(_puzzleCompleteTrigger, Time.fixedDeltaTime);
		}
	}
	#endregion

	#region Methods
	private void OnPlayerEnteredTrigger()
	{
		LevelCompleted?.Invoke(this, _levelIndex);
	}
	#endregion
}

