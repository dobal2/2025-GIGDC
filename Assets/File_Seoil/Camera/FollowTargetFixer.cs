using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CinemachineCamera))]
public class FollowTargetFixer : MonoBehaviour
{
    private void Awake() =>
        GetComponent<CinemachineCamera>().Target.TrackingTarget = PlayerController.Instance?.transform??LobbyPlayerController.Instance.transform;
}
