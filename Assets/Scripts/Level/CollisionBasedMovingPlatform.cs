using UnityEngine;

/// <summary>
/// Controls a 2D platform that moves back and forth along a specified axis.
/// It uses a BoxCast to detect upcoming collisions with the "World" layer and reverses direction.
/// This method is reliable for a Kinematic Rigidbody interacting with Static colliders.
/// This component requires a Rigidbody2D (set to Kinematic) and a BoxCollider2D on the same GameObject.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))] // We now specifically need a BoxCollider2D for this logic
public class CollisionBasedMovingPlatform : MonoBehaviour
{
    // Enum to make axis selection clear and easy in the Inspector.
    public enum MovementAxis { Horizontal, Vertical }

    [Header("Movement Settings")]
    [Tooltip("The axis on which the platform will move.")]
    public MovementAxis axis = MovementAxis.Horizontal;

    [Tooltip("The speed at which the platform moves.")]
    public float speed = 3f;

    [Header("Collision Detection")]
    [Tooltip("The layer the platform will check for collisions to reverse direction.")]
    public LayerMask collisionLayer;

    [Tooltip("How far ahead the platform checks for collisions. Should be a small value.")]
    public float collisionCheckDistance = 0.1f;

    // --- Private Variables ---
    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;
    private Vector2 moveDirection;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();

        // Ensure the Rigidbody is set to Kinematic.
        rb.isKinematic = true;
        rb.gravityScale = 0;
        rb.sleepMode = RigidbodySleepMode2D.NeverSleep;

        // Set the initial movement direction based on the user's choice in the Inspector.
        if (axis == MovementAxis.Horizontal)
        {
            moveDirection = Vector2.right;
        }
        else // Vertical
        {
            moveDirection = Vector2.up;
        }
    }

    void FixedUpdate()
    {
        // --- Proactive Collision Check ---
        // Before moving, we cast a box in our direction of travel to see if we're about to hit something.
        RaycastHit2D hit = Physics2D.BoxCast(
            boxCollider.bounds.center,      // The center of the box to cast
            boxCollider.bounds.size,        // The size of the box to cast
            0f,                             // Angle of the box
            moveDirection,                  // Direction to cast the box
            collisionCheckDistance,         // Max distance to cast
            collisionLayer                  // The layer(s) to check against
        );

        // If the BoxCast hit something on our collision layer...
        if (hit.collider != null)
        {
            // Reverse the direction of movement.
            moveDirection *= -1;
        }

        // --- Movement ---
        // We use FixedUpdate for physics-based movement.
        // Rigidbody2D.MovePosition handles movement smoothly and correctly for a kinematic body.
        Vector2 newPosition = rb.position + moveDirection * speed * Time.fixedDeltaTime;
        rb.MovePosition(newPosition);
    }

    // OnCollisionEnter2D is no longer needed with this approach.

    // --- VISUAL DEBUGGING ---
    // This draws a wireframe box in the Scene view to show the BoxCast area.
    private void OnDrawGizmos()
    {
        if (boxCollider != null)
        {
            Gizmos.color = Color.red;
            Vector3 gizmoPosition = boxCollider.bounds.center + (Vector3)(moveDirection * collisionCheckDistance);
            Gizmos.DrawWireCube(gizmoPosition, boxCollider.bounds.size);
        }
    }
}

