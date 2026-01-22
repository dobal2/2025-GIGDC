using System;
using UnityEngine;

public class DialogAudioEventReciever : DialogEventRecieverBase
{
    protected override void RegisterCommands()
    {
        OnEvent.Add("PlaySound", PlaySound);
    }

    private void PlaySound(string line)
    {
        throw new NotImplementedException();
    }
}
