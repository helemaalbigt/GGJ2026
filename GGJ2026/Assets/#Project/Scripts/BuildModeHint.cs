using System.Collections;
using UnityEngine;

public class BuildModeHint : MonoBehaviour
{
    public GameObject hintGameObject;
    public GameController.GameState requiredGameState;

    private void OnEnable()
    {
        GameController.GameStateChanged += ToggleVisibility;
    }

    private void OnDisable()
    {
        GameController.GameStateChanged -= ToggleVisibility;
    }

    private void ToggleVisibility(object sender, GameController.GameState gameState)
    {
        if (gameState == requiredGameState)
        {
            StartCoroutine(WaitAndSetVisibility());
        }
        else
        {
            StopAllCoroutines();
            hintGameObject.SetActive(false);
        }
    }

    private IEnumerator WaitAndSetVisibility()
    {
        yield return new WaitForSeconds(3f);
        hintGameObject.SetActive(true);
    }
}
