using UnityEngine;

public class ProjectileInputSpawnerHitboxBehavior : MonoBehaviour, IInteractable
{
    [field: SerializeField]
    public ProjectileInputSpawnerController Controller { get; private set; }

    [field: SerializeField]
    public LineRenderer BorderIndicator { get; private set; }

    [field: SerializeField]
    public CircleCollider2D Collider { get; private set; }

    [field: SerializeField]
    public int BorderIndicatorSegments { get; private set; }

    [field: SerializeField]
    public float RotationSpeed { get; private set; }

    private bool BorderIndicatorRotating { get; set; }

    private DragIndicatorController DragIndicator { get; set; }

    private float InitialColliderRadius { get; set; }

    private void Awake()
    {
        DragIndicator = Controller.DragIndicator;
        InitialColliderRadius = Collider.radius;

        InitializeBorderIndicator();
    }

    private void Update()
    {
        if (!BorderIndicatorRotating)
            return;

        var offset = Time.time * RotationSpeed;
        BorderIndicator.material.mainTextureOffset = new Vector2(offset, 0);
    }

    public void OnDragStart(Vector2 pos)
    {
        DragIndicator.StartDrag(pos);

        Collider.radius = 100f;
    }

    public void OnDragEnd(Vector2 pos)
    {
        Collider.radius = InitialColliderRadius;

        var launchForce = DragIndicator.GetLaunchVector();
        var launchPosition = DragIndicator.GetLaunchPosition();

        DragIndicator.EndDrag();

        var spawnPosition = launchPosition.ClosestPointInCircle(Controller.transform.position, Collider.radius);
        Controller.SpawnProjectile(spawnPosition, launchForce);
    }

    public void OnDrag(Vector2 pos)
        => DragIndicator.Drag(pos);

    public void OnPointerEnter(Vector2 pos)
        => EnableSpawner();

    public void OnPointerExit(Vector2 pos)
    {
        if (!DragIndicator.IsDragging)
            DisableSpawner();
    }

    private void EnableSpawner()
    {
        BorderIndicatorRotating = true;

        BorderIndicator.material.color = Color.white;
    }

    private void DisableSpawner()
    {
        BorderIndicatorRotating = false;
        DragIndicator.EndDrag();

        BorderIndicator.material.color = Color.gray;
    }

    private void InitializeBorderIndicator()
    {
        BorderIndicator.material.color = Color.gray;
        BorderIndicator.positionCount = BorderIndicatorSegments;

        for (var i = 0; i < BorderIndicatorSegments; i++)
        {
            var angle = i * Mathf.PI * 2f / BorderIndicatorSegments;
            var x = Mathf.Cos(angle) * Collider.radius;
            var y = Mathf.Sin(angle) * Collider.radius;
            BorderIndicator.SetPosition(i, new Vector3(x, y, 0));
        }
    }
}
