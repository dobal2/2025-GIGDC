using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIButtonNavigator : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static UIButtonNavigator Instance { get; private set; }

    [Header("키 설정 ScriptableObject")]
    public KeyData keyData;

    [Header("활성화된 버튼 레이어")]
    [SerializeField] private int activeButtonLayer = 0;

    public int ActiveButtonLayer => activeButtonLayer;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        GameObject current = EventSystem.current.currentSelectedGameObject;
        if (current == null) return;

        var currentSelectable = current.GetComponent<Selectable>();
        var currentButton = current.GetComponent<UIButtonController>();
        if (currentSelectable == null || currentButton == null) return;

        Vector2 input = Vector2.zero;

        if (Input.GetKeyDown(keyData.Player.UpKey)) input = Vector2.up;
        else if (Input.GetKeyDown(keyData.Player.DownKey)) input = Vector2.down;
        else if (Input.GetKeyDown(keyData.Player.LeftUpKey)) input = Vector2.left;
        else if (Input.GetKeyDown(keyData.Player.RightKey)) input = Vector2.right;

        if (input != Vector2.zero)
        {
            Selectable next = FindNextSelectable(currentSelectable, input);
            if (next != null) next.Select();
        }

        if (Input.GetKeyDown(keyData.Player.SelectKey))
        {
            Button btn = current.GetComponent<Button>();
            if (btn != null) btn.onClick.Invoke();
        }
    }

    Selectable FindNextSelectable(Selectable current, Vector2 direction)
    {
        Selectable[] all = Selectable.allSelectablesArray;
        List<Selectable> candidates = new List<Selectable>();

        foreach (var s in all)
        {
            if (s == current) continue;
            if (!s.IsInteractable() || !s.gameObject.activeInHierarchy) continue;

            var btn = s.GetComponent<UIButtonController>();
            if (btn == null || btn.buttonLayer != activeButtonLayer) continue;

            Vector3 dirTo = s.transform.position - current.transform.position;
            if (Vector2.Dot(direction.normalized, dirTo.normalized) > 0.5f)
            {
                candidates.Add(s);
            }
        }

        // 가장 가까운 후보 선택
        float minDist = float.MaxValue;
        Selectable best = null;
        foreach (var s in candidates)
        {
            float dist = (s.transform.position - current.transform.position).sqrMagnitude;
            if (dist < minDist)
            {
                minDist = dist;
                best = s;
            }
        }

        return best;
    }

    public void SetActiveLayer(int newLayer)
    {
        activeButtonLayer = newLayer;
        Debug.Log($"[UIButtonNavigator] 활성 버튼 레이어 변경됨 → {activeButtonLayer}");
    }
}