using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(CinemachineCamera))]
public class FollowTargetFixer : MonoBehaviour
{
    private void Start()
    {
        if(PlayerController.Instance != null)
        {
            GetComponent<CinemachineCamera>().Target.TrackingTarget = PlayerController.Instance.transform;
        }
        else
        {
            GetComponent<CinemachineCamera>().Target.TrackingTarget = LobbyPlayerController.Instance.transform;
        }
    }
        
}
