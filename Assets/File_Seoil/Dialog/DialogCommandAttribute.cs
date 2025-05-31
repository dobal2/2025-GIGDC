using System;
using UnityEngine;

public class DialogCommandAttribute : Attribute
{
    public string CommandName { get; private set; }
    public DialogCommandAttribute(string commandName)
    {
        CommandName = commandName;
    }
}
