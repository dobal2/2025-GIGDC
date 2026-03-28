using System;
using System.Collections.Generic;

public class DialogAnimatonEventReciever : DialogEventRecieverBase
{
    private Dictionary<string, Action> animationDictionaries = new();

    protected override void RegisterCommands()
    {
        OnEvent.Add("AnimateCharacter", AnimateCharacter);
        RegisterAnimations();
    }

    private void RegisterAnimations()
    {
        animationDictionaries.Add("Byeongtae1_AwakeUp", Byeongtae1_AwakeUp);
        animationDictionaries.Add("Byeongtae1_Lay", Byeongtae1_Lay);
        animationDictionaries.Add("Byeongtae1_TurnHead", Byeongtae1_TurnHead);
        animationDictionaries.Add("Byeongtae2_Shrink", Byeongtae2_Shrink);
        animationDictionaries.Add("Byeongtae4_Walk", Byeongtae4_Walk);
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

    private void Byeongtae1_AwakeUp()
    {
        var lobbyPlayerController = FindAnyObjectByType<LobbyPlayerController>();
        lobbyPlayerController.Byeongtae1_AwakeUp();
    }

    private void Byeongtae1_Lay()
    {
        var lobbyPlayerController = FindAnyObjectByType<LobbyPlayerController>();
        lobbyPlayerController.Byeongtae1_Lay();
    }

    private void Byeongtae1_TurnHead()
    {
        var lobbyPlayerController = FindAnyObjectByType<LobbyPlayerController>();
        lobbyPlayerController.Byeongtae1_TurnHead();
    }

    private void Byeongtae2_Shrink()
    {
        var lobbyPlayerController = FindAnyObjectByType<LobbyPlayerController>();
        lobbyPlayerController.Byeongtae2_Shrink();
    }

    private void Byeongtae4_Walk()
    {
        var lobbyPlayerController = FindAnyObjectByType<LobbyPlayerController>();
        lobbyPlayerController.Byeongtae4_Walk();
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