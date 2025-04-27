using UnityEngine;

[CreateAssetMenu(fileName = "Enemy", menuName = "Scriptable Objects/Enemy")]
public class EnemyData : ScriptableObject
{
    public Sprite _sprite;
    public int _health;
    public int _damage;
    public float _speed;
    public float _weight;
}
