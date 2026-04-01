using System;
using UnityEngine;

public class TitleScreenManager : MonoBehaviour
{
    [SerializeField] private WeaponDatabase weaponData;

    void Start()
    {
        SaveKey.Instance.LoadKeyBindings();
        InputManager.Instance.CurrentContext = InputManager.InputContext.UI;
    }

    public void Continue()
    {
        Debug.Log("이어하기");
        Stage.LoadData();
        weaponData.unlockedWeapons = new UnlockedWeapons { Spear = true, Bow = true, Bomb = true };
        SceneLoader.Instance.LoadScene(SceneType.Lobby_Over);
    }

    public void NewGame()
    {
        Debug.Log("처음부터");
        Stage.Data = StageDataType.Start;
        weaponData.unlockedWeapons = new UnlockedWeapons { Spear = true, Bow = true, Bomb = true };
        SceneLoader.Instance.LoadScene(SceneType.Lobby_Over);
    }

    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}