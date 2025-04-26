using UnityEngine;

public class SpawnerController : MonoBehaviour
{
    [SerializeField]
    private GameObject enemy;

    [SerializeField]
    private float spawnFrequency = 1.0f;

    void Start()
    {
        InvokeRepeating(nameof(Spawn), 0.5f, 1f);
    }

    private void Spawn()
    {
        var randomY = Random.Range(-3, 3);
        var spawnPosition = new Vector3(transform.position.x, transform.position.y + randomY, transform.position.z);

        var newEnemy = Instantiate(enemy);
        newEnemy.transform.position = spawnPosition;
    }
}