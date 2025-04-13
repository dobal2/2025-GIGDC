using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    [Header("Scriptable Objects")]
    [SerializeField] private KeyData keyData;

    [Header("Player MonoBehaviors")]
    [SerializeField] private PlayerMoveController moveController;

    private Dictionary<KeyCode, Action> keyDownActions;
    private Dictionary<KeyCode, Action> keyUpActions;

    private void Awake()
    {
        InitializeActions();
    }

    private void InitializeActions()
    {
        keyDownActions = new Dictionary<KeyCode, Action>
        {
            {keyData.Player.LeftMoveKey, moveController.OnMoveLeftKeyDown},
            {keyData.Player.RightMoveKey, moveController.OnMoveRightKeyDown}
        };

        keyUpActions = new Dictionary<KeyCode, Action>
        {
            {keyData.Player.LeftMoveKey, moveController.OnMoveLeftKeyUp},
            {keyData.Player.RightMoveKey, moveController.OnMoveRightKeyUp}
        };
    }

    private void Update()
    {
        foreach(var actions in keyDownActions)
        {
            if (Input.GetKeyDown(actions.Key)) actions.Value.Invoke();
        }

        foreach(var actions in keyUpActions)
        {
            if (Input.GetKeyUp(actions.Key)) actions.Value.Invoke();
        }
    }
}
