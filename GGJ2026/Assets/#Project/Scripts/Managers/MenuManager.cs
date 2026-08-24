using UnityEngine;
using UnityEngine.InputSystem;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance { get; private set; }

    [SerializeField] private GameObject menuPanel;

    private bool _isPaused;
    private GameController.GameState _currentGameState;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void OnEnable()
    {
        GameController.GameStateChanged += OnGameStateChanged;
    }

    private void OnDisable()
    {
        GameController.GameStateChanged -= OnGameStateChanged;
    }

    private void OnGameStateChanged(object sender, GameController.GameState state)
    {
        _currentGameState = state;
        // Auto-unpause if the game leaves a pausable state (e.g. player dies while paused)
        if (_isPaused && !CanPause(state))
            SetPaused(false);
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            TryToggle();

        if (Gamepad.current != null && Gamepad.current.startButton.wasPressedThisFrame)
            TryToggle();
    }

    private void TryToggle()
    {
        if (!_isPaused && !CanPause(_currentGameState))
            return;
        SetPaused(!_isPaused);
    }

    // Movement/Building are normal gameplay; EndGame (level completed) should also
    // allow opening the menu so the player can quit without needing to restart first.
    private static bool CanPause(GameController.GameState state) =>
        state == GameController.GameState.Movement ||
        state == GameController.GameState.Building ||
        state == GameController.GameState.EndGame;

    private void SetPaused(bool paused)
    {
        _isPaused = paused;
        menuPanel.SetActive(paused);
        Time.timeScale = paused ? 0f : 1f;
        AudioListener.pause = paused;
    }

    public void Resume() => SetPaused(false);

    public void QuitGame()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
