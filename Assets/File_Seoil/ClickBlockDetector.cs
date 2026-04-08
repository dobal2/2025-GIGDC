using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ClickBlockDetector : MonoBehaviour
{
    [SerializeField] private GraphicRaycaster raycaster;
    [SerializeField] private EventSystem eventSystem;

    public List<RaycastResult> GetBlockingUI(Vector2 screenPosition)
    {
        PointerEventData pointerData = new PointerEventData(eventSystem);
        pointerData.position = screenPosition;

        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(pointerData, results);
        return results;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var results = GetBlockingUI(Input.mousePosition);

            if (results.Count > 0)
            {
                foreach (var result in results)
                    Debug.Log($"막고 있는 UI: {result.gameObject.name}");
            }
        }
    }
}