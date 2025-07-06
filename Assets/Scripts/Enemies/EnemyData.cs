using UnityEngine;

[CreateAssetMenu(fileName = "Enemy", menuName = "Scriptable Objects/Enemy")]
public class EnemyData : ScriptableObject
{
    [field: SerializeField]
    public Sprite Sprite { get; set; }

    [field: SerializeField]
    public int StartingHealth { get; set; }

    [field: SerializeField]
    public int Damage { get; set; }

    [field: SerializeField]
    public float Speed { get; set; }

    [field: SerializeField]
    public float Weight { get; set; }
}
