using UnityEngine;

public class GameBootstrapper : MonoBehaviour
{
    [field: SerializeField]
    public InputManager InputManager { get; private set; }

    [field: SerializeField]
    public PlayerManager PlayerManager { get; private set; }

    private void Awake()
    {
        CreateManager(InputManager);
        CreateManager(PlayerManager);
    }

    private static void CreateManager<T>(T managerPrefab) where T : MonoBehaviour
    {
        if (FindAnyObjectByType<T>())
        {
            Debug.LogWarning($"An instance of {typeof(T).Name} already exists in the scene. Destroying the new instance.");
            return;
        }

        var manager = Instantiate(managerPrefab);
        DontDestroyOnLoad(manager);
    }
}
