using System;
using System.Collections.Generic;
using UnityEngine;

public class DialogAnimatonEventReciever : MonoBehaviour
{
    public Dictionary<string, Action> OnAnimationEvent;

    private Dialog dialog;

    public void Initialize(Dialog dialog)
    {
        this.dialog = dialog;

        RegisterCommands();
    }

    private void RegisterCommands()
    {

    }
}
