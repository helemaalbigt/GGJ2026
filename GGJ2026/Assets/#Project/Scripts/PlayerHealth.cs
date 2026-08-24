using UnityEngine;
using UnityEngine.UI;


public class PlayerHealth : MonoBehaviour
{
	#region Editor Fields
	[SerializeField] private Image _image;
	#endregion

	#region Fields
	#endregion

	#region Properties
	#endregion

	#region Mono
	private void Awake()
	{
		_image.fillAmount = 1;
	}
	private void OnEnable()
	{
		GameController.BurnTimeUpdated += UpdateHealthBar;
		GameController.GameStateChanged += HandleEndState;
	}

	private void OnDisable()
	{
		GameController.BurnTimeUpdated -= UpdateHealthBar;
		GameController.GameStateChanged -= HandleEndState;
	}

	#endregion

	#region Methods
	private void HandleEndState(object sender, GameController.GameState e)
	{
		if (e != GameController.GameState.EndGame) return;

		gameObject.SetActive(false);
	}

	private void UpdateHealthBar(object sender, float e)
	{
		_image.fillAmount = 1 - e;

		var active = 1 - _image.fillAmount > float.Epsilon;
		_image.transform.parent.gameObject.SetActive(active);
	}

	#endregion
}

