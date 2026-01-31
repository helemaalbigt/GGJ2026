using UnityEngine;

public class ShadowCameraController : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Vector3 cameraOffset;

    // Update is called once per frame
    void Update()
    {
        transform.position = player.transform.position + cameraOffset;
    }
}
