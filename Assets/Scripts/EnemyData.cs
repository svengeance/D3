using UnityEngine;

[CreateAssetMenu(fileName = "Enemy", menuName = "Scriptable Objects/Enemy")]
public class EnemyData : ScriptableObject
{
    [SerializeField]
    public Sprite _sprite;

    [SerializeField]
    public int _health;

    [SerializeField]
    public int _damage;

    [SerializeField]
    public float _speed;

    [SerializeField]
    public float _weight;
}
