using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using UnityEngine.EventSystems;
using TMPro;

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
    public TMP_Text keyText;
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

    void OnEnable()
    {
        KeyBindingResetter.OnReset += UpdateKeyText;
    }

    void OnDisable()
    {
        KeyBindingResetter.OnReset -= UpdateKeyText;
    }

    void SetupBinding()
    {
        StopCoroutine(WaitForKey());

        object container = keyType == KeyType.Player ? (object)keyData.Player : keyData.UI;
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
            UpdateKeyText();
        };
    }

    public void UpdateKeyText()
    {
        if (keyText == null || getKey == null)
        {
            keyText.text = "???";
            return;
        }

        keyText.text = getKey().ToString();
    }

    IEnumerator WaitForKey()
    {
        if (InputManager.Instance.IsCapturingKey) yield break;

        keyText.text = "키 입력";
        EventSystem.current.SetSelectedGameObject(null);
        InputManager.Instance.IsCapturingKey = true;
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

        InputManager.Instance.IsCapturingKey = false;
        rebindButton.interactable = true;
        Debug.Log("입력 종료");
    }
}