using UnityEngine;

public class OutOfBounds : MonoBehaviour
{
    public float minY = -100f;
    public float maxY = 100f;
    public float minX = int.MinValue;
    public float maxX = int.MaxValue;

    private GameObject player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("OutOfBounds: Player object not found in the scene.");
        }
    }

    private void Update()
    {
        if (player != null)
        {
            Vector3 playerPosition = player.transform.position;
            if (playerPosition.y < minY || playerPosition.y > maxY || playerPosition.x < minX || playerPosition.x > maxX)
            {
                Debug.Log("OutOfBounds: Player is out of bounds. Triggering death.");
                GameManager.Instance.Die();
            }
        }
    }
}
