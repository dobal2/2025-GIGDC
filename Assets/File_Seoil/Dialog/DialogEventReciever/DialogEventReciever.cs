using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class DialogEventReciever : DialogEventRecieverBase
{
    [SerializeField] private DialogAnimatonEventReciever animatonEventReciever;
    [SerializeField] private DialogAudioEventReciever audioEventReciever;
    [SerializeField] private DialogCameraEventReciever cameraEventReciever;
    [SerializeField] private DialogCharacterEventReciever characterEventReciever;
    [SerializeField] private DialogGameFlowEventReciever gameFlowEventReciever;

    protected override void RegisterCommands()
    {
        animatonEventReciever.Initialize(dialog);
        audioEventReciever.Initialize(dialog);
        cameraEventReciever.Initialize(dialog);
        characterEventReciever.Initialize(dialog);
        gameFlowEventReciever.Initialize(dialog);

        RegisterSubEventCommands(animatonEventReciever.OnEvent);
        RegisterSubEventCommands(audioEventReciever.OnEvent);
        RegisterSubEventCommands(cameraEventReciever.OnEvent);
        RegisterSubEventCommands(characterEventReciever.OnEvent);
        RegisterSubEventCommands(gameFlowEventReciever.OnEvent);
    }

    private void RegisterSubEventCommands(Dictionary<string, Action<string>> eventDictionary)
    {
        foreach (var pair in eventDictionary)
        {
            OnEvent[pair.Key] = pair.Value;
        }
    }
}
