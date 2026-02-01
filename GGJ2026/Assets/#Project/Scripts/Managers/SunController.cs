using System.Collections;
using System.Linq;
using UnityEngine;

public class SunController : MonoBehaviour
{

    [SerializeField] private Light sun;
    [SerializeField] private Transform startPosition;
    [SerializeField] private Transform endPosition;
    [SerializeField] private Color startColor;
    [SerializeField] private Color endColor;
    [SerializeField] private Gradient gradient;
    [SerializeField] private float speed = .2f;

    private Light sunLight;
    private float t;

    void Start()
    {
        sunLight = sun.GetComponent<Light>();
        sun.transform.position = startPosition.position;
        sun.transform.rotation = startPosition.rotation;
    }

    private void Update()
    {
        t += Time.deltaTime * speed;

        sunLight.color = gradient.Evaluate(t);
        sun.transform.position = Vector3.Lerp(startPosition.position, endPosition.position, t);
        sun.transform.rotation = Quaternion.Lerp(startPosition.rotation, endPosition.rotation, t);
        sunLight.intensity = Mathf.Lerp(1f, 0.5f, t);
    }
}
