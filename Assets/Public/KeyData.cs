using UnityEngine;

[CreateAssetMenu(fileName = "KeyData", menuName = "Scriptable Objects/KeyData")]
public class KeyData : ScriptableObject
{
    public PlayerData Player;

    [System.Serializable]
    public class PlayerData
    {
        [Header("Moves")]
        public KeyCode LeftMoveKey;
        public KeyCode RightMoveKey;
        public KeyCode RunKey;
        public KeyCode JumpKey;

        [Header("Combats")]
        public KeyCode AttackKey;
        public KeyCode SkillKey;

        [Header("Interactions")]
        public KeyCode InteractionKey;
        public KeyCode ItemSelectionLeftKey;
        public KeyCode ItemSelectionRightKey;
        public KeyCode OnItemSelectionKey;
        public KeyCode OffItemSelectionKey;

        [Header("UI")]
        public KeyCode UpKey;
        public KeyCode DownKey;
        public KeyCode LeftKey;
        public KeyCode RightKey;
        public KeyCode SelectKey;
    }
}
