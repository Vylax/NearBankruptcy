using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float moveSpeed = 2f;
    bool movementDirection = true;
    Vector2 MovementDirection => (movementDirection ? Vector2.left : Vector2.right);

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
        Vector3 movement = moveSpeed * Time.fixedDeltaTime * MovementDirection;
        transform.Translate(movement);
    }
}
