using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [SerializeField] private CameraManager cameraManager;
    [SerializeField] private GameObject blockPrefab;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cameraManager.ShowOverheadView();
    }

    // Update is called once per frame
    void Update()
    {

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            cameraManager.ShowFirstPersonView();
        }

        if(Keyboard.current.sKey.wasPressedThisFrame)
        {
            Instantiate(blockPrefab, new Vector3(2, 0, -10), Quaternion.identity);
        }
    }
}
