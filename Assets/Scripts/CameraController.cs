using UnityEngine;

/// <summary>
/// Camera Controller
///
/// A fixed-angle camera that follows the player from above.
/// The rotation NEVER changes — only the position tracks the player.
///
/// ═══════════════════════════════════════════════════════════════
/// TEACHING POINT — The "Camera" in 3C (Isometric):
///   Classic isometric angle: 45° horizontal, 35.26° vertical (true iso)
///   In Unity: Rotation X = 30~45°, Y = 45°, Z = 0
///
///   WHY fixed angle?
///   • Player always knows which way is "forward" on screen
///   • No motion sickness from camera spinning
///   • Perfect for top-down strategy / puzzle games (Stardew Valley, Hades)
///
/// TEACHING POINT — LateUpdate vs Update:
///   Camera MUST move in LateUpdate, AFTER the player has moved.
///   If you use Update, the camera jitters because player and camera
///   move in unpredictable order within the same frame.
/// ═══════════════════════════════════════════════════════════════
/// </summary>
public class CameraController : MonoBehaviour
{
    [Header("Target")]
    [Tooltip("Drag the Player GameObject here")]
    [SerializeField] private Transform target;

    [Header("Camera Settings")]
    [Tooltip("How high and far behind the camera sits above the player")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 8f, -6f);

    [Tooltip("Smoothness of camera movement. Higher = snappier. 0 = instant")]
    [SerializeField] private float smoothSpeed = 8f;

    // Called every frame AFTER all Update() calls have run
    private void Update()
    {
        if (target == null) return;

        // ── Step 1: Calculate where the camera should be ──────────────────
        // We always add the SAME offset — camera never rotates
        Vector3 desiredPosition = target.position + offset;

        // ── Step 2: Smoothly move towards that position ────────────────────
        // Lerp = Linear Interpolation: moves a fraction of the remaining distance each frame
        // smoothSpeed * Time.deltaTime controls how fast we close the gap
        transform.position = Vector3.Lerp(
            transform.position,     // Where we are now
            desiredPosition,        // Where we want to be
            smoothSpeed * Time.deltaTime
        );

        // ── Note: We do NOT change transform.rotation ─────────────────────
        // Set the camera rotation ONCE in the Inspector: X=45, Y=0, Z=0
        // (or wherever you want the fixed isometric angle)
    }

    // Draw a line in Scene view showing where the camera is relative to player
    private void OnDrawGizmos()
    {
        if (target == null) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, target.position);
        Gizmos.DrawWireSphere(target.position + offset, 0.3f);
    }
}
