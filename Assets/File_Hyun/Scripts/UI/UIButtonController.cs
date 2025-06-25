using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class UIButtonController : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    [Header("이웃 버튼 지정")]
    public GameObject upButton;
    public GameObject downButton;
    public GameObject leftButton;
    public GameObject rightButton;

    [Header("클릭 후 선택될 버튼")]
    public GameObject nextOnClick;

    [Header("이펙트")]
    public UnityEvent onSelect;
    public UnityEvent onDeselect;

    public GameObject GetNeighbor(Vector2 direction)
    {
        if (direction == Vector2.up) return upButton;
        if (direction == Vector2.down) return downButton;
        if (direction == Vector2.left) return leftButton;
        if (direction == Vector2.right) return rightButton;
        return null;
    }

    public void OnSelect(BaseEventData eventData)
    {
        InputManager.Instance.lastSelectedButton = this.gameObject;
        onSelect?.Invoke();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        onDeselect?.Invoke();
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        DrawArrow(transform, upButton, Color.green);
        DrawArrow(transform, downButton, Color.red);
        DrawArrow(transform, leftButton, Color.blue);
        DrawArrow(transform, rightButton, Color.yellow);
    }

    void DrawArrow(Transform from, GameObject to, Color color)
    {
        if (to == null) return;

        Vector3 start = from.position;
        Vector3 end = to.transform.position;
        Vector3 dir = (end - start).normalized;

        Gizmos.color = color;
        Gizmos.DrawLine(start, end);

        Handles.color = color;

        float arrowHeadLength = 0.15f;
        float arrowHeadWidth = 0.1f;

        Vector3 right = Quaternion.Euler(0, 0, 90) * dir;
        Vector3 p1 = end;
        Vector3 p2 = end - dir * arrowHeadLength + right * arrowHeadWidth;
        Vector3 p3 = end - dir * arrowHeadLength - right * arrowHeadWidth;

        Handles.DrawAAConvexPolygon(p1, p2, p3);
    }
#endif
}