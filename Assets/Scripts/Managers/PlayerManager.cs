using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [field: SerializeField]
    public ProjectileController SelectedProjectileData { get; private set; }

    [field: SerializeField]
    public PlayerController Player { get; private set; }

    public static PlayerManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        transform.SetParent(null);
        DontDestroyOnLoad(gameObject);
    }
}
