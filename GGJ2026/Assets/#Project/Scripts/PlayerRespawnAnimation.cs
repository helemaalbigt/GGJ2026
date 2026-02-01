using System.Collections;
using UnityEngine;


public class PlayerRespawnAnimation : MonoBehaviour
{
	#region Editor Fields
	[SerializeField] private GameObject _vampire;
	[SerializeField] private GameObject _bat;
	[SerializeField] private Vector3 _targetUpPosition = new Vector3(0, 0.001f, 0);
	[SerializeField] private AnimationCurve _animCurve;
	[SerializeField] private float _respawnTime;
	#endregion

	#region Fields
	private Vector3[] _curveControlPoints = new Vector3[3];

	private GameController _gameController;

	private bool _isMoving;
	#endregion

	#region Properties
	#endregion

	#region Mono
	private void Awake()
	{
		_gameController = FindAnyObjectByType<GameController>();
	}
	private void OnEnable()
	{
		GameController.PlayerStateChanged += OnPlayerStateChanged;

	}
	private void OnDisable()
	{
		GameController.PlayerStateChanged -= OnPlayerStateChanged;
	}

	private void OnPlayerStateChanged(object sender, GameController.PlayerState e)
	{
		if (e == GameController.PlayerState.Safe || e == GameController.PlayerState.Burning)
		{
			// enable the vampire
			_bat.SetActive(false);
			_vampire.SetActive(true);
		}
		else
		{
			if (_isMoving) return;
			// enable the bat
			_bat.SetActive(true);
			_vampire.SetActive(false);

			// set bat position also as the first control point
			_curveControlPoints[0] = _bat.transform.position = _vampire.transform.position;

			// set the other control points
			_curveControlPoints[1] = _bat.transform.position + _targetUpPosition;
			_curveControlPoints[2] = _gameController.CurrentLevel.transform.position;

			_isMoving = true;

			// start moving bat 
			StartCoroutine(MoveBat());
		}
	}

	private IEnumerator MoveBat()
	{
		float t = 0;

		while (t < _respawnTime)
		{
			t += Time.deltaTime;
			_bat.transform.position = BezierCurve.DeCasteljau(_curveControlPoints, t / _respawnTime);
			var nextPoint = BezierCurve.DeCasteljau(_curveControlPoints, (t / _respawnTime) + 0.05f);
			_bat.transform.forward = (nextPoint - _bat.transform.position).normalized;
			yield return null;
		}

		_isMoving = false;
	}
	#endregion

	#region Methods
	#endregion
}

