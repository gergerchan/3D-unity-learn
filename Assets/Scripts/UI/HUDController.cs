using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// HUD Controller
///
/// Displays the shelf restock progress and the win screen.
/// Updates ONLY when events fire — never checks state every frame.
///
/// ═══════════════════════════════════════════════════════════════
/// 💡 CONCEPT: Event-driven UI vs Polling
///   BAD:  void Update() { progressText.text = gameManager.count; }
///   GOOD: Subscribe once, update only when something changes
///   This saves CPU and keeps UI code out of game logic.
/// ═══════════════════════════════════════════════════════════════
/// </summary>
public class HUDController : MonoBehaviour
{
    [Header("Progress UI")]
    [Tooltip("TextMeshPro text that shows '1 / 3 Shelves'")]
    [SerializeField] private TextMeshProUGUI progressText;

    [Header("Interact Prompt")]
    [Tooltip("The '[ E ] Restock' prompt — shown when near a shelf")]
    [SerializeField] private GameObject interactPrompt;

    [Header("Win Screen")]
    [Tooltip("The win panel — hidden at start, shown on game win")]
    [SerializeField] private GameObject winPanel;

    [Tooltip("TextMeshPro text showing total win count on the win screen")]
    [SerializeField] private TextMeshProUGUI winScoreText;
    private void OnEnable()
    {
        GameManager.OnProgressChanged += HandleProgressChanged;
        GameManager.OnGameWon += HandleGameWon;
    }

    private void OnDisable()
    {
        GameManager.OnProgressChanged -= HandleProgressChanged;
        GameManager.OnGameWon -= HandleGameWon;
    }

    private void Start()
    {
        // Hide everything until events fire
        if (winPanel != null) winPanel.SetActive(false);
        if (interactPrompt != null) interactPrompt.SetActive(false);
    }

    // Called by GameManager when a shelf is restocked
    private void HandleProgressChanged(int restocked, int required)
    {
        if (progressText != null)
            progressText.text = $"{restocked} / {required} Shelves Restocked";
    }

    // Called by GameManager when win condition is reached
    private void HandleGameWon()
    {
        if (winPanel != null) winPanel.SetActive(true);

        // PlayerPrefs already updated by DataManager — safe to read here
        if (winScoreText != null)
        {
            int totalWins = PlayerPrefs.GetInt("TotalWin", 0);
            winScoreText.text = $"Total Wins: {totalWins}";
        }
    }

    /// <summary>
    /// Called by the Restart Button's OnClick in the Inspector.
    /// Reloads the active scene to restart the game.
    /// </summary>
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Called by PlayerInteraction to show/hide the prompt.
    /// </summary>
    public void ShowInteractPrompt(bool show)
    {
        if (interactPrompt != null)
            interactPrompt.SetActive(show);
    }
}
