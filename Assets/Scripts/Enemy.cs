using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    bool movingEnemy = true;

    public float moveSpeed = 2f;
    [SerializeField]
    bool movementDirection = true;

    Vector2 MovementDirection => (movementDirection ? Vector2.left : Vector2.right);

    float raycastDistance = 0.8f;
    public LayerMask wallLayer;

    bool isWaiting = false;

    // private SpriteRenderer spriteRenderer;

    IEnumerator Wait() {
        isWaiting = true;
        yield return new WaitForSeconds(0.5f);
        isWaiting = false;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        if (movingEnemy)
        {
            move();
        }
    }

    bool detectWall()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, MovementDirection, raycastDistance, wallLayer);
        return hit.collider != null;
    }

    bool detectHole()
    {
        Vector2 origin = (Vector2)transform.position + (Vector2)MovementDirection.normalized * 0.7f;
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, raycastDistance, wallLayer);
        return hit.collider == null;
    }

    // void flipSprite()
    // {
    //     spriteRenderer.flipX = movementDirection;
    // }

    void move()
    {
        if (isWaiting) return;

        if (detectWall() || detectHole())
        {
            StartCoroutine(Wait());
            movementDirection = !movementDirection;
            // flipSprite();
        }

        Vector3 movement = moveSpeed * Time.fixedDeltaTime * MovementDirection;
        transform.Translate(movement);
    }
}