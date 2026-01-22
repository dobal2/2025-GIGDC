using UnityEngine;
using System;
using System.Reflection;

public class SaveKey : MonoBehaviour
{
    public static SaveKey Instance { get; private set; }

    [Header("저장할 키 데이터")]
    public KeyData keyData;

    private const string PlayerPrefix = "Key_Player_";
    private const string UiPrefix = "Key_UI_";

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    [ContextMenu("Save Key Bindings")]
    public void SaveKeyBindings()
    {
        SaveFields(keyData.Player, PlayerPrefix);
        SaveFields(keyData.UI, UiPrefix);
        PlayerPrefs.Save();
        Debug.Log("키 설정 저장 완료");
    }

    [ContextMenu("Load Key Bindings")]
    public void LoadKeyBindings()
    {
        LoadFields(keyData.Player, PlayerPrefix);
        LoadFields(keyData.UI, UiPrefix);
        Debug.Log("키 설정 불러오기 완료");
    }

    void SaveFields(object target, string prefix)
    {
        var fields = target.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
        foreach (var field in fields)
        {
            if (field.FieldType == typeof(KeyCode))
            {
                string keyName = prefix + field.Name;
                KeyCode value = (KeyCode)field.GetValue(target);
                PlayerPrefs.SetString(keyName, value.ToString());
            }
        }
    }

    void LoadFields(object target, string prefix)
    {
        var fields = target.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
        foreach (var field in fields)
        {
            if (field.FieldType == typeof(KeyCode))
            {
                string keyName = prefix + field.Name;
                if (PlayerPrefs.HasKey(keyName))
                {
                    string keyStr = PlayerPrefs.GetString(keyName);
                    if (Enum.TryParse(keyStr, out KeyCode parsedKey))
                    {
                        field.SetValue(target, parsedKey);
                    }
                    else
                    {
                        Debug.LogWarning($"잘못된 키 값: {keyStr}");
                    }
                }
            }
        }
    }
}