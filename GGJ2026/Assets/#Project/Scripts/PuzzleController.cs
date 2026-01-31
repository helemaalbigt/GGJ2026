using System;
using UnityEngine;


public class PuzzleController : MonoBehaviour
{
	#region Editor Fields
	[SerializeField] private int _levelIndex;
	#endregion

	#region Fields
	#endregion

	#region Properties
	public static event EventHandler<int> LevelCompleted;
	#endregion

	#region Mono
	private void OnTriggerEnter(Collider other)
	{
		if (other.isTrigger) return;
		if (other.gameObject.layer == Layers.Player)
		{
			// player entered the trigger
			OnPlayerEnteredTrigger();

			// destroy this object to prevent multiple triggers
			Destroy(gameObject, Time.fixedDeltaTime);
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

