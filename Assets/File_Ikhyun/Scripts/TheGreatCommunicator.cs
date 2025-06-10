using System;
using UnityEngine;

public class TheGreatCommunicator : MonoBehaviour {
    [SerializeField] private Boolean needCommunication = true;
    
    
    private void Update() {
        if (needCommunication && Input.GetKeyDown(KeyCode.Z)) {
            Debug.LogError("NullReferenceException: Object reference not set to an instance of an object\nat PlayerController.Update () [0x00010] in Assets/File_Hyun/Scripts/Player/PlayerController.cs:45\n");
        }
    }

    
    
}
