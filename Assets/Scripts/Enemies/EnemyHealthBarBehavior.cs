using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBarBehavior : MonoBehaviour
{
    [field: SerializeField]
    private Slider HealthBar { get; set; }

    public void UpdateHealthBar(float currentHealth, float maxHealth)
        => HealthBar.value = 1 - currentHealth / maxHealth;
}
