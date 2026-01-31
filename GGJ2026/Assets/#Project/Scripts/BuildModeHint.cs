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
        buildModeHint.SetActive(gameState == GameController.GameState.Building);
    }
}
