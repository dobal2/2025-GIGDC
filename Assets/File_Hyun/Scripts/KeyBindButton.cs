using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class KeyBindButton : MonoBehaviour
{
    public enum KeyType { Left, Right, Jump, Attack }
    public KeyType keyType;

    public Text displayText;
    public Button button;

    private bool waitingForKey = false;
    private KeyData activeKeyData;

    public void Initialize(KeyData keyData)
    {
        activeKeyData = keyData;
        UpdateDisplay();
        button.onClick.AddListener(StartBinding);
    }

    void StartBinding()
    {
        waitingForKey = true;
        displayText.text = "눌러주세요...";
    }

    void Update()
    {
        if (!waitingForKey) return;

        foreach (KeyCode code in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(code))
            {
                AssignKey(code);
                break;
            }
        }
    }

    void AssignKey(KeyCode newKey)
    {
        waitingForKey = false;

        switch (keyType)
        {
            case KeyType.Left: activeKeyData.Player.LeftMoveKey = newKey; break;
            case KeyType.Right: activeKeyData.Player.RightMoveKey = newKey; break;
            case KeyType.Jump: activeKeyData.Player.JumpKey = newKey; break;
            case KeyType.Attack: activeKeyData.Player.AttackKey = newKey; break;
        }

        UpdateDisplay();
    }

    void UpdateDisplay()
    {
        KeyCode currentKey = KeyCode.None;
        switch (keyType)
        {
            case KeyType.Left: currentKey = activeKeyData.Player.LeftMoveKey; break;
            case KeyType.Right: currentKey = activeKeyData.Player.RightMoveKey; break;
            case KeyType.Jump: currentKey = activeKeyData.Player.JumpKey; break;
            case KeyType.Attack: currentKey = activeKeyData.Player.AttackKey; break;
        }

        displayText.text = currentKey.ToString();
    }
}