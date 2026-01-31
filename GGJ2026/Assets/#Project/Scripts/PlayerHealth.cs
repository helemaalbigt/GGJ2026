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
	}
	private void OnDisable()
	{
		GameController.BurnTimeUpdated -= UpdateHealthBar;
	}

	private void UpdateHealthBar(object sender, float e)
	{
		_image.fillAmount = 1 - e;
	}
	#endregion

	#region Methods
	#endregion
}

