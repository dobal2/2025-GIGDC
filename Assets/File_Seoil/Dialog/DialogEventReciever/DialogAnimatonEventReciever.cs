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
        animationDictionaries.Add("Captain_Walk", Captain_Walk);
        animationDictionaries.Add("Captain_Sitdown", Captain_Sitdown);
        animationDictionaries.Add("Captain_Transform", Captain_Transform);
        animationDictionaries.Add("Death_Walk", Death_Walk);
        animationDictionaries.Add("Death_Greet", Death_Greet);
    }

    private void AnimateCharacter(string line)
    {
        if(animationDictionaries.ContainsKey(line))
        {
            animationDictionaries[line].Invoke();
        }
        else throw new KeyNotFoundException(line);
    }

    private void Captain_Walk()
    {
        var captainController = FindAnyObjectByType<CaptainController>();
        captainController.Captain_Walk();
    }

    private void Captain_Sitdown()
    {
        var captainController = FindAnyObjectByType<CaptainController>();
        captainController.Captain_Sitdown();
    }

    private void Captain_Transform()
    {
        var captainController = FindAnyObjectByType<CaptainController>();
        captainController.Captain_Transform();
    }

    private void Death_Walk()
    {
        var captainController = FindAnyObjectByType<CaptainController>();
        captainController.Death_Walk();
    }

    private void Death_Greet()
    {
        var captainController = FindAnyObjectByType<CaptainController>();
        captainController.Death_Greet();
    }
}