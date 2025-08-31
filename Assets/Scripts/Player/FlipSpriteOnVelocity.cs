using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SwapSpriteOnVelocity : MonoBehaviour
{
    [Tooltip("The Rigidbody2D to monitor. The sprite will change based on its horizontal velocity.")]
    [SerializeField] private Rigidbody2D targetRigidbody;

    [Tooltip("The sprite to display when the character is moving to the right.")]
    [SerializeField] private Sprite rightSprite;

    [Tooltip("The sprite to display when the character is moving to the left.")]
    [SerializeField] private Sprite leftSprite;

    private SpriteRenderer spriteRenderer;

    private const float VelocityThreshold = 0.05f;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (targetRigidbody == null)
        {
            Debug.LogError("Target Rigidbody2D is not assigned in the SwapSpriteOnVelocity script on " + gameObject.name + ". Please assign it in the Inspector.");
            enabled = false;
        }
        else if (rightSprite == null || leftSprite == null)
        {
            Debug.LogError("Right Sprite or Left Sprite is not assigned in the SwapSpriteOnVelocity script on " + gameObject.name + ". Please assign them in the Inspector.");
            enabled = false;
        }
    }

    void Update()
    {
        float horizontalVelocity = targetRigidbody.linearVelocity.x;

        if (Mathf.Abs(horizontalVelocity) > VelocityThreshold)
        {
            if (horizontalVelocity > 0)
            {
                spriteRenderer.sprite = rightSprite;
            }
            else
            {
                spriteRenderer.sprite = leftSprite;
            }
        }
    }
}