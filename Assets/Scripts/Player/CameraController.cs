using UnityEngine;

/// <summary>
/// A 2D camera controller that smoothly follows a target with an asymmetrical offset based on movement direction.
/// This creates a "look-ahead" effect, showing more of the screen in the direction the player is moving.
/// The camera smoothly returns to a centered position when the player is stationary.
/// </summary>
public class CameraController : MonoBehaviour
{
    [Header("Target")]
    [Tooltip("The Transform of the player or object the camera should follow.")]
    public Transform target;

    [Tooltip("The Rigidbody2D of the player. Used to determine movement direction based on velocity.")]
    public Rigidbody2D playerRigidbody;

    [Header("Smoothing and Offset")]
    [Tooltip("How quickly the camera follows the target while it's moving. Lower values are faster.")]
    [Range(0.02f, 1.0f)]
    public float movingSmoothTime = 0.25f;

    [Tooltip("How quickly the camera recenters horizontally on the target when it's idle. Lower values are faster.")]
    [Range(0.02f, 5.0f)]
    public float idleSmoothTimeX = 0.5f;

    [Tooltip("How quickly the camera recenters vertically on the target when it's idle. Lower values are faster.")]
    [Range(0.02f, 1.0f)]
    public float idleSmoothTimeY = 0.5f;

    [Tooltip("The maximum horizontal distance the camera will offset from the target.")]
    public float horizontalOffset = 3f;

    [Tooltip("A fixed vertical offset from the target's position.")]
    public float verticalOffset = 1f;

    [Tooltip("The distance the player must move horizontally from a standstill before the camera's look-ahead is triggered.")]
    public float lookAheadDeadZone = 1f;

    [Tooltip("How quickly the camera's horizontal offset is applied when the player changes direction.")]
    public float offsetChangeSpeed = 5f;

    // Private variables used by the smoothing algorithm
    private float cameraVelocityX = 0f;
    private float cameraVelocityY = 0f;
    private float currentHorizontalOffset = 0f;
    private float deadZoneAnchorX; // The position from which the dead zone is measured.

    // A small threshold to prevent the camera from shifting due to minor physics jitter when stationary.
    private const float VELOCITY_THRESHOLD = 0.1f;

    void Start()
    {
        // Initialize the dead zone anchor to the target's starting position.
        if (target != null)
        {
            deadZoneAnchorX = target.position.x;
        }
    }

    void LateUpdate()
    {
        // Exit early if the target or its rigidbody is not assigned.
        if (target == null || playerRigidbody == null)
        {
            Debug.LogWarning("Camera controller is missing a target or player Rigidbody2D. Please assign it in the Inspector.");
            return;
        }

        // Determine if the player is considered to be moving horizontally
        bool isMoving = Mathf.Abs(playerRigidbody.linearVelocity.x) > VELOCITY_THRESHOLD;

        // 1. DETERMINE THE TARGET HORIZONTAL OFFSET
        float targetHorizontalOffset = 0f;
        if (isMoving)
        {
            // Player is moving. Check if they have moved past the dead zone from our last stationary point.
            if (Mathf.Abs(target.position.x - deadZoneAnchorX) > lookAheadDeadZone)
            {
                // Trigger the look-ahead offset.
                targetHorizontalOffset = horizontalOffset * Mathf.Sign(playerRigidbody.linearVelocity.x);
            }
            // If inside the dead zone, targetHorizontalOffset remains 0, keeping the camera centered.
        }
        else
        {
            // Player is idle. Reset the anchor for the dead zone to their current position.
            // The next movement will be measured from this spot.
            deadZoneAnchorX = target.position.x;
            targetHorizontalOffset = 0f;
        }

        // 2. SMOOTHLY INTERPOLATE THE CURRENT OFFSET
        // Instead of instantly changing the offset, we smoothly Lerp towards the target offset.
        // This prevents jarring camera jumps when the player quickly changes direction or stops.
        currentHorizontalOffset = Mathf.Lerp(currentHorizontalOffset, targetHorizontalOffset, Time.deltaTime * offsetChangeSpeed);

        // 3. CALCULATE THE CAMERA'S TARGET POSITION
        // Start with the player's position and apply the smoothed horizontal and fixed vertical offsets.
        Vector3 targetPosition = new Vector3(
            target.position.x + currentHorizontalOffset,
            target.position.y + verticalOffset,
            transform.position.z // Keep the camera's original Z position.
        );

        // 4. APPLY SMOOTH DAMPING PER-AXIS
        float finalPosX;
        float finalPosY;

        if (isMoving)
        {
            // When moving, use the single movingSmoothTime for both axes for a consistent feel.
            finalPosX = Mathf.SmoothDamp(transform.position.x, targetPosition.x, ref cameraVelocityX, movingSmoothTime);
            finalPosY = Mathf.SmoothDamp(transform.position.y, targetPosition.y, ref cameraVelocityY, movingSmoothTime);
        }
        else
        {
            // When idle, use the separate X and Y smooth times for custom recentering.
            finalPosX = Mathf.SmoothDamp(transform.position.x, targetPosition.x, ref cameraVelocityX, idleSmoothTimeX);
            finalPosY = Mathf.SmoothDamp(transform.position.y, targetPosition.y, ref cameraVelocityY, idleSmoothTimeY);
        }

        // 5. SET THE NEW CAMERA POSITION
        transform.position = new Vector3(finalPosX, finalPosY, transform.position.z);
    }
}

