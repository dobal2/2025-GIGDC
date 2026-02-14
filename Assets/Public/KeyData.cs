using UnityEngine;

[CreateAssetMenu(fileName = "KeyData", menuName = "Scriptable Objects/KeyData")]
public class KeyData : ScriptableObject
{
    public PlayerKey Player;
    public UIKey UI;

    [System.Serializable]
    public class PlayerKey
    {
        [Header("Moves")]
        public KeyCode LeftMoveKey;
        public KeyCode RightMoveKey;
        public KeyCode DownMoveKey;
        public KeyCode DashKey;
        public KeyCode JumpKey;

        [Header("Combats")]
        public KeyCode AttackKey;
        public KeyCode SkillKey;

        [Header("Interactions")]
        public KeyCode InteractionKey;
        public KeyCode WeaponchangeKey;
        public KeyCode ProcessKey;
    }

    [System.Serializable]
    public class UIKey
    {
        [Header("UI")]
        public KeyCode UpKey;
        public KeyCode DownKey;
        public KeyCode LeftKey;
        public KeyCode RightKey;
        public KeyCode SelectKey;
        public KeyCode EscapeKey;
    }
}
