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
    public LayerMask enemyLayer;

    bool isWaiting = false;

    // private SpriteRenderer spriteRenderer;

    IEnumerator Wait() {
        isWaiting = true;
        yield return new WaitForSeconds(0.5f);
        isWaiting = false;
        flipSprite();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        moveSpeed = Random.Range(1f, 3f);
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
        Vector2 origin = (Vector2)transform.position + (Vector2)MovementDirection.normalized * 0.8f;
        RaycastHit2D hitWall = Physics2D.Raycast(transform.position, MovementDirection, raycastDistance, wallLayer);
        RaycastHit2D hitEnemy = Physics2D.Raycast(origin, MovementDirection, raycastDistance - raycastDistance, enemyLayer);
        return hitWall.collider != null || hitEnemy.collider != null;
    }

    bool detectHole()
    {
        Vector2 originWall = (Vector2)transform.position + (Vector2)MovementDirection.normalized * 0.7f;
        Vector2 originEnemy = (Vector2)transform.position + (Vector2)MovementDirection.normalized * 0.8f;
        RaycastHit2D hitWall = Physics2D.Raycast(originWall, Vector2.down, raycastDistance, wallLayer);
        RaycastHit2D hitEnemy = Physics2D.Raycast(originEnemy, Vector2.down, raycastDistance, enemyLayer);
        return hitWall.collider == null && hitEnemy.collider == null;
    }

    void flipSprite()
    {
        GetComponent<SpriteRenderer>().flipX = !movementDirection;
    }

    void move()
    {
        if (isWaiting) return;

        if (detectWall() || detectHole())
        {
            StartCoroutine(Wait());
            movementDirection = !movementDirection;
        }

        Vector3 movement = moveSpeed * Time.fixedDeltaTime * MovementDirection;
        transform.Translate(movement);
    }
}