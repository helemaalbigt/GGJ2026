using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine;


public class CameraController : MonoBehaviour
{
	#region Editor Fields
	[SerializeField] private List<LevelCamera> _levelCameras;
	[SerializeField] private CinemachineBrain _cameraBrain;
	#endregion

	#region Fields
	private LevelCamera _currentCamera;
	#endregion

	#region Properties
	#endregion

	#region Mono
	private void Reset()
	{
		_cameraBrain = GetComponentInChildren<CinemachineBrain>();
	}
	private void Awake()
	{
		// disable all cameras
		// except for the one with level index 0
		foreach (var levelCamera in _levelCameras)
		{
			if (levelCamera.LevelIndex == -1)
			{
				levelCamera.Camera.gameObject.SetActive(true);
				_currentCamera = levelCamera;
				continue;
			}
			levelCamera.Camera.gameObject.SetActive(false);
		}
	}
	private void OnEnable()
	{
		PuzzleController.LevelCompleted += EnableCamera;
	}
	private void OnDisable()
	{
		PuzzleController.LevelCompleted -= EnableCamera;
	}
	#endregion


	#region Methods
	private void EnableCamera(object sender, LevelCompleteEventArgs e)
	{
		// first disable the current camera
		_currentCamera.Camera.gameObject.SetActive(false);

		// select the level camera
		_currentCamera = _levelCameras.Where((c) => c.LevelIndex == e.LevelIndex).FirstOrDefault();

		// set the easing attributes
		_cameraBrain.DefaultBlend = _currentCamera.Ease;

		// enable the new camera
		_currentCamera.Camera.gameObject.SetActive(true);
	}
	#endregion
}

[Serializable]
public class LevelCamera
{
	public CinemachineVirtualCameraBase Camera;
	public int LevelIndex;
	public CinemachineBlendDefinition Ease;
}