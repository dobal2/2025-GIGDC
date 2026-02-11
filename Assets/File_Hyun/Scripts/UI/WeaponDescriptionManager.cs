using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class WeaponDescriptionManager : MonoBehaviour
{
    [SerializeField] WeaponDatabase database;

    [Header("무기 UI 부모")]
    [SerializeField] RectTransform weaponUIContainer;

    [Header("무기 UI")]
    [SerializeField] GameObject spearUI;
    [SerializeField] GameObject bowUI;
    [SerializeField] GameObject bombUI;

    [Header("슬롯 좌표")]
    [SerializeField] float[] slotXPositions = new float[3];
    [SerializeField] float slotYPosition;

    Dictionary<WeaponType, GameObject> weaponUIs;
    readonly List<WeaponType> unlockedWeapons = new();
    int currentIndex = 0;

    void Awake()
    {
        weaponUIs = new()
        {
            { WeaponType.Spear, spearUI },
            { WeaponType.Bow, bowUI },
            { WeaponType.Bomb, bombUI }
        };
    }

    void OnEnable()
    {
        SetupDisplay();
        InputManager.Instance.OnUILeftKeyDown += () => TryMove(-1);
        InputManager.Instance.OnUIRightKeyDown += () => TryMove(1);
    }

    void OnDisable()
    {
        InputManager.Instance.OnUILeftKeyDown -= () => TryMove(-1);
        InputManager.Instance.OnUIRightKeyDown -= () => TryMove(1);
    }

    void SetupDisplay()
    {
        unlockedWeapons.Clear();

        foreach (WeaponType type in System.Enum.GetValues(typeof(WeaponType)))
        {
            if (database.IsWeaponUnlocked(type))
                unlockedWeapons.Add(type);

            if (weaponUIs.TryGetValue(type, out GameObject ui))
                ui.SetActive(false);
        }

        for (int i = 0; i < unlockedWeapons.Count; i++)
        {
            WeaponType type = unlockedWeapons[i];
            if (!weaponUIs.TryGetValue(type, out GameObject obj))
                continue;

            obj.SetActive(true);
            RectTransform rt = obj.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(slotXPositions[i], slotYPosition);
        }

        currentIndex = 0;
        UpdatePosition();
    }

    public void TryMove(int delta)
    {
        int nextIndex = currentIndex + delta;

        if (nextIndex < 0 || nextIndex >= unlockedWeapons.Count)
            return;

        currentIndex = nextIndex;
        UpdatePosition();
    }

    void UpdatePosition()
    {
        if (slotXPositions.Length < unlockedWeapons.Count)
        {
            Debug.LogError("슬롯 X 좌표 배열이 해금된 무기 수보다 적습니다.");
            return;
        }

        float newX = -slotXPositions[currentIndex];
        weaponUIContainer.DOAnchorPosX(newX, 0.3f)
            .SetEase(Ease.OutCubic)
            .SetUpdate(true);
    }
}