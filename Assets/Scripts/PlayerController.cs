using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

/// <summary>
/// Point-and-Click Player Controller
///
/// Tap the ground  → agent walks there
/// Tap a shelf     → agent walks to that shelf → PlayerInteraction restocks on arrival
///
/// ═══════════════════════════════════════════════════════════════
/// ⚠️ COMMON SETUP MISTAKES:
///   1. Player sinks underground → fix NavMeshAgent Base Offset (see Awake)
///   2. Shelf click does nothing → check shelfLayer mask + Shelf layer on collider
///   3. Ground click does nothing → check groundLayer mask + Ground layer on floor
///
/// Enable "debugMode" in the Inspector to see raycast results in the Console.
/// ═══════════════════════════════════════════════════════════════
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [Tooltip("Walk speed — keep in sync with NavMeshAgent Speed field")]
    [SerializeField] private float moveSpeed = 4f;

    [Header("NavMesh Height Fix")]
    [Tooltip("Raise/lower the agent so the character stands on the floor, not inside it.\n" +
             "• Capsule (pivot at center, height=2) → set to 1\n" +
             "• Kenney character (pivot at feet)    → set to 0\n" +
             "This sets NavMeshAgent.baseOffset at startup.")]
    [SerializeField] private float agentBaseOffset = 1f;

    [Header("Raycast Layers")]
    [Tooltip("Your floor/ground plane layer — tap here to just move")]
    [SerializeField] private LayerMask groundLayer;

    [Tooltip("Your shelf objects layer — tap here to move + restock on arrival")]
    [SerializeField] private LayerMask shelfLayer;

    [Header("Click Indicator (optional)")]
    [Tooltip("Small disc prefab shown where you tapped")]
    [SerializeField] private GameObject clickIndicatorPrefab;

    [Header("Debug")]
    [Tooltip("Enable to print raycast results to the Console — helps diagnose click issues")]
    [SerializeField] private bool debugMode = true;

    // ── Private references ─────────────────────────────────────────────────
    private NavMeshAgent _agent;
    private Camera _mainCamera;
    private PlayerInteraction _interaction;
    private GameObject _clickIndicatorInstance;

    public bool IsMoving => _agent != null && _agent.velocity.magnitude > 0.1f;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _mainCamera = Camera.main;
        _interaction = GetComponent<PlayerInteraction>();

        // ── Fix: set the base offset so the character stands ON the floor ──
        _agent.baseOffset = agentBaseOffset;
        _agent.speed = moveSpeed;

        if (clickIndicatorPrefab != null)
        {
            _clickIndicatorInstance = Instantiate(clickIndicatorPrefab);
            _clickIndicatorInstance.SetActive(false);
        }
    }

    private void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        // Mouse click (Editor / Desktop)
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            ProcessTap(Mouse.current.position.ReadValue());

        // Touch (iOS)
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
            ProcessTap(Touchscreen.current.primaryTouch.position.ReadValue());
    }

    private void ProcessTap(Vector2 screenPos)
    {
        Ray ray = _mainCamera.ScreenPointToRay(screenPos);

        if (debugMode)
            Debug.Log($"[PlayerController] Tap at screen {screenPos}");

        // ── Check Shelf layer FIRST (shelf sits above ground) ─────────────
        if (Physics.Raycast(ray, out RaycastHit shelfHit, 100f, shelfLayer))
        {
            Shelf slot = shelfHit.collider.GetComponentInParent<Shelf>();

            if (debugMode)
                Debug.Log($"[PlayerController] Shelf ray hit: {shelfHit.collider.gameObject.name}" +
                          $" | Has Shelf: {slot != null}" +
                          $" | IsEmpty: {slot?.IsEmpty}");

            if (slot != null && slot.IsEmpty)
            {
                _agent.SetDestination(shelfHit.point);
                _interaction?.SetRestockTarget(slot);
                ShowClickIndicator(shelfHit.point);
                return;
            }
        }

        // ── Check Ground layer — plain move ───────────────────────────────
        if (Physics.Raycast(ray, out RaycastHit groundHit, 100f, groundLayer))
        {
            if (debugMode)
                Debug.Log($"[PlayerController] Ground ray hit: {groundHit.collider.gameObject.name}" +
                          $" | Point: {groundHit.point}");

            _agent.SetDestination(groundHit.point);
            _interaction?.ClearRestockTarget();
            ShowClickIndicator(groundHit.point);
        }
        else
        {
            if (debugMode)
                Debug.LogWarning("[PlayerController] Raycast hit NOTHING. " +
                                 "Check: (1) groundLayer is set in Inspector, " +
                                 "(2) your floor has the correct Layer assigned.");
        }
    }

    private void ShowClickIndicator(Vector3 worldPos)
    {
        if (_clickIndicatorInstance == null) return;
        _clickIndicatorInstance.transform.position = worldPos + Vector3.up * 0.02f;
        _clickIndicatorInstance.SetActive(true);
    }
}
