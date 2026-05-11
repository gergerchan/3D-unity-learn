using System;
using UnityEngine;

/// <summary>
/// Game Manager
///
/// Tracks how many shelves have been restocked.
/// When the required number is hit, the game is won.
///
/// ═══════════════════════════════════════════════════════════════
/// TEACHING POINT — Single Responsibility:
///   • GameManager only cares about WIN/LOSE logic
///   • It doesn't know what a shelf looks like
///   • It doesn't know how the player moves
///   • It just counts and announces: "We won!"
/// ═══════════════════════════════════════════════════════════════
/// </summary>
public class GameManager : MonoBehaviour
{
    [Header("Win Condition")]
    [Tooltip("How many shelves must be restocked to win? (1/3 of total is the goal)")]
    [SerializeField] private int shelvesRequiredToWin = 3;

    // ── Event — HUDController and others listen to this ───────────────────
    public static event Action<int, int> OnProgressChanged;  // (restocked, required)
    public static event Action OnGameWon;

    private int _restockedCount = 0;
    private bool _gameWon = false;

    private void OnEnable()
    {
        // Start listening to Shelf events
        Shelf.OnShelfRestocked += HandleShelfRestocked;
    }

    private void OnDisable()
    {
        // IMPORTANT: Always unsubscribe to prevent memory leaks
        Shelf.OnShelfRestocked -= HandleShelfRestocked;
    }

    private void Start()
    {
        // Broadcast starting state to UI
        OnProgressChanged?.Invoke(_restockedCount, shelvesRequiredToWin);
    }

    // Called every time any Shelf fires OnShelfRestocked
    private void HandleShelfRestocked()
    {
        if (_gameWon) return;   // Don't count after the game is won

        _restockedCount++;
        OnProgressChanged?.Invoke(_restockedCount, shelvesRequiredToWin);

        if (_restockedCount >= shelvesRequiredToWin)
        {
            _gameWon = true;
            OnGameWon?.Invoke();
            Debug.Log("Game Won!");
        }
    }
}
