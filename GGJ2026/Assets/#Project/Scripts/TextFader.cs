using System.Collections;
using TMPro;
using UnityEngine;


public class TextFader : MonoBehaviour
{
	#region Editor Fields
	[SerializeField] private int _levelIndex;
	[SerializeField] private TextMeshPro _text;
	[SerializeField] private float _fadeTime;
	#endregion

	#region Fields
	#endregion

	#region Properties
	#endregion

	#region Mono
	private void Reset()
	{
		_text = GetComponent<TextMeshPro>();
	}
	private void Awake()
	{
		// set alpha to 0
		var color = _text.color;
		color.a = 0;
		_text.color = color;
	}
	private void OnEnable()
	{
		PuzzleController.LevelCompleted += StartFadeText;
	}

	private void OnDisable()
	{
		PuzzleController.LevelCompleted -= StartFadeText;
	}
	#endregion

	#region Methods
	private void StartFadeText(object sender, int e)
	{
		if (_levelIndex != e) return;
		StartCoroutine(FadeText());
	}

	private IEnumerator FadeText()
	{
		var value = 0f;

		Color color = _text.color;

		// target alpha is 1, should start on 0
		while (value < _fadeTime)
		{
			value += Time.deltaTime;

			color.a = Mathf.Lerp(0, 1, value / _fadeTime);
			_text.color = color;
			yield return null;
		}
		color.a = 1;
		_text.color = color;
	}
	#endregion
}

