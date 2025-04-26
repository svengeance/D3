using UnityEngine;

[CreateAssetMenu(fileName = "Enemy", menuName = "Scriptable Objects/Enemy")]
public class EnemyData : ScriptableObject
{
    [SerializeField] public Sprite sprite;
    [SerializeField] public int health;
    [SerializeField] public int damage;
    [SerializeField] public float speed;
    [SerializeField] public float weight;
}
