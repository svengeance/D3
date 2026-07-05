using UnityEngine;

public class EnemySpawnerController : MonoBehaviour
{
    [SerializeField]
    private EnemyController _enemy;

    [SerializeField]
    private PlayerController _player;

    [SerializeField]
    private float _spawnFrequency = 1.0f;

    // 0 = spawn forever. Any positive value means spawn exactly that many enemies.
    [SerializeField]
    private int _spawnCount;

    [SerializeField]
    private Vector2 _spawnOffset = Vector2.zero;

    [SerializeField]
    private float _spawnYRange = 3f;

    private int _spawnedCount;

    private void Start()
    {
        if (_spawnCount > 0)
        {
            Spawn();
            if (_spawnCount > 1)
                InvokeRepeating(nameof(Spawn), _spawnFrequency, _spawnFrequency);
        }
        else
        {
            InvokeRepeating(nameof(Spawn), 0.5f, _spawnFrequency);
        }
    }

    private void Spawn()
    {
        if (_spawnCount > 0 && _spawnedCount >= _spawnCount)
        {
            CancelInvoke(nameof(Spawn));
            return;
        }

        var randomY = _spawnYRange > 0f ? Random.Range(-_spawnYRange, _spawnYRange) : 0f;
        var spawnPosition = (Vector2)transform.position + _spawnOffset;
        spawnPosition.y += randomY;

        var spawnedEnemy = Instantiate(_enemy, spawnPosition, Quaternion.identity);
        spawnedEnemy.name = "Enemy";

        _spawnedCount++;

        if (_spawnCount > 0 && _spawnedCount >= _spawnCount)
            CancelInvoke(nameof(Spawn));
    }
}
