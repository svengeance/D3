using JetBrains.Annotations;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private D3Input _d3Input;
    private bool _isDragging;
    private Camera _mainCamera;

    private Vector2 _pointerPressPosition;

    [CanBeNull]
    private IInteractable _pointerTarget;

    public static InputManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        _d3Input = new D3Input();
        _mainCamera = Camera.main;

        transform.SetParent(null);
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        var pointer = GetWorldPoint(_d3Input.Gameplay.Pointer.ReadValue<Vector2>());
        var target = RaycastToInteractable(pointer);

        HandleClickAction(pointer, target);
        HandleHoverAction(pointer, target);
    }

    private void OnEnable()
        => _d3Input?.Enable();

    private void OnDisable()
        => _d3Input?.Disable();

    private void HandleHoverAction(Vector2 pointer, [CanBeNull] IInteractable target)
    {
        if (target == _pointerTarget)
            return;

        _pointerTarget?.OnPointerExit(pointer);
        _pointerTarget = target;
        _pointerTarget?.OnPointerEnter(pointer);
    }

    private void HandleClickAction(Vector2 pointer, IInteractable target)
    {
        if (_d3Input.Gameplay.Click.WasPressedThisFrame())
        {
            _pointerPressPosition = pointer;
            _pointerTarget = target;
            _pointerTarget?.OnMouseDown(pointer);
            _isDragging = false;
        }

        if (_d3Input.Gameplay.Click.IsPressed())
        {
            var moved = Vector2.Distance(_pointerPressPosition, pointer);

            if (_isDragging)
            {
                _pointerTarget?.OnDrag(pointer);
            }
            else if (moved > 0.1f)
            {
                _pointerTarget?.OnDragStart(_pointerPressPosition);
                _isDragging = true;
            }
        }

        if (_d3Input.Gameplay.Click.WasReleasedThisFrame())
        {
            _pointerTarget?.OnMouseUp(pointer);

            if (_isDragging)
            {
                _isDragging = false;
                _pointerTarget?.OnDragEnd(pointer);
            }
            else
            {
                _pointerTarget?.OnClick(pointer);
            }

            _pointerTarget = null;
        }
    }

    private Vector2 GetWorldPoint(Vector2 screenPoint)
        => _mainCamera.ScreenToWorldPoint(screenPoint);

    [CanBeNull]
    private static IInteractable RaycastToInteractable(Vector2 worldPoint)
    {
        var hitObject = Physics2D.OverlapPoint(worldPoint);
        return hitObject?.TryGetComponent<IInteractable>(out var interactable) == true
            ? interactable
            : null;
    }
}
