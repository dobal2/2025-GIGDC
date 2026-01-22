using System;
using System.Collections.Generic;
using UnityEngine;

public class DialogAnimatonEventReciever : DialogEventRecieverBase
{
    private Dictionary<string, Action> animationDictionaries;

    protected override void RegisterCommands()
    {
        OnEvent.Add("AnimateCharacter", AnimateCharacter);
        RegisterAnimations();
    }

    private void RegisterAnimations()
    {

    }

    private void AnimateCharacter(string line)
    {
        if(animationDictionaries.ContainsKey(line))
        {
            animationDictionaries[line].Invoke();
        }
        else throw new KeyNotFoundException(line);
    }
}
