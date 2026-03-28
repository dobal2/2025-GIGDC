using System;
using UnityEngine;

public class DialogCharacterEventReciever : DialogEventRecieverBase
{
    protected override void RegisterCommands()
    {
        OnEvent.Add("MoveCharactor", MoveCharactor);
    }

    private void MoveCharactor(string line)
    {
        //throw new NotImplementedException();
    }
}
