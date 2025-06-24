using UnityEngine;

public interface IInteractable
{
    void OnClick(Vector2 pos) { }

    void OnMouseDown(Vector2 pos) { }
    void OnMouseUp(Vector2 pos) { }

    void OnPointerEnter(Vector2 pos) { }
    void OnPointerExit(Vector2 pos) { }

    void OnDragStart(Vector2 pos) { }
    void OnDrag(Vector2 pos) { }
    void OnDragEnd(Vector2 pos) { }
}
