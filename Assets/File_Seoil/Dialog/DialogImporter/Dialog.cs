using System;
using UnityEngine;

[Serializable]
public class Dialog
{
    public TargetData Target;
    [field: SerializeField] public string Line { get; private set; }

    public Dialog(TargetData target, string line)
    {
        Target = target;
        Line = line;
    }
}
