using UnityEngine;


public class PlayerBurnParticle : MonoBehaviour
{
	#region Editor Fields
	[SerializeField] private ParticleSystem _particle;
	#endregion

	#region Fields
	#endregion

	#region Properties
	#endregion

	#region Mono
	private void Awake()
	{
		_particle.Stop();
	}
	private void OnEnable()
	{
		GameController.PlayerStateChanged += StopStartBurning;
	}
	private void OnDisable()
	{
		GameController.PlayerStateChanged -= StopStartBurning;
	}

	private void StopStartBurning(object sender, GameController.PlayerState e)
	{
		if (e == GameController.PlayerState.Burning)
		{
			_particle.Play();
		}
		else
		{
			_particle.Stop();
		}
	}
	#endregion

	#region Methods
	#endregion
}

