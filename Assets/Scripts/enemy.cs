using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    bool movingEnemy = true;

    public float moveSpeed = 2f;
    bool movementDirection = true;

    float raycastDistance = 0.8f;
    LayerMask wallLayer = 6;

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
        if (!isWaiting)
        {
            Vector2 raycastDirection = movementDirection ? Vector2.left : Vector2.right;
        
            RaycastHit2D hit = Physics2D.Raycast(transform.position, raycastDirection, raycastDistance, wallLayer);
            
            if (hit.collider != null)
            {
                print("aaaaaaaaaaaaaaaaaaaaaaaaaaa");
                StartCoroutine(Wait());
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
}