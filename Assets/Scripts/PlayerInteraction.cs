using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Player Interaction (Click-to-Restock)
///
/// PlayerController tells this script WHICH shelf was tapped.
/// This script watches the NavMeshAgent every frame until it arrives,
/// then triggers the restock.
///
/// ═══════════════════════════════════════════════════════════════
/// TEACHING POINT — Arrival Detection:
///   NavMeshAgent.remainingDistance = how far the agent still has to walk
///   NavMeshAgent.stoppingDistance  = how close it stops before the target
///   NavMeshAgent.pathPending        = true while Unity is still calculating the path
///
///   We wait until:
///     • pathPending is FALSE (path is calculated)
///     • remainingDistance ≤ stoppingDistance (we're close enough)
///   Only THEN do we restock — not when we just walk past!
///
/// TEACHING POINT — Why separate from PlayerController?
///   Single Responsibility Principle:
///   Controller = "where does the player go?"
///   Interaction = "what happens when the player gets there?"
///   Splitting them makes each script easier to understand and test.
/// ═══════════════════════════════════════════════════════════════
/// </summary>
public class PlayerInteraction : MonoBehaviour
{
    [Header("References")]
    [Tooltip("HUDController — to show/hide the 'walking to shelf' prompt")]
    [SerializeField] private HUDController hud;

    [Tooltip("A small extra buffer added to stoppingDistance for the arrival check")]
    [SerializeField] private float arrivalBuffer = 0.2f;

    // ── Private state ──────────────────────────────────────────────────────
    private NavMeshAgent _agent;
    private Shelf _targetShelf;         // The shelf we're heading to
    private bool _hasTarget = false;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (!_hasTarget) return;

        // Guard: if the shelf was already restocked by something else, cancel
        if (_targetShelf == null || !_targetShelf.IsEmpty)
        {
            ClearRestockTarget();
            return;
        }

        // ── Arrival check ──────────────────────────────────────────────────
        // pathPending = Unity is still computing the path → not arrived yet
        if (_agent.pathPending) return;

        // remainingDistance = how many units left to walk
        float stopAt = _agent.stoppingDistance + arrivalBuffer;

        if (_agent.remainingDistance <= stopAt)
        {
            // We've arrived! Restock the shelf.
            _targetShelf.Restock();
            ClearRestockTarget();
        }
    }

    /// <summary>
    /// Called by PlayerController when the player taps an empty shelf.
    /// </summary>
    public void SetRestockTarget(Shelf shelf)
    {
        _targetShelf = shelf;
        _hasTarget = true;

        // Show a "Heading to shelf..." prompt in the HUD
        if (hud != null)
            hud.ShowInteractPrompt(true);
    }

    /// <summary>
    /// Called when the player taps the ground instead (cancels any shelf job).
    /// </summary>
    public void ClearRestockTarget()
    {
        _targetShelf = null;
        _hasTarget = false;

        if (hud != null)
            hud.ShowInteractPrompt(false);
    }

    // Visualise the agent's stopping zone in Scene view
    private void OnDrawGizmos()
    {
        if (!_hasTarget || _agent == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _agent.stoppingDistance + arrivalBuffer);
    }
}
