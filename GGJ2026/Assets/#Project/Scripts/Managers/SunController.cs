using UnityEngine;

public class SunController : MonoBehaviour
{
	[SerializeField] private Light sun;
	[SerializeField] private Transform startPosition;
	[SerializeField] private Transform playPosition;
    [SerializeField] private Transform endPosition;
    [SerializeField] private Color playColor;
	[SerializeField] private Color endColor;
	[SerializeField] private Gradient gradient;
    [SerializeField] private float speed = .2f;
    [SerializeField] private float playIntensity = .5f;


    private Light sunLight;
	private float t;

	private bool _doGameIntro = false;
    private bool _doGameOutro = false;

    private void OnEnable()
	{
		GameController.GameStateChanged += StartIntro;
    }

	private void OnDisable()
	{
		GameController.GameStateChanged -= StartIntro;
	}

	void Start()
	{
		sunLight = sun.GetComponent<Light>();
		sun.transform.position = startPosition.position;
		sun.transform.rotation = startPosition.rotation;

		sunLight.color = gradient.Evaluate(t);
		sunLight.intensity = Mathf.Lerp(1f, 0.5f, t);
	}

	private void Update()
	{
		if (_doGameIntro) UpdateIntro();
        if (_doGameOutro) UpdateOutro();
	}

    private void UpdateIntro() 	{

        t += Time.deltaTime * speed;
        sunLight.color = gradient.Evaluate(t);
        sun.transform.position = Vector3.Lerp(startPosition.position, playPosition.position, t);
        sun.transform.rotation = Quaternion.Lerp(startPosition.rotation, playPosition.rotation, t);
        sunLight.intensity = Mathf.Lerp(1f, playIntensity, t);
    }

    private void UpdateOutro()
    {
        t += Time.deltaTime * speed;
        sunLight.color = Color.Lerp(endColor, playColor, t);
        sun.transform.position = Vector3.Lerp(playPosition.position, endPosition.position, t);
        sun.transform.rotation = Quaternion.Lerp(playPosition.rotation, endPosition.rotation, t);
        sunLight.intensity = Mathf.Lerp(playIntensity, .1f, t);
    }

    private void StartIntro(object sender, GameController.GameState e)
	{
		if (e == GameController.GameState.StartGame)
		{
			_doGameIntro = true;
            _doGameOutro = false;
        }
    }

	public void StartOutro()
	{
        t = 0;
		_doGameOutro = true;
		_doGameIntro = false;
    }

}
