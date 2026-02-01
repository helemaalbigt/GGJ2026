using System;
using UnityEngine;


public class PuzzleController : MonoBehaviour
{
	#region Editor Fields
	[SerializeField] private bool _skipBuildFase;
	[SerializeField] private int _levelIndex;
	[SerializeField] private Collider _puzzleCompleteTrigger;
	[SerializeField] private Vector3 _targetPositionOffset = new Vector3(-0.0086f, 0, 0);
	[SerializeField] private Grabbable[] _grabbableObjects;
	[SerializeField] private GameObject[] _enableWhenActive;
	[SerializeField] private Animator _animator;

	private Pose[] _startPoses;
	#endregion

	#region Fields
	#endregion

	#region Properties
	public static event EventHandler<LevelCompleteEventArgs> LevelCompleted;
	public int LevelIndex => _levelIndex;
	#endregion

	#region Mono

	private void Start()
	{
		_startPoses = new Pose[_grabbableObjects.Length];
		for (int i = 0; i < _grabbableObjects.Length; i++)
		{
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

			if (other.TryGetComponent<PlayerMovement3D>(out var playerMovement))
			{
				playerMovement.MoveTo(transform.position + _targetPositionOffset);
			}
			_animator.enabled = true;

			// destroy the trigger to prevent multiple triggers
			Destroy(_puzzleCompleteTrigger, Time.fixedDeltaTime);
		}
	}
	#endregion

	#region Methods
	private void OnPlayerEnteredTrigger()
	{
		LevelCompleted?.Invoke(this, new LevelCompleteEventArgs(_levelIndex, _skipBuildFase));
	}
	public void SetupLevel()
	{
		foreach (var grabbableObject in _grabbableObjects)
		{
			grabbableObject.gameObject.SetActive(true);
		}

		SetAllChildBlocksGravity(false);

		foreach (var enableWhenActive in _enableWhenActive)
		{
			enableWhenActive.SetActive(true);
		}
	}
	public void SetAllChildBlocksGravity(bool gravityOn)
	{
		foreach (var grabbable in _grabbableObjects)
		{
			grabbable.rigidBody.useGravity = gravityOn;
			grabbable.rigidBody.linearDamping = gravityOn ? 1f : 10f;
			grabbable.rigidBody.angularDamping = gravityOn ? 0.1f : 10f;
			if (gravityOn)
			{
				grabbable.rigidBody.constraints = RigidbodyConstraints.None;
			}
			else
			{
				grabbable.rigidBody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
			}
		}
	}

	public void ResetLevel(bool resetObjects)
	{
		LevelCompleted?.Invoke(this, new LevelCompleteEventArgs(_levelIndex, _skipBuildFase));  // reset camera

		if (_startPoses == null || _startPoses.Length != _grabbableObjects.Length)
		{
			Debug.LogError("Start poses not yet initialized.");
			return;
		}

		if (resetObjects)
		{

			for (int i = 0; i < _grabbableObjects.Length; i++)
			{
				var grabbableObject = _grabbableObjects[i];
				var origPose = _startPoses[i];
				//TODO: lerp this because its cool
				grabbableObject.rigidBody.Move(origPose.position, origPose.rotation);
			}
		}

		SetAllChildBlocksGravity(false);
	}

	public void FreezeLevel()
	{
		for (int i = 0; i < _grabbableObjects.Length; i++)
		{
			var grabbableObject = _grabbableObjects[i];
			Destroy(grabbableObject);
		}
		SetAllChildBlocksGravity(false);

		foreach (var enableWhenActive in _enableWhenActive)
		{
			enableWhenActive.SetActive(false);
		}
	}

	internal void StartLevel()
	{
		SetAllChildBlocksGravity(true);

	}

	#endregion
}

public class LevelCompleteEventArgs : EventArgs
{
	public int LevelIndex { get; private set; }
	public bool SkipBuildMode { get; private set; }

	public LevelCompleteEventArgs(int levelIndex, bool skipBuildMode)
	{
		LevelIndex = levelIndex;
		SkipBuildMode = skipBuildMode;
	}
}
