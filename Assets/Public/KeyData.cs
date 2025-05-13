using UnityEngine;

[CreateAssetMenu(fileName = "KeyData", menuName = "Scriptable Objects/KeyData")]
public class KeyData : ScriptableObject
{
    public PlayerData Player;
    public UI Ui;

    [System.Serializable]
    public class PlayerData
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
    }

    [System.Serializable]
    public class UI
    {
        [Header("UI")]
        public KeyCode UpKey;
        public KeyCode DownKey;
        public KeyCode LeftKey;
        public KeyCode RightKey;
        public KeyCode SelectKey;
    }
}
