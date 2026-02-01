using System.Collections;
using UnityEngine;

public class BuildModeHint : MonoBehaviour {
    public GameObject buildModeHint;
  
    private void OnEnable()
    {
        GameController.GameStateChanged += ToggleVisibility;
    }

    private void OnDisable()
    {
        GameController.GameStateChanged -= ToggleVisibility;
    }
    
    private void ToggleVisibility(object sender, GameController.GameState gameState) {
        if (gameState == GameController.GameState.Building) {
            StartCoroutine(WaitAndSetVisibility());
        }
        else {
            StopAllCoroutines();
            buildModeHint.SetActive(false);
        }
    }

    private IEnumerator WaitAndSetVisibility() {
        yield return new WaitForSeconds(3f);
        buildModeHint.SetActive(true);
    }
}
