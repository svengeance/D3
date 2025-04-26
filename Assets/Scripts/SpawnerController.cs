using UnityEngine;

public class SpawnerController : MonoBehaviour
{
    [SerializeField]
    private EnemyController _enemyPrefab;

    [SerializeField]
    private PlayerController _playerPrefab;

    [SerializeField]
    private float _spawnFrequency = 1.0f;

    private void Start() =>
        InvokeRepeating(nameof(Spawn), 0.5f, _spawnFrequency);

    private void Spawn()
    {
        var randomY = Random.Range(-3f, 3f);
        var spawnPosition = new Vector2(transform.position.x, transform.position.y + randomY);

        var newEnemy = Instantiate(_enemyPrefab, spawnPosition, Quaternion.identity);
        newEnemy.Initialize(_playerPrefab);
    }
}
