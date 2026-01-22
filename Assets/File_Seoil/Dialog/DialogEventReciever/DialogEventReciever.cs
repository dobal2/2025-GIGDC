using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class DialogEventReciever : MonoBehaviour
{
    [SerializeField] private DialogAnimatonEventReciever dialogAnimatonEventReciever;

    private Dialog dialog;

    public Dictionary<string, Action<string>> DialogEvent { get; private set; }

    public void Initialize(Dialog dialog)
    {
        this.dialog = dialog;

        RegisterCommands();

        dialogAnimatonEventReciever.Initialize(dialog);
    }

    private void RegisterCommands()
    {

    }

    private void StartBoss(string line)
    {
        Boss.Instance.StartBattle();
    }

    private void MoveCharactor(string line)
    {

    }

    private void ShakeCamera(string line)
    {
        float duration = 0;

        try
        {
            duration = Convert.ToSingle(line);
        }
        catch (FormatException)
        {
            Debug.LogWarning("Invalid ShakeCamera Parameters: " + line);
            return;
        }

        CameraUtility.TopCamera.transform
            .DOShakePosition(
                duration: duration,
                strength: 0.5f
                );
    }

    private void ScaleCamera(string line)
    {

    }

    private void FadeCameraToDark(string line)
    {

    }

    private void FadeCameraToBright(string line)
    {

    }

    private void ChangeScene(string line)
    {
        SceneController.Instance.LoadScene(line);
    }

    private void GiveItem(string line)
    {
        switch (line)
        {
            case "Bow":
                WeaponDatabase.Instance.unlockedWeapons.Bow = true;
                break;
            case "Bomb":
                WeaponDatabase.Instance.unlockedWeapons.Bomb = true;
                break;
        }
    }

    private void PlaySound(string line)
    {

    }

    private void MoveCamera(string line)
    {

    }

    private void SetTimeScale(string line)
    {

    }

    private void Progess()
    {
        Stage.Progress();
    }

    private void AnimateCharactor(string line)
    {
        if(dialogAnimatonEventReciever.OnAnimationEvent.ContainsKey(line))
        {
            dialogAnimatonEventReciever.OnAnimationEvent[line].Invoke();
        }
        else throw new KeyNotFoundException($"{line}");
    }
}
