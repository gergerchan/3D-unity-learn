using System;
using UnityEngine;

/// <summary>
/// Shelf Slot
///
/// Represents ONE shelf slot in the minimarket.
/// Each shelf knows its own state: Empty or Restocked.
/// When restocked, it fires a static event so GameManager can hear it.
///
/// ═══════════════════════════════════════════════════════════════
/// TEACHING POINT — Static Events (Observer Pattern):
///   • Shelf doesn't know GameManager exists
///   • GameManager doesn't pull Shelf — Shelf pushes an event
///   • This "loose coupling" means we can add shelves without changing GameManager
/// ═══════════════════════════════════════════════════════════════
/// </summary>
public class Shelf : MonoBehaviour
{
    [Header("State")]
    [Tooltip("Start the game with this shelf already empty (needing restock)")]
    [SerializeField] private bool startsEmpty = true;

    [Header("Visuals")]
    [Tooltip("Drag the 'full shelf' child GameObject here")]
    [SerializeField] private GameObject fullShelfVisual;

    [Tooltip("Drag the 'empty shelf' child GameObject here (optional highlight)")]
    [SerializeField] private GameObject emptyShelfVisual;

    // ── Static event — GameManager listens to this ─────────────────────────
    // "static" means ALL Shelves share one event channel
    public static event Action OnShelfRestocked;

    // Is this shelf currently empty and needing to be restocked?
    public bool IsEmpty { get; private set; }

    // Unity calls this before the first frame
    private void Start()
    {
        IsEmpty = startsEmpty;
        UpdateVisuals();
    }

    /// <summary>
    /// Called by PlayerInteraction when the player is near and arrives.
    /// </summary>
    public void Restock()
    {
        if (!IsEmpty) return;   // Already full — nothing to do

        IsEmpty = false;
        UpdateVisuals();

        // Fire the event — GameManager will count this
        OnShelfRestocked?.Invoke();
    }

    // Show/hide the correct GameObjects based on state
    private void UpdateVisuals()
    {
        if (fullShelfVisual != null)
            fullShelfVisual.SetActive(!IsEmpty);    // Show when full

        if (emptyShelfVisual != null)
            emptyShelfVisual.SetActive(IsEmpty);    // Show when empty
    }

    // Draw a colored wire sphere in Scene view so designers can see the shelf state
    private void OnDrawGizmos()
    {
        Gizmos.color = IsEmpty ? Color.red : Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.3f);
    }
}
