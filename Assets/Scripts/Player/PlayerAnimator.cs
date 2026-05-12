using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Player Animator
///
/// Reads the NavMeshAgent's actual velocity to drive Animator parameters.
/// When the agent is walking, velocity > 0. When stopped, velocity ≈ 0.
///
/// ═══════════════════════════════════════════════════════════════
/// 💡 CONCEPT: Reading NavMeshAgent velocity
///   We don't control the movement directly anymore (NavMeshAgent does).
///   So we READ from the agent: "how fast are you actually moving?"
///   agent.velocity.magnitude → 0 = stopped, >0 = moving
///
/// 💡 CONCEPT: Animator Parameter Hashes
///   Animator.StringToHash("IsMoving") converts the name to an int ONCE.
///   Using the int is faster than re-evaluating the string every frame.
/// ═══════════════════════════════════════════════════════════════
/// </summary>
[RequireComponent(typeof(Animator))]
public class PlayerAnimator : MonoBehaviour
{
    [Header("References")]
    [Tooltip("NavMeshAgent on the same GameObject — we read its velocity")]
    [SerializeField] private NavMeshAgent agent;

    // Cache the hash ONCE at startup — reuse forever
    private static readonly int IsMovingHash = Animator.StringToHash("IsMoving");

    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();

        // Auto-find agent if not assigned
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (agent == null) return;

        // Is the agent actually moving right now?
        bool isMoving = agent.velocity.magnitude > 0.1f;

        // Drive the Animator — your Animator Controller must have
        // a bool parameter named exactly "IsMoving"
        _animator.SetBool(IsMovingHash, isMoving);
    }
}
