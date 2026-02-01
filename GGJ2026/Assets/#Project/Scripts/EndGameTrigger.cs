using UnityEngine;


public class EndGameTrigger : MonoBehaviour
{
	#region Editor Fields
	[SerializeField] private Vector3 _targetPositionOffset;
	[SerializeField] private Animator _animator;
	#endregion

	#region Fields
	#endregion

	#region Properties
	#endregion

	#region Mono
	private void OnTriggerEnter(Collider other)
	{
		if (other.isTrigger) return;
		if (other.gameObject.layer != Layers.Player) return;

		// get the game manager
		var gameController = FindFirstObjectByType<GameController>();
		gameController.SetGameState(GameController.GameState.EndGame);

		_animator.enabled = true;

		if (other.TryGetComponent<PlayerMovement3D>(out var playerMovement))
		{
			playerMovement.MoveTo(transform.position + _targetPositionOffset);
		}
	}
	#endregion

	#region Methods
	#endregion
}

