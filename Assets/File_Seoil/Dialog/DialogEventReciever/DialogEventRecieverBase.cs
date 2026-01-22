using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class DialogEventRecieverBase : MonoBehaviour
{
    public Dictionary<string, Action<string>> OnEvent { get; protected set; } = new();

    protected Dialog dialog;

    public virtual void Initialize(Dialog dialog)
    {
        this.dialog = dialog;

        RegisterCommands();
    }

    protected abstract void RegisterCommands();
}
