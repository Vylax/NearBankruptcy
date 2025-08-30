using UnityEngine;

public class CheckerFlag : MonoBehaviour
{
    public float rotationSpeed = 45f; // degrees per second

    public float scaleAmplitude = 0.1f; // amplitude of scaling

    public float scaleFrequency = 2f; // frequency of scaling

    public float floatFrequency = 1f; // frequency of floating

    public float floatAmplitude = 0.1f; // amplitude of floating

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameManager.Instance.Win();
        }
    }

    private void Update()
    {
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);

        // scale up and down a bit
        float scale = 1 + scaleAmplitude * Mathf.Sin(Time.time * scaleFrequency);
        transform.localScale = new Vector3(scale, scale, 1);

        // add floating effect
        float floatY = floatAmplitude * Mathf.Sin(Time.time * floatFrequency);
        transform.position = new Vector3(transform.position.x, transform.position.y + floatY * Time.deltaTime, transform.position.z);
    }
}
