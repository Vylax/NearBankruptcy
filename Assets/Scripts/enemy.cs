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

    void move()
    {
        if (isWaiting) return;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, MovementDirection, raycastDistance, wallLayer);

        if (hit.collider != null)
        {
            print("aaaaaaaaaaaaaaaaaaaaaaaaaaa");
            StartCoroutine(Wait());
            movementDirection = !movementDirection;
        }

        Vector3 movement = moveSpeed * Time.fixedDeltaTime * MovementDirection;
        transform.Translate(movement);
    }
}