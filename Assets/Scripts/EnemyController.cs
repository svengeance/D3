using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField]
    private EnemyData _enemyData;

    public PlayerController _player;

    public void Initialize(PlayerController playerController) =>
        _player = playerController;
}
