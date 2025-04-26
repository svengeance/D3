using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField]
    private EnemyData enemyData;
    
    public PlayerController player;

    public void Initialize(PlayerController playerController)
    {
        player = playerController;
    }
}
