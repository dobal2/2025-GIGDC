using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using UnityEngine.EventSystems;

public class KeyBindButton : MonoBehaviour
{
    [Header("키 데이터")]
    public KeyData keyData;

    public enum KeyType { Player, UI }
    [Header("키 타입")]
    public KeyType keyType;

    [Header("대상 키 이름")]
    public string fieldName;

    [Header("키 레이블")]
    public Text labelText;
    public Text keyText;
    public Button rebindButton;

    private Func<KeyCode> getKey;
    private Action<KeyCode> setKey;

    void Start()
    {
        SetupBinding();
        keyText.text = getKey().ToString();

        rebindButton.onClick.AddListener(() =>
        {
            StartCoroutine(WaitForKey());
        });
    }

    void SetupBinding()
    {
        object container = keyType == KeyType.Player ? (object)keyData.Player : keyData.Ui;
        var field = container.GetType().GetField(fieldName);
        if (field == null || field.FieldType != typeof(KeyCode))
        {
            Debug.LogError($"KeyBindButton: '{fieldName}' is not a valid KeyCode field.");
            return;
        }

        getKey = () => (KeyCode)field.GetValue(container);
        setKey = (key) =>
        {
            field.SetValue(container, key);
            keyText.text = key.ToString();
        };
    }

    IEnumerator WaitForKey()
    {
        keyText.text = "키 입력";
        EventSystem.current.SetSelectedGameObject(null);
        rebindButton.interactable = false;
        yield return null;

        bool keyCaptured = false;
        while (!keyCaptured)
        {
            yield return null;

            foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(key))
                {
                    Debug.Log($"감지된 키: {key}");
                    setKey.Invoke(key);
                    keyCaptured = true;
                    break;
                }
            }
        }

        Debug.Log("입력 종료");

        yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
        rebindButton.interactable = true;
    }
}