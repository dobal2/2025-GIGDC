using System;
using UnityEngine;
using System.Reflection;

public class KeyBindingResetter : MonoBehaviour
{
    [Header("현재 키값")]
    public KeyData currentKeyData;

    [Header("기본 키값")]
    public KeyData defaultKeyData;

    public static event Action OnReset;

    [ContextMenu("기본값으로 초기화")]
    public void ResetToDefault()
    {
        if (currentKeyData == null || defaultKeyData == null)
        {
            Debug.LogWarning("KeyData가 할당되지 않았습니다.");
            return;
        }

        CopyAllFields(currentKeyData.Player, defaultKeyData.Player);
        CopyAllFields(currentKeyData.UI, defaultKeyData.UI);

        OnReset?.Invoke();
        Debug.Log("키 설정이 기본값으로 초기화되었습니다.");
    }

    void CopyAllFields(object target, object source)
    {
        if (target == null || source == null) return;

        var fields = target.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
        foreach (var field in fields)
        {
            if (field.FieldType == typeof(KeyCode))
            {
                var value = field.GetValue(source);
                field.SetValue(target, value);
            }
        }
    }

    public void SaveKeyBindings()
    {
        SaveKey.Instance.SaveKeyBindings();
    }
}