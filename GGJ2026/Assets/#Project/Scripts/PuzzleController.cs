using System;
using UnityEngine;


public class PuzzleController : MonoBehaviour
{
	#region Editor Fields
	[SerializeField] private int _levelIndex;
	[SerializeField] private Collider _puzzleCompleteTrigger;
	[SerializeField] private Grabbable[] _grabbableObjects;
	private Pose[] _startPoses;
	#endregion

	#region Fields
	#endregion

	#region Properties
	public static event EventHandler<int> LevelCompleted;
	public int LevelIndex => _levelIndex;
	#endregion

	#region Mono

	private void Start() {
		_startPoses = new Pose[_grabbableObjects.Length];
		for (int i = 0; i < _grabbableObjects.Length; i++) {
			var grabbableObject = _grabbableObjects[i];
			_startPoses[i] = new Pose(grabbableObject.transform.position, grabbableObject.transform.rotation);
		}
	}

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
	public void SetupLevel()
	{
		foreach (var grabbableObject in _grabbableObjects) {
			grabbableObject.gameObject.SetActive(true);
		}
	}
	
	public void ResetLevel()
	{
		for (int i = 0; i < _grabbableObjects.Length; i++) {
			var grabbableObject = _grabbableObjects[i];
			var origPose = _startPoses[i];
			//TODO: lerp this because its cool
			grabbableObject.transform.position = origPose.position;
			grabbableObject.transform.rotation = origPose.rotation;
		}
	}
	
	public void FreezeLevel()
	{
		for (int i = 0; i < _grabbableObjects.Length; i++) {
			var grabbableObject = _grabbableObjects[i];
			Destroy(grabbableObject);
		}
	}
	
	#endregion
}

