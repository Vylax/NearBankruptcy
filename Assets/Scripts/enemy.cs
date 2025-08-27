using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float moveSpeed = 2f;
    bool movementDirection = true;

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
        move();
    }

    void move()
    {
        Vector2 raycastDirection = movementDirection ? Vector2.left : Vector2.right;
        
        RaycastHit2D hit = Physics2D.Raycast(transform.position, raycastDirection);
        
        if (hit.collider != null)
        {
            movementDirection = !movementDirection;
        }

        if (movementDirection)
        {
            Vector3 movement = Vector3.left * moveSpeed * Time.fixedDeltaTime;
            transform.Translate(movement);
        }
        else
        {
            Vector3 movement = Vector3.right * moveSpeed * Time.fixedDeltaTime;
            transform.Translate(movement);
        }
    }
}
