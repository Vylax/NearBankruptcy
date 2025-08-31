using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float spawnInterval = 5f;
    [SerializeField] private float minY = -10f;
    [SerializeField] private float maxY = 10f;

    private void Start()
    {
        InvokeRepeating(nameof(SpawnBullet), 0f, spawnInterval);
    }

    private void SpawnBullet()
    {
        // pick a random height
        float randomY = Random.Range(minY, maxY);

        // spawn position = spawner’s X, random Y
        Vector3 spawnPos = new Vector3(transform.position.x, randomY, 0f);

        Instantiate(bulletPrefab, spawnPos, Quaternion.identity);
    }
}
