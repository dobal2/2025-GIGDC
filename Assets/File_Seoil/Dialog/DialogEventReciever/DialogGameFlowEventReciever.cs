using System;
using UnityEngine;

public class DialogGameFlowEventReciever : DialogEventRecieverBase
{
    protected override void RegisterCommands()
    {
        OnEvent.Add("StartBoss", StartBoss);
        OnEvent.Add("ChangeScene", ChangeScene);
        OnEvent.Add("GiveItem", GiveItem);
        OnEvent.Add("Progress", Progress);
        OnEvent.Add("SetTimeScale", SetTimeScale);
    }

    private void StartBoss(string line)
    {
        Boss.Instance.StartBattle();
    }

    private void ChangeScene(string line)
    {
        SceneLoader.Instance.LoadScene(line);
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

    private void Progress(string line)
    {
        Stage.Progress();
    }

    private void SetTimeScale(string line)
    {
        throw new NotImplementedException();
    }
}
